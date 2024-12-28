using System;
using Business.Core.Enums;

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