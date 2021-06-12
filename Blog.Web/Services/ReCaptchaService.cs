using Blog.Web.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Web.Services
{
    public class ReCaptchaService : ICaptchaService
    {
        private readonly ReCaptchaSettings _reCaptchaSettings;
        private readonly HttpClient _httpClient;

        public ReCaptchaService(HttpClient httpClient, IOptions<ReCaptchaSettings> reCaptchaSettings)
        {
            _reCaptchaSettings = reCaptchaSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<bool> VerifyReCaptcha(string token)
        {
            var query = $"?secret={_reCaptchaSettings.SecretKey}&response={token}";
            var response = await _httpClient.GetFromJsonAsync<ReCaptchaResponse>(query);

            return response.Success && response.Score >= 0.7;
        }
    }
}
