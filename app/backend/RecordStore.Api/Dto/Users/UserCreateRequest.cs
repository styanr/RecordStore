namespace RecordStore.Api.Dto.Users;

public class UserCreateRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    
    public int RoleId { get; set; }
}