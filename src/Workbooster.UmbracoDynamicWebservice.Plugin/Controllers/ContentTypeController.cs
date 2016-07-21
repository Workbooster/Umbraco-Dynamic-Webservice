using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Umbraco.Web.WebApi.Filters;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models;

namespace Workbooster.UmbracoDynamicWebservice.Plugin.Controllers
{
    [PluginController("DynamicWebservice")]
    public class ContentTypeController : ControllerBase
    {
        #region PUBLIC METHODS

        [HttpGet]
        public List<ContentTypeDto> Read()
        {
            List<ContentTypeDto> listOfSchemas = new List<ContentTypeDto>();

            var listOfContentTypes = ApplicationContext
                .Services
                .ContentTypeService
                .GetAllContentTypes()
                .ToArray();

            foreach (var contentType in listOfContentTypes)
            {
                ContentTypeDto schema = new ContentTypeDto()
                {
                    Id = contentType.Id,
                    Alias = contentType.Alias,
                    Description = contentType.Description,

                    Properties = ConvertToPropertyDto(contentType.PropertyTypes, ApplicationContext),
                };

                listOfSchemas.Add(schema);
            }

            return listOfSchemas;
        }

        #endregion

        #region INTERNAL METHODS

        internal static List<PropertyDto> ConvertToPropertyDto(IEnumerable<PropertyType> listOfPropertyTypes, ApplicationContext applicationContext)
        {
            List<PropertyDto> listOfDtos = new List<PropertyDto>();

            // load the Umbraco type definitions which specifies the database data type
            IDataTypeDefinition[] listOfDataTypeDefinitions = applicationContext.Services.DataTypeService.GetAllDataTypeDefinitions().ToArray();

            foreach (var propertyType in listOfPropertyTypes)
            {
                if (propertyType.DataTypeDefinitionId != 0)
                {
                    // only consider properties with a data type definition

                    IDataTypeDefinition dataTypeDefinition = listOfDataTypeDefinitions
                        .Where(dtd => dtd.Id == propertyType.DataTypeDefinitionId)
                        .FirstOrDefault();

                    if (dataTypeDefinition == null)
                        throw new Exception(String.Format(
                            "DataTypeDefinition with Id={0} not found. Property alias = {1}",
                            propertyType.DataTypeDefinitionId,
                            propertyType.Alias));

                    // find out the .NET CLR data type from the Umbraco database data type

                    Type propertyDataType = null;

                    switch (dataTypeDefinition.DatabaseType)
                    {
                        case DataTypeDatabaseType.Date:
                            propertyDataType = typeof(DateTime);
                            break;
                        case DataTypeDatabaseType.Decimal:
                            propertyDataType = typeof(decimal);
                            break;
                        case DataTypeDatabaseType.Integer:
                            propertyDataType = typeof(int);
                            break;
                        case DataTypeDatabaseType.Ntext:
                            propertyDataType = typeof(string);
                            break;
                        case DataTypeDatabaseType.Nvarchar:
                            propertyDataType = typeof(string);
                            break;
                        default:
                            throw new Exception(String.Format(
                            "Unknown database type '{0}'. Property alias = {1}",
                            dataTypeDefinition.DatabaseType,
                            propertyType.Alias));
                    }

                    // Copy the property information to the DTO

                    listOfDtos.Add(new PropertyDto()
                    {
                        Name = propertyType.Name,
                        Alias = propertyType.Alias,
                        Type = propertyDataType,

                        IsNullable = propertyType.Mandatory == false
                            && String.IsNullOrEmpty(propertyType.ValidationRegExp),

                        Description = propertyType.Description,
                    });
                }
            }

            return listOfDtos;
        }

        #endregion
    }
}
