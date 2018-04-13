using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HybridClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using IdentityModel.Client;
using System;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;

namespace HybridClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        [Authorize]
        public async Task<IActionResult> GetIdentity()
        {
            await RefreshTokensAsync();
            var token = await HttpContext.GetTokenAsync("access_token");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = await client.GetStringAsync("http://localhost:5001/api/Identity");
                return Ok(new { value = content });
            }
        }

        [Authorize]
        [Route("Home/RefreshToken")]
        [HttpGet]
        public async Task RefreshTokensAsync()
        {
            var authServerInfo = await DiscoveryClient.GetAsync("http://localhost:5000");
            var client = new TokenClient(authServerInfo.TokenEndpoint, "hybrid_client", "secret");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");//获取老的refresh_token
            var response = await client.RequestRefreshTokenAsync(refreshToken);//使用client和老的获取老的refresh_token获取新的token
            var identityToken = await HttpContext.GetTokenAsync("identity_token");//获取原来的identity_token
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);//设置过期时间

            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = identityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = response.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = response.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };
            var authenticationInfo = await HttpContext.AuthenticateAsync("Cookies");//获取当前用户authentication信息
            authenticationInfo.Properties.StoreTokens(tokens);//设置新的token信息
            await HttpContext.SignInAsync("Cookies", authenticationInfo.Principal, authenticationInfo.Properties);//相当于重新登陆，更新用户的token信息
        }
    }
}
