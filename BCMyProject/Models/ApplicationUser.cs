using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BCMyProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public byte[] Avatar { get; set; }
        public string Nick { get; set; }

        public List<Coment> Coment { get; set; }
        public List<Board> Board { get; set; }
        public List<Photo> Photos { get; set; }

        public ApplicationUser()
        {
            Coment = new List<Coment>();
            Board = new List<Board>();
            Photos = new List<Photo>();
        }
    }
}
