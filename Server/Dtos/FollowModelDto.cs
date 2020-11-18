using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Dtos
{
    public class FollowModelDto {
        public int[] TeamIds { get; set; }
    }
}