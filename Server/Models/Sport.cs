using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class Sport {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public string ImageName { get; set; }
        [NotMapped]
        public string ImageSrc { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        
    }
}