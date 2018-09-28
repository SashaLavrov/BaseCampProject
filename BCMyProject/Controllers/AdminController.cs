using BCMyProject.Data;
using BCMyProject.Exstension;
using BCMyProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMyProject.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        readonly ApplicationDbContext _db;
        readonly IHostingEnvironment _appEnvironment;
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(ApplicationDbContext db, IHostingEnvironment appEnvironment,
            UserManager<ApplicationUser> userManager, IHostingEnvironment env, SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            IEnumerable<ApplicationUser> users = _db.Users.Where(x => x != user);

            return View(users);
        }

        public async Task<IActionResult> DropUser(string id)
        {
            var user = _db.Users.Where(x => x.Id == id).FirstOrDefault();



            IEnumerable<Photo> photo = _db.Photos.Where(x => x.User == user);
            foreach (var ph in photo)
            {
                if (System.IO.File.Exists($"wwwroot/{ph.Path}"))
                {
                    IEnumerable<PhotoBoard> pb = _db.PhotoBoards.Where(p => p.PhotoId == ph.PhotoId);
                    if (!pb.IsNullOrEmpty())
                    {
                        _db.PhotoBoards.RemoveRange(pb);
                    }
                    _db.Photos.Remove(ph);
                    System.IO.File.Delete($"wwwroot/{ph.Path}");
                }
            }

            IEnumerable<Board> board = _db.Boards
               .Where(x => x.User == user);
            foreach (var i in board)
            {
                if (!board.IsNullOrEmpty())
                {
                    IEnumerable<PhotoBoard> pb = _db.PhotoBoards
                        .Where(x => x.BoardId == i.BoardId);

                    if (!pb.IsNullOrEmpty())
                    {
                        _db.PhotoBoards.RemoveRange(pb);
                        _db.SaveChanges();
                    }
                    _db.Boards.RemoveRange(board);
                }
            }

            _db.Coments.RemoveRange(_db.Coments.Where(x => x.User == user));

            _db.SaveChanges();
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}