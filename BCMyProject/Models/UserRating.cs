using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.Models
{
    public class UserRating
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int RatingId { get; set; }
        public Rating Rating { get; set; }
    }
}
