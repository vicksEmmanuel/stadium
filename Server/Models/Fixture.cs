using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class Fixture {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Label { get; set; }
        [Required]
        public DateTime EventTime { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }

        [Required]
        public virtual int Team1Id { get; set; }   
        [ForeignKey("Team1Id")]     
        public virtual Team Team1 { get; set; }

        [Required]
        public virtual int Team2Id { get; set; }   
        [ForeignKey("Team2Id")]     
        public virtual Team Team2{ get; set; }

        [Required]
        public virtual int CompetitionId { get; set; }
        [ForeignKey("CompetitionId")]     
        public virtual Competition Competition{ get; set; }


        [NotMapped]
        public dynamic? Data { get; set; }
        
    }
}