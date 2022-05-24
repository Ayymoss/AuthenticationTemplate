using System.Net.Http.Json;
using BlazorAuthenticationLearn.Shared.Models;

namespace BlazorAuthenticationLearn.Client.Utilities;

    public class FileManager
    {
        private readonly HttpClient  _http;

        public FileManager(HttpClient http)
        {
            _http = http;
        }
        
        public async Task<bool> UploadFileChunk(FileChunk fileChunk)
        {
            try
            {
                var result = await _http.PostAsJsonAsync("api/File/Upload", fileChunk);
                result.EnsureSuccessStatusCode();
                var responseBody = await result.Content.ReadAsStringAsync();
                return Convert.ToBoolean(responseBody);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

