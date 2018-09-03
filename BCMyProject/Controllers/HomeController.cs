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
            UserManager<ApplicationUser> userManager)
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
        public async Task<IActionResult> AddFile(IFormFile uploadedFile, string Topic)
        {
            ApplicationUser User = await GetCurrentUserAsync();
            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                Photo file = new Photo
                {
                    PhotoName = uploadedFile.FileName,
                    Path = path,
                    Topic = Topic,
                    Date = DateTime.Now,
                    User = User,
                    Rating = new Rating()
                };
                _db.Photos.Add(file);
                _db.SaveChanges();
            }

            return Redirect("~/Home/Index");
        }

        public async Task<IActionResult> ShowThisPhoto(Photo photo)
        {
            ViewBag.Photo = _db.Photos.Include(r => r.Rating).Where(p => p == photo).First();
            ViewBag.Coment = _db.Coments.Include(u => u.User).Where(x => x.Photo == photo).ToList();
            ApplicationUser user = await GetCurrentUserAsync();
            bool  isLike = false;
            IEnumerable<UserRating> res = _db.UserRatings.Where(x => x.UserId == user.Id && x.RatingId == photo.RatingId);
            if (res.Count() != 0)
            {
                isLike = res.First().Like;
            }
            ViewBag.isLike = isLike;
            return View();
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
        public async Task<bool> Like(ComentViewModel cvm)
        {
            int p = Convert.ToInt32(cvm.PhotoId);
            ApplicationUser user = await GetCurrentUserAsync();
            Photo photo = _db.Photos.Where(x => x.PhotoId == p).First();
            Rating responce = _db.Ratings.Where(e => e.RatingId == photo.RatingId).First();
            IEnumerable<UserRating> result = _db.UserRatings.Where(x => x.UserId == user.Id && x.RatingId == photo.RatingId);
            UserRating res = null;
            if (result.Count() != 0)
            {
                res = _db.UserRatings.Where(x => x.UserId == user.Id && x.RatingId == responce.RatingId).First();
            }
            
            if (res == null)
            {
                _db.UserRatings.Add(new UserRating {
                    UserId = user.Id,
                    RatingId = responce.RatingId,
                    Like = true,
                });
                responce.Value++;
                _db.SaveChanges();
                return true;
            }else if (res != null)
            {
                if (!res.Like)
                {
                    res.Like = true;
                    responce.Value++;
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    res.Like = false;
                    responce.Value--;
                    _db.SaveChanges();
                    return false;
                }
            }
            return false;
        }

        public async Task<ViewResult> MyPage()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            ViewBag.Boards = _db.Boards.Where(x => x.User == user).ToList();
            ViewBag.Photos = _db.Photos.Where(x => x.User == user).ToList();
            return View(user);
        }

        public ViewResult Open(Photo p)
        {
            ViewBag.Var = p;
            return View(p);
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
