using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class Competition {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Name { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Season { get; set; }
        [Required]
        public bool Current { get; set; }
        public string ImageName { get; set; }
        public string CoverImage { get; set; }
        [NotMapped]
        public string ImageSrc { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        [NotMapped]
        public string CoverImageSrc { get; set; }
        [NotMapped]
        public IFormFile CoverImageFile { get; set; }
        [Required]
        public virtual int SportId { get; set; }   
        [ForeignKey("SportId")]     
        public virtual Sport Sports{ get; set; }
    }
}