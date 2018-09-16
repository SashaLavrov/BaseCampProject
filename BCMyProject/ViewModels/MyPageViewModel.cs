using BCMyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.ViewModels
{
    public class MyPageViewModel
    {
        public IEnumerable<Board>  Boards { get; set; }
        public IEnumerable<Photo> Photos { get; set; }
        public ApplicationUser User { get; set; }
    }
}
