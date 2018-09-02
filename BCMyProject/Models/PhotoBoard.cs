using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.Models
{
    public class PhotoBoard
    {
        public Board Board { get; set; }
        public int BoardId { get; set; }

        public Photo Photo { get; set; }
        public int PhotoId { get; set; }
    }
}
