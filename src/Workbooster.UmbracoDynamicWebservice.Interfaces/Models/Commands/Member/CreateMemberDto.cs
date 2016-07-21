using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Member
{
    public class CreateMemberDto
    {
        public MemberTypeDto MemberType { get; set; }
        public List<MemberDto> Members { get; set; }
    }
}
