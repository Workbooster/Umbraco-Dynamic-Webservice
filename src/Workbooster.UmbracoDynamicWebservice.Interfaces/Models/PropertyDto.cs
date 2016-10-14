using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models
{
    public class PropertyDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public Type Type { get; set; }
        public bool IsNullable { get; set; }
        public string ValidationRegExp { get; set; }
        public string Description { get; set; }
    }
}
