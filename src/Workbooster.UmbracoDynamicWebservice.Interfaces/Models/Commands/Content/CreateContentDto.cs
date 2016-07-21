using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Content
{
    public class CreateContentDto
    {
        public ContentTypeDto ContentType { get; set; }
        public List<ContentDto> Contents { get; set; }
    }
}
