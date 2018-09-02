using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.Models
{
    public class Photo
    {
        [Key]
        public int PhotoId { get; set; }
        [Required]
        [MaxLength(200)]
        public string PhotoName { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Topic { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public List<PhotoBoard> PhotoBoard { get; set; }

        public List<Coment> Coment { get; set; }

        public int RatingId { get; set; }
        public Rating Rating { get; set; }

        public Photo()
        {
            PhotoBoard = new List<PhotoBoard>();
            Coment = new List<Coment>();
        }
    }
}
