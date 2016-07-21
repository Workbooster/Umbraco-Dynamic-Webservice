using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Content;

namespace Workbooster.UmbracoDynamicWebservice.Plugin.Controllers
{
    [PluginController("DynamicWebservice")]
    public class ContentController : ControllerBase
    {
        #region PUBLIC METHODS

        [HttpPost]
        public List<ContentDto> Read([FromBody]ReadContentDto readCommand)
        {
            if (readCommand.ContentType == null || readCommand.ContentType.Id == default(int))
                throw new Exception("A content type is required.");
            
            IEnumerable<IContent> listOfNodes;

            if (readCommand.ParentId != null)
            {
                // load all children of the parent node
                // filter for the content type (only nodes of this type)
                listOfNodes = ApplicationContext.Services.ContentService
                    .GetChildren((int)readCommand.ParentId)
                    .Where(n => n.ContentTypeId == readCommand.ContentType.Id);
            }
            else
            {
                // load all content nodes of the given content type without considering the nodes position in the Umbraco content tree
                listOfNodes = ApplicationContext.Services.ContentService
                    .GetContentOfContentType(readCommand.ContentType.Id);
            }

            List<ContentDto> listOfContent = new List<ContentDto>();

            foreach (var node in listOfNodes)
            {
                ContentDto content = GetContentDto(node, readCommand.ContentType);

                listOfContent.Add(content);
            }

            return listOfContent;
        }

        [HttpPost]
        public List<ContentDto> Create([FromBody]CreateContentDto createCommand)
        {
            List<ContentDto> listOfCreatedContent = new List<ContentDto>();

            foreach (var content in createCommand.Contents)
            {
                IContent node = ApplicationContext.Services.ContentService
                    .CreateContent(content.Name, content.ParentId, createCommand.ContentType.Alias);

                for (int i = 0; i < createCommand.ContentType.Properties.Count; i++)
                {
                    PropertyDto property = createCommand.ContentType.Properties[i];
                    node.SetValue(property.Alias, content.PropertyValues[i]);
                }

                ApplicationContext.Services.ContentService.SaveAndPublishWithStatus(node);

                ContentDto createdContent = GetContentDto(node, createCommand.ContentType);

                listOfCreatedContent.Add(createdContent);

            }

            return listOfCreatedContent;
        }

        [HttpPost]
        public void Update([FromBody]UpdateContentDto createCommand)
        {
            foreach (var content in createCommand.Contents)
            {

                IContent node = ApplicationContext.Services.ContentService
                    .GetById(content.Id);

                for (int i = 0; i < createCommand.ContentType.Properties.Count; i++)
                {
                    PropertyDto property = createCommand.ContentType.Properties[i];
                    node.SetValue(property.Alias, content.PropertyValues[i]);
                }

                ApplicationContext.Services.ContentService.Save(node);

            }
        }

        [HttpPost]
        public void Delete([FromBody]UpdateContentDto createCommand)
        {
            foreach (var content in createCommand.Contents)
            {

                IContent node = ApplicationContext.Services.ContentService
                    .GetById(content.Id);

                ApplicationContext.Services.ContentService.Delete(node);
            }
        }

        #endregion

        #region INTERNAL METHODS

        protected ContentDto GetContentDto(IContent node, ContentTypeDto contentType)
        {
            ContentDto content = new ContentDto()
            {
                Id = node.Id,
                Key = node.Key.ToString(),
                Name = node.Name,
                ParentId = node.ParentId,
                PropertyValues = new object[contentType.Properties.Count],
            };

            for (int i = 0; i < contentType.Properties.Count; i++)
            {
                PropertyDto property = contentType.Properties[i];
                content.PropertyValues[i] = node.GetValue(property.Alias);
            }

            return content;
        }

        #endregion
    }
}
