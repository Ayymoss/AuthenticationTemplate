using System.Net.Http.Json;
using BlazorAuthenticationLearn.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorAuthenticationLearn.Client.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public AccountService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    public ClientGlobalAccountModel User { get; set; }

    public async Task LoginUser(ClientLoginAccountModel loginRegisterAccountModel)
    {
        var result = await _httpClient.PostAsJsonAsync("api/UserAccount/Login", loginRegisterAccountModel);
        await GetUserInfo(result);
    }

    public async Task CreateUser(ClientRegisterAccountModel userRegisterAccount)
    {
        var result = await _httpClient.PostAsJsonAsync("api/UserAccount/Register", userRegisterAccount);
        await GetUserInfo(result);
    }
    
    private async Task GetUserInfo(HttpResponseMessage result)
    {
        var response = await result.Content.ReadFromJsonAsync<ClientGlobalAccountModel>();

        User.Id = response!.Id;
        User.Username = response.Username;
        
        _navigationManager.NavigateTo("/");
    }
}
