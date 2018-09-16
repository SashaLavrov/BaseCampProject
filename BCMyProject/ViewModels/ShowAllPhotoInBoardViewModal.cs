using BCMyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.ViewModels
{
    public class ShowAllPhotoInBoardViewModal
    {
        public IEnumerable<Photo> Photo { get; set; }
        public string BoardName { get; set; }
        public int BoardId { get; set; }
    }
}
