using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordCredentialsServer
{
    /// <summary>
    /// Identity配置，初始化Identityserver
    /// </summary>
    public class Config
    {
        //scopes定义
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                //给api资源定义一个scopes
                new ApiResource("password_credentials_api","密码模式")
            };
        }

        //定义可以访问该API的客户端
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "password_client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,//密码模式
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "password_credentials_api" }
                }
            };
        }

        /// <summary>
        /// 定义测试用户
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "user",
                    Password = "password"
                }
            };
        }
    }
}
