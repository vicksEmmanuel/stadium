using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class GoLiveDto {
        public string UserTeamGroup {get; set;}
        public string AdminTeamGroup { get; set; }
        public string UserChatTeamGroup { get; set;}
    }
}