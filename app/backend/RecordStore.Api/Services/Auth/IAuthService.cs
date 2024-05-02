using RecordStore.Api.Dto.Users;

namespace RecordStore.Api.Services.Users;

public interface IAuthService
{
    public Task<string> RegisterUserAsync(UserRegisterDto userRegisterDto);
    public Task<string> LoginAsync(UserLoginDto userLoginDto);
    
    public Task CreateUserAsync(UserCreateRequest request);
}