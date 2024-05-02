using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Logs;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Services.Logs;

public class LogService : ILogService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public LogService(RecordStoreContext context, IMapper mapper, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }
    
    public async Task<List<LogResponse>> GetAllAsync(GetLogQueryParams queryParams)
    {
        var from = queryParams.From ?? DateOnly.MinValue;
        var to = queryParams.To ?? DateOnly.MaxValue;
        
        var fromDateTime = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, DateTimeKind.Utc);
        var toDateTime = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, DateTimeKind.Utc);
        
        var logs = await _context.Logs
            .Where(l => l.Timestamp >= fromDateTime && l.Timestamp <= toDateTime)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
        
        var response = _mapper.Map<List<LogResponse>>(logs);

        return response;
    }

    public async Task<LogResponse> CreateAsync(LogCreateRequest entity)
    {
        var log = _mapper.Map<Log>(entity);

        var user = await GetCurrentUserAsync();

        log.UserId = user?.Id ?? null;
        
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
        
        var response = _mapper.Map<LogResponse>(log);

        return response;
    }

    public async Task LogActionAsync(string action, string description)
    {
        var user = await GetCurrentUserAsync();
        var userId = user?.Id;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "call insert_log(@userId, @action, @description)";
                cmd.Parameters.Add(new NpgsqlParameter("userId", NpgsqlDbType.Integer) { Value = userId ?? (object)DBNull.Value });
                cmd.Parameters.Add(new NpgsqlParameter("action", NpgsqlDbType.Varchar) { Value = action });
                cmd.Parameters.Add(new NpgsqlParameter("description", NpgsqlDbType.Varchar) { Value = description });

                await cmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    private async Task<UserResponse?> GetCurrentUserAsync()
    {
        try
        {
            return await _userService.GetCurrentUserAsync();
        }
        catch (Exception e) when (e is InvalidOperationException or UserNotFoundException)
        {
            return null;
        }
    }
}