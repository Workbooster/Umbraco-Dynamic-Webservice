using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models;
using Workbooster.UmbracoDynamicWebservice.Interfaces.Models.Commands.Member;

namespace Workbooster.UmbracoDynamicWebservice.Plugin.Controllers
{
    [PluginController("DynamicWebservice")]
    public class MemberController : ControllerBase
    {
        #region PUBLIC METHODS

        [HttpPost]
        public List<MemberDto> Read([FromBody]ReadMemberDto readCommand)
        {
            if (readCommand.MemberType == null)
                throw new Exception("A member type is required.");

            IEnumerable<IMember> listOfMembers;

            if (readCommand.MemberType.Id != default(int))
            {
                // use the numeric Id as the identifier for the member type
                listOfMembers = ApplicationContext
                    .Services
                    .MemberService
                    .GetMembersByMemberType(readCommand.MemberType.Id);
            }
            else if (String.IsNullOrEmpty(readCommand.MemberType.Name) == false)
            {
                // use the alias as the identifier for the member type
                listOfMembers = ApplicationContext
                    .Services
                    .MemberService
                    .GetMembersByMemberType(readCommand.MemberType.Alias);
            }
            else
            {
                throw new Exception("A member type is required.");
            }

            List<MemberDto> listOfDto = new List<MemberDto>();

            foreach (var node in listOfMembers)
            {
                MemberDto member = GetMemberDto(node, readCommand.MemberType);

                listOfDto.Add(member);
            }

            return listOfDto;
        }

        [HttpPost]
        public List<MemberDto> Create([FromBody]CreateMemberDto createCommand)
        {
            List<MemberDto> listOfCreatedMembers = new List<MemberDto>();

            foreach (var dto in createCommand.Members)
            {
                IMember member = ApplicationContext.Services.MemberService
                    .CreateMember(dto.Username, dto.Email, dto.Name, createCommand.MemberType.Alias);
                
                for (int i = 0; i < createCommand.MemberType.Properties.Count; i++)
                {
                    PropertyDto property = createCommand.MemberType.Properties[i];
                    member.SetValue(property.Alias, dto.PropertyValues[i]);
                }

                ApplicationContext.Services.MemberService.Save(member);
                ApplicationContext.Services.MemberService.SavePassword(member, dto.Password);

                MemberDto createdMember = GetMemberDto(member, createCommand.MemberType);

                listOfCreatedMembers.Add(createdMember);
            }

            return listOfCreatedMembers;
        }

        #endregion

        #region INTERNAL METHODS

        protected MemberDto GetMemberDto(IMember node, MemberTypeDto memberType)
        {
            MemberDto content = new MemberDto()
            {
                Id = node.Id,
                Key = node.Key.ToString(),
                Name = node.Name,
                Username = node.Username,
                Email = node.Email,
                Password = null,
                PropertyValues = new object[memberType.Properties.Count],
            };

            for (int i = 0; i < memberType.Properties.Count; i++)
            {
                PropertyDto property = memberType.Properties[i];
                content.PropertyValues[i] = node.GetValue(property.Alias);
            }

            return content;
        }

        #endregion
    }
}
