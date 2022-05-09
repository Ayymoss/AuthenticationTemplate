using BlazorAuthenticationLearn.Shared.Models;

namespace BlazorAuthenticationLearn.Client.Services;

public interface IAuthService
{
    Task Login(LoginRequest loginRegisterRequest);
    Task Register(RegisterRequest userRegisterAccount);
    Task Logout();
    Task<CurrentUser> CurrentUserInfo();

}
