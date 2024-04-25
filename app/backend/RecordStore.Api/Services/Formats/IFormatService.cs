using RecordStore.Api.Dto.Formats;

namespace RecordStore.Api.Services.Formats;

public interface IFormatService
{
    public Task<List<FormatResponseDto>> GetFormatsAsync(string name);
}