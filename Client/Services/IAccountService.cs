using BlazorAuthenticationLearn.Shared.Models;

namespace BlazorAuthenticationLearn.Client.Services;

public interface IAccountService
{
    Task LoginUser(ClientLoginAccountModel loginRegisterAccountModel);
    Task CreateUser(ClientRegisterAccountModel userRegisterAccount);

}
