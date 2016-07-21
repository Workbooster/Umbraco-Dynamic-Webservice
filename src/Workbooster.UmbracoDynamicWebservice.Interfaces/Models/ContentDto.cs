using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models
{
    public class ContentDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }

        public object[] PropertyValues { get; set; }
    }
}
