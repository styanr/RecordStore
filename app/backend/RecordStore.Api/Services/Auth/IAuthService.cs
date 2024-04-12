using RecordStore.Api.Dto.Users;

namespace RecordStore.Api.Services.Users;

public interface IAuthService
{
    public Task RegisterAsync(UserRegisterDto userRegisterDto);
    public Task<string> LoginAsync(UserLoginDto userLoginDto);
}