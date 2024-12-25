using Business.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Business.TeamManagement.Dto
{
    public class ExportTeamInvitationDto
    {

        public string TeamName { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string InvitedUserName { get; set; }

        public Invitation State { get; set; }

        public DateTime? ResponseTime { get; set; }

        public DateTime CreationTime { get; set; }
    }
}