using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.Models
{
    public class Coment
    {
        [Key]
        public int ComentId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
