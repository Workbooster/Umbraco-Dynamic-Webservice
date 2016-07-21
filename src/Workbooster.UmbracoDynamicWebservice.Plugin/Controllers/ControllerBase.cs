using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Workbooster.UmbracoDynamicWebservice.Plugin.Controllers
{
    public abstract class ControllerBase : UmbracoApiController
    {
        public IUser UmbracoUser { get; private set; }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            if (IsAuthorizedRequest(controllerContext))
            {
                base.Initialize(controllerContext);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        /// <summary>
        /// Checks whether the basic authentication header contains a Base64 encoded username
        /// and password that matches with the credentials of an Umbraco user that has the 
        /// UserType set to the value in DynamicWebservicePlugin.ALLOWED_USER_TYPE_ALIAS.
        /// 
        /// HEADER EXAMPLE:
        /// Authorization:Basic V2Vic2VydmljZTphYmNkJDEyMzQ=
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        protected bool IsAuthorizedRequest(HttpControllerContext controllerContext)
        {
            var auth = controllerContext.Request.Headers.Authorization;

            if (auth != null
                && auth.Scheme == "Basic"
                && String.IsNullOrEmpty(auth.Parameter) == false)
            {
                // The username and password must be Base64 encoded
                // You can do that in C# by using the following command:
                // Convert.ToBase64String(Encoding.ASCII.GetBytes("Username:Password"))

                string authHeader = Encoding.Default.GetString(Convert.FromBase64String(auth.Parameter));

                var tokens = authHeader.Split(':');

                if (tokens.Length >= 2)
                {
                    string username = tokens[0];
                    string password = tokens[1];

                    if (UmbracoContext.Security.ValidateBackOfficeCredentials(username, password))
                    {
                        UmbracoUser = ApplicationContext.Services.UserService.GetByUsername(username);

                        // Check whether the user is an API user
                        if (UmbracoUser != null && UmbracoUser.UserType.Alias == DynamicWebservicePlugin.ALLOWED_USER_TYPE_ALIAS)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
