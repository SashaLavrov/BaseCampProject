using BCMyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.ViewModels
{
    public class MainViewModel
    {
        public IEnumerable<Photo> Photo { get; set; }
        public IEnumerable<Board> Board { get; set; }
    }
}
