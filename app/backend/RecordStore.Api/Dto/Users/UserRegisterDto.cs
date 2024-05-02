using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Users;

public class UserRegisterDto
{
    [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}