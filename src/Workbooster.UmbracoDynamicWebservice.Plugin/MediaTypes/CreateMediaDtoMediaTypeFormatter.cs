using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Media;

namespace Workbooster.UmbracoDynamicWebservice.Plugin.MediaTypes
{
    public class CreateMediaDtoMediaTypeFormatter : MediaTypeFormatter
    {
        public CreateMediaDtoMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(CreateMediaDto);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public async override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var provider = await content.ReadAsMultipartAsync();

            var modelContent = provider.Contents
                .FirstOrDefault(c => c.Headers.ContentDisposition.Name.Replace("\"", "") == "createmediadto");

            // read the metadata for all media files
            var createMediaDto = await modelContent.ReadAsAsync<CreateMediaDto>();

            // get a list of all binary file contents
            var listOfFileContents = provider.Contents
                .Where(c => Regex.IsMatch(c.Headers.ContentDisposition.Name.Replace("\"", ""), @"file\d+"))
                .ToList();

            createMediaDto.Files = new List<FileData>();

            // loop through all binary file contents
            foreach (var fileContent in listOfFileContents)
            {
                createMediaDto.Files.Add(new FileData()
                {
                    FileName = fileContent.Headers.ContentDisposition.FileName.Replace("\"", ""),
                    Data = await fileContent.ReadAsByteArrayAsync(),
                });
            }

            return createMediaDto;

        }
    }
}
