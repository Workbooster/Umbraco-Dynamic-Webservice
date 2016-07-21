using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbooster.UmbracoDynamicWebservice.Interfaces.Models
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public object[] PropertyValues { get; set; }
    }
}
