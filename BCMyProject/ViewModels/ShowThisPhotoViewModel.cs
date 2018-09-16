using BCMyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.ViewModels
{
    public class ShowThisPhotoViewModel
    {
        public Photo Photo { get; set; }
        public int Rating { get; set; }
        public List<Coment> Coments { get; set; }
        public bool IsCurrentUserLike { get; set; }
        public IEnumerable<Board> Board { get; set; }

        public ShowThisPhotoViewModel()
        {
            Coments = new List<Coment>();
        }
    }
}
