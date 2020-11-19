using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class JoinedLiveModel {
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime JoinTime {get; set;}
        public string GroupName {get; set; }
        public dynamic? Data { get; set; }

    }
}