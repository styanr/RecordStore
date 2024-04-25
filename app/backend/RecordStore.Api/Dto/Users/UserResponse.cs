namespace RecordStore.Api.Dto.Users;

public class UserResponse
{
    public int Id { get; set; }
    
    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Role { get; set; }

}