using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Media
{
    public class CreateMediaDto
    {
        public List<MediaDto> Medias { get; set; }
        public List<FileData> Files { get; set; }
    }
}
