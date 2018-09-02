using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.Models
{
    public class Board
    {
        [Key]
        public int BoardId { get; set; }

        [Required]
        [MaxLength(500)]
        public string BoardName { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<PhotoBoard> PhotoBoard { get; set; }

        public Board()
        {
            PhotoBoard = new List<PhotoBoard>();
        }
    }
}
