using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class NotificationModelDto {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Message { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        public bool IsRead { get; set; }
        [Required]
        public bool IsRecurring { get; set; }

        [Required]
        public virtual int UserId { get; set; }
        public virtual int? TeamId { get; set; }
        public virtual int? PlayerId {get; set; }
        
        public dynamic? Data { get; set; }

    }
}