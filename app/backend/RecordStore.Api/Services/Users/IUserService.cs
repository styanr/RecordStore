using RecordStore.Api.Dto.Users;

namespace RecordStore.Api.Services.Users;

public interface IUserService
{
    public Task RegisterAsync(UserRegisterDto userRegisterDto);
    public Task<string> LoginAsync(UserLoginDto userLoginDto);
}