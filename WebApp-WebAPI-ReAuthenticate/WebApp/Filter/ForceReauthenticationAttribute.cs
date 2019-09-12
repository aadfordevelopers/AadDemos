using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filter
{
    public class ForceReauthenticationAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly int _afterTimeinSeconds;

        public ForceReauthenticationAttribute(int afterTimeinSeconds)
        {
            _afterTimeinSeconds = afterTimeinSeconds;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var actualAuthTime = int.TryParse(context.HttpContext.User.FindFirst("auth_time")?.Value, out int authTime);

            if (actualAuthTime && ToUnixTimestamp(DateTime.UtcNow) - authTime < _afterTimeinSeconds)
            {
                await next();
            }
            else
            {
                var state = new Dictionary<string, string> { { "forceReAuth", "true" } };
                await context.HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties(state)
                {
                    RedirectUri = context.HttpContext.Request.Path,
                });
            }
        }

        private long ToUnixTimestamp(DateTime d)
        {
            var epoch = d - new DateTime(1970, 1, 1, 0, 0, 0);

            return (long)epoch.TotalSeconds;
        }
    }
}
