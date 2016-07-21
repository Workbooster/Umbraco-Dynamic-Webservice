using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Media;

namespace Workbooster.UmbracoDynamicWebservice.Plugin.Controllers
{
    [PluginController("DynamicWebservice")]
    public class MediaController : ControllerBase
    {
        #region PUBLIC METHODS

        [HttpPost]
        public List<MediaDto> Create(CreateMediaDto createMediaDto)
        {
            List<MediaDto> listOfCreatedMedia = new List<MediaDto>();

            foreach (var metadata in createMediaDto.Medias)
            {
                // search for a matching file upload
                var file = createMediaDto.Files.FirstOrDefault(f => f.FileName == metadata.FileName);

                if (file == null)
                {
                    // no file upload found
                }
                else
                {
                    var fileExtension = metadata.FileName.Substring(metadata.FileName.LastIndexOf('.') + 1).ToLower();

                    var mediaType = Constants.Conventions.MediaTypes.File;

                    if (UmbracoConfig.For.UmbracoSettings().Content.ImageFileTypes.Contains(fileExtension))
                        mediaType = Constants.Conventions.MediaTypes.Image;

                    IMedia media = ApplicationContext.Services.MediaService.CreateMedia(metadata.FileName, metadata.ParentId, mediaType);

                    media.SetValue(Constants.Conventions.Media.File, metadata.FileName, new MemoryStream(file.Data));

                    ApplicationContext.Services.MediaService.Save(media);
                }
            }

            return listOfCreatedMedia;
        }

        #endregion
    }
}
