using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Core;
using Workbooster.UmbracoDynamicWebservice.Plugin.MediaTypes;

namespace Workbooster.UmbracoDynamicWebservice.Plugin
{
    public class WebApiApplication : ApplicationEventHandler 
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);

            GlobalConfiguration.Configure(config =>
            {
                // register our custom formatter for the upload of media items including binary file data
                config.Formatters.Add(new CreateMediaDtoMediaTypeFormatter());
            });

        }
    }
}
