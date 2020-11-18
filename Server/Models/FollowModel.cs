using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class Follow {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual int UserId{ get; set; }   
        
        [ForeignKey("UserId")]     
        public virtual Users Users{ get; set; }

        [Required]
        public virtual int TeamId{ get; set; }   
        
        [ForeignKey("TeamId")]     
        public virtual Team Teams{ get; set; }

        
    }
}