using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class GoLiveHubModel {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string ConnectionId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserEmail { get; set; }
        
        [Required]
        public bool isOpen { get; set; }

        [Required]
        public virtual int UserId { get; set; }   
        [ForeignKey("UserId")]     
        public virtual Users Users{ get; set; }
    }
}