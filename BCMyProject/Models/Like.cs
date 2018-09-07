using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public int PhotoId { get; set; }
        public string UserId { get; set; }

        public Photo Photo { get; set; }
    }
}
