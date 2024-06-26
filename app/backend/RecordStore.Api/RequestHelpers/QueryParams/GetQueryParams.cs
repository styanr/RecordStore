﻿namespace RecordStore.Api.RequestHelpers.QueryParams;

public class GetQueryParams
{
    public string? OrderBy { get; set; }
    public string? OrderDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 16;
}