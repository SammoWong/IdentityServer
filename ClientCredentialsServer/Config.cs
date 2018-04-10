using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer
{
    /// <summary>
    /// Identity配置，初始化Identityserver
    /// </summary>
    public class Config
    {
        //scopes定义
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                //给api资源定义一个scopes
                new ApiResource("client_credentials_api","客户端模式")
            };
        }

        //定义可以访问该API的客户端
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,//客户端模式
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "client_credentials_api" }
                }
            };
        }
    }
}
