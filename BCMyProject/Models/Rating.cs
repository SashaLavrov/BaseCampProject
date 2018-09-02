using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCMyProject.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        public int Value { get; set; }

        public List<UserRating> UserRating { get; set; }
        public List<Photo> Photo { get; set; }

        public Rating()
        {
            UserRating = new List<UserRating>();
            Photo = new List<Photo>();
        }
    }
}
