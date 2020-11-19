using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class NotificationModel {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Message { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Type { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public bool IsRead { get; set; }
        [Required]
        public bool IsRecurring { get; set; }

        [Required]
        public virtual int UserId { get; set; }   
        [ForeignKey("UserId")]     
        public virtual Users Users{ get; set; }

        public virtual int? TeamId { get; set; }
        [ForeignKey("TeamId")]     
        public virtual Team Team{ get; set; }

        public virtual int? PlayerId {get; set; }
        [ForeignKey("PlayerId")]     
        public virtual Players Player{ get; set; }

        [NotMapped]
        public dynamic? Data { get; set; }
        
    }
}