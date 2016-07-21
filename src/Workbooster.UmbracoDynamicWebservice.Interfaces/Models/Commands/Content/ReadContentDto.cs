using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Content
{
    public class ReadContentDto
    {
        public int? ParentId { get; set; }
        public ContentTypeDto ContentType { get; set; }
    }
}
