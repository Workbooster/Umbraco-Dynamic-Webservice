using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models;

namespace Workbooster.UmbracoDynamicWebservice.Plugin.Controllers
{
    [PluginController("DynamicWebservice")]
    public class MemberTypeController : ControllerBase
    {
        [HttpGet]
        public List<MemberTypeDto> Read()
        {
            List<MemberTypeDto> listOfSchemas = new List<MemberTypeDto>();

            IMemberType[] listOfMemberTypes = ApplicationContext
                .Services
                .MemberTypeService
                .GetAll()
                .ToArray();

            foreach (var memberType in listOfMemberTypes)
            {
                MemberTypeDto schema = new MemberTypeDto()
                {
                    Id = memberType.Id,
                    Alias = memberType.Alias,
                    Name = memberType.Name,
                    Description = memberType.Description,

                    Properties = ContentTypeController.ConvertToPropertyDto(memberType.PropertyTypes, ApplicationContext),
                };

                listOfSchemas.Add(schema);
            }

            return listOfSchemas;
        }
    }
}
