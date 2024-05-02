
namespace RecordStore.Api.RequestHelpers.QueryParams;

public class GetUserQueryParams : GetQueryParams
{
    public string? Email { get; set; }
    public string? RoleName { get; set; }
    public string? Name { get; set; }
}