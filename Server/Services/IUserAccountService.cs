using BlazorAuthenticationLearn.Shared;

namespace BlazorAuthenticationLearn.Server.Services;

public interface IUserAccountService
{
    Task<List<UserAccountDto>> GetAll();
}
