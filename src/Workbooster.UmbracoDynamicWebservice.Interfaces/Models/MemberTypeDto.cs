using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models
{
    public class MemberTypeDto
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<PropertyDto> Properties { get; set; }

        public MemberTypeDto()
        {
            Properties = new List<PropertyDto>();
        }
    }
}
