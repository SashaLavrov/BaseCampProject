using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCMyProject.Models;
using BCMyProject.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BCMyProject.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace BCMyProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        readonly ApplicationDbContext _db;
        readonly IHostingEnvironment _appEnvironment;
        readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext db, IHostingEnvironment appEnvironment,
            UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _db = db;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public IActionResult Index()
        {
            ViewBag.Topic = _db.Photos.Select(x => x.Topic).Distinct();
            ViewBag.Some = _db.Photos.GroupBy(x => x.Topic).Select(grp => grp.First());
            return View();
        }

        public ViewResult ShowAllPhotoInTopic(string topic)
        {
            ViewBag.TopicName = topic;
            var res = _db.Photos.Where(x => x.Topic == topic);
            return View(res);
        }

        public async Task<ViewResult> AddFile()
        {
            var user = await GetCurrentUserAsync();
            string mail = user?.Email;
            ViewBag.UserName = mail;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string topic)
        {
            if (topic == null)
            {
                topic = "Some";
            }
            ApplicationUser User = await GetCurrentUserAsync();
            string path = "/Files/" + file.FileName;
            if (file.Length > 0)
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }   
            Photo photo = new Photo
            {
                PhotoName = file.FileName,
                Path = path,
                Topic = topic,
                Date = DateTime.Now,
                User = User,
            };
            _db.Photos.Add(photo);
            _db.SaveChanges();
            return Ok();
        }


        public async Task<IActionResult> ShowThisPhoto(Photo photo)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            bool isLike = false;
            //ShowThisPhotoViewModel stpvm = new ShowThisPhotoViewModel();
            List<Coment> coments = _db.Coments.Include(u => u.User).Where(x => x.Photo == photo).ToList();
            int ratingVal = _db.Likes.Where(l => l.PhotoId == photo.PhotoId).Count();
            isLike = _db.Likes.Select(l => l.UserId).Contains(user.Id);
            ShowThisPhotoViewModel model = new ShowThisPhotoViewModel
            {
                Coments = coments,
                IsCurrentUserLike = isLike,
                Photo = photo,
                Rating = ratingVal
            };
            return View(model);
        }

        public IActionResult GetFile(Photo photo)
        {
            // Путь к файлу
            string file_path = Path.Combine(_appEnvironment.ContentRootPath, $"wwwroot{photo.Path}");

            //
            string MyString = Path.GetExtension(photo.Path);
            char[] MyChar = { '.' };
            string NewString = MyString.TrimStart(MyChar);
            // Тип файла - content-type
            string file_type = $"application/{NewString}";
            // Имя файла - необязательно
            PhysicalFileResult res = PhysicalFile(file_path, file_type, photo.PhotoName);
            //
            res.FileDownloadName = photo.PhotoName;
            //
            return res;
        }

        [HttpPost]
        public async Task<IActionResult> AddComent(ComentViewModel cvm)
        {
            int p = Convert.ToInt32(cvm.PhotoId);
            ApplicationUser user = await GetCurrentUserAsync();
            Photo photo = _db.Photos.Where(x => x.PhotoId == p).First();
            if (cvm.Text != null)
            {
                await _db.Coments.AddAsync(new Coment
                {
                    Text = cvm.Text,
                    User = user,
                    Date = DateTime.Now,
                    Photo = photo
                });
                _db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<bool> Like(int PhotoId)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            bool isLike = _db.Likes.Select(l => l.PhotoId ).Contains(PhotoId);

            if (!isLike)
            {
                _db.Likes.Add(new Like {
                    PhotoId = PhotoId,
                    UserId = user.Id
                });
                _db.SaveChanges();
                return true;
            }
            else 
            {
                Like like = _db.Likes.Where(l => l.UserId == user.Id && l.PhotoId == PhotoId).First();
                _db.Likes.Remove(like);
                _db.SaveChanges();
                return false;
            }
        }

        public async Task<IActionResult> MyPage()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            //ViewBag.Boards = _db.Boards.Where(x => x.User == user).Include(b => b.PhotoBoard).ThenInclude(b => b.Photo).ToList();
            ViewBag.Boards = _db.Boards.GroupBy(x => x.BoardName)
                .Select(grp => grp.First())
                .Include(b => b.PhotoBoard)
                .ThenInclude(b => b.Photo);
            IEnumerable<Photo> photos = _db.Photos.Where(x => x.User == user)
                .ToList();
            ViewBag.User = user;
            return View(photos);
        }

        [HttpPost]
        public async Task<IActionResult> Avatar(IFormFile Avatar)
        {
            if (Avatar != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(Avatar.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)Avatar.Length);
                }
                // установка массива байтов
                ApplicationUser User = await GetCurrentUserAsync();
                User.Avatar = imageData;
            }
            _db.SaveChanges();

            return RedirectToAction("MyPage");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeNick(string Nick)
        {
            ApplicationUser User = await GetCurrentUserAsync();
            if (Nick != null)
            {
                User.Nick = Nick;
                _db.SaveChanges();
            }
            return RedirectToAction("MyPage");
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveFile(int photo)
        {
            Photo Photo = _db.Photos.Where(p => p.PhotoId == photo).First();
            ApplicationUser user = await GetCurrentUserAsync();
            if ((System.IO.File.Exists($"wwwroot/{Photo.Path}")))
            {
                if (Photo.User == user)
                {
                    _db.Photos.Remove(Photo);
                    System.IO.File.Delete($"wwwroot/{Photo.Path}");
                    _db.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
