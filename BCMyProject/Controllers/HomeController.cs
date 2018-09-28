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
using BCMyProject.Exstension;

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

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await GetCurrentUserAsync();

            IEnumerable<Photo> photo = _db.Photos
                .OrderByDescending(x => x.Date);

            IEnumerable<Board> Board = _db.Boards
                .Where(b => b.UserId == user.Id)
                .Include(b => b.PhotoBoard)
                .ThenInclude(b => b.Photo);

            MainViewModel model = new MainViewModel
            {
                Photo = photo,
                Board = Board,
            };

            return View(model);
        }

        public async Task<IActionResult> ChangePhoto(int photoId)
        {
            ApplicationUser user = await GetCurrentUserAsync();

            Photo photo = await _db.Photos
                .Where(x => x.PhotoId == photoId && x.UserId == user.Id)
                .FirstOrDefaultAsync();
            ChangePhotoViewModel model = new ChangePhotoViewModel
            {
                Photo = photo
            };
             return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePhoto(int id, string description, string topic)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            var ph = await _db.Photos.Where(x => x.PhotoId == id && x.UserId == user.Id).FirstOrDefaultAsync();
            if(ph != null)
            {
                ph.Topic = topic;
                ph.Description = description;
            }
            _db.SaveChanges();
            return RedirectToAction("MyPage");
        }

        [HttpPost]
        public async Task<IActionResult> SearchPhoto(string target)
        {
            ApplicationUser user = await GetCurrentUserAsync();

            IEnumerable<Photo> photoT = _db.Photos
                .Where(x => x.Topic.Contains(target))
                .OrderByDescending(x => x.Date);

            IEnumerable<Photo> photoD = _db.Photos
                .Where(x => x.Description.Contains(target))
                .OrderByDescending(x => x.Date);

            IEnumerable<Photo> res = photoT
                .Union(photoD);

            IEnumerable<Board> Board = _db.Boards
                .Where(b => b.UserId == user.Id)
                .Include(b => b.PhotoBoard)
                .ThenInclude(b => b.Photo);
            MainViewModel model = new MainViewModel
            {
                Photo = res,
                Board = Board
            };
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string topic, string description)
        {
            if (topic == null)
            {
                topic = "Без темы";
            }
            ApplicationUser User = await GetCurrentUserAsync();
            Photo photo = new Photo
            {
                Path = "/Files/",
                PhotoName = file.FileName,
                Topic = topic,
                Date = DateTime.Now,
                User = User,
                Description = description,
            };
            _db.Photos.Add(photo);
            _db.SaveChanges();

            var newName = photo.PhotoId + photo.PhotoName;
            photo.PhotoName = newName;
            photo.Path += newName;
            _db.SaveChanges();

            string path = "/Files/" + newName;
            if (file.Length > 0)
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            return Ok();
        }

        public async Task<IActionResult> ShowThisPhoto(int photoId)
        {
            ApplicationUser user = await GetCurrentUserAsync();

            Photo ph = _db.Photos.Where(x => x.PhotoId == photoId).Include(x => x.User).FirstOrDefault();

            List<Coment> coments = _db.Coments
                .Include(u => u.User)
                .Where(x => x.Photo.PhotoId == photoId)
                .ToList();
            int ratingVal = _db.Likes
                .Where(l => l.PhotoId == photoId)
                .Count();
            bool isLike = _db.Likes
                .Where(x => x.PhotoId == photoId && x.UserId == user.Id)
                .IsNullOrEmpty();
            IEnumerable<Board> Board = _db.Boards
                .Where(b => b.UserId == user.Id)
                .Include(b => b.PhotoBoard)
                .ThenInclude(b => b.Photo);
            ShowThisPhotoViewModel model = new ShowThisPhotoViewModel
            {
                Coments = coments,
                IsCurrentUserLike = !isLike,
                Photo = ph,
                Rating = ratingVal,
                Board = Board,

            };
            return View(model);
        }

        public IActionResult GetFile(Photo photo)
        {
            string file_path = Path.Combine(_appEnvironment.ContentRootPath, $"wwwroot{photo.Path}");
            string MyString = Path.GetExtension(photo.Path);
            string NewString = MyString.TrimStart('.');
            string file_type = $"application/{NewString}";
            PhysicalFileResult res = PhysicalFile(file_path, file_type,photo.PhotoId + "." + NewString);
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
            bool isLike = _db.Likes
               .Where(x => x.PhotoId == PhotoId && x.UserId == user.Id)
               .IsNullOrEmpty();

            if (isLike)
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
                Like like = _db.Likes
                    .Where(l => l.UserId == user.Id && l.PhotoId == PhotoId)
                    .First();
                _db.Likes.Remove(like);
                _db.SaveChanges();
                return false;
            }
        }

        public async Task<IActionResult> MyPage()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            IEnumerable<Photo> photos = _db.Photos
                .Where(p => p.UserId == user.Id);

            IEnumerable<Board> boards = _db.Boards
                .Where(b => b.UserId == user.Id)
                .Include(b => b.PhotoBoard)
                .ThenInclude(b => b.Photo);
            
            MyPageViewModel model = new MyPageViewModel
            {
                Boards = boards,
                Photos = photos,
                User = user
            };

            return View(model);
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
            IEnumerable<Photo> ph =  _db.Photos.Where(p => p.PhotoId == photo);
            if (!ph.IsNullOrEmpty())
            {
                Photo Photo = ph.First();
                ApplicationUser user = await GetCurrentUserAsync();
                if (System.IO.File.Exists($"wwwroot/{Photo.Path}"))
                {
                    if (Photo.User == user)
                    {
                        IEnumerable<PhotoBoard> pb = _db.PhotoBoards.Where(p => p.PhotoId == photo);
                        if (!pb.IsNullOrEmpty())
                        {
                            _db.PhotoBoards.RemoveRange(pb);
                        }
                        _db.Photos.Remove(Photo);
                        System.IO.File.Delete($"wwwroot/{Photo.Path}");
                        _db.SaveChanges();
                        return Ok();
                    }
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(string boardName)
        {
            if (boardName != null)
            {
                ApplicationUser user = await GetCurrentUserAsync();
                Board Board = new Board
                {
                    BoardName = boardName,
                    UserId = user.Id
                };
                _db.Boards.Add(Board);
                _db.SaveChanges();
                var responce = Board.BoardId;
                return Json(responce);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotInBoard(int boardId, int photoId)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            bool res = _db.PhotoBoards
                .Where(x => x.BoardId == boardId && x.PhotoId == photoId)
                .IsNullOrEmpty();
            if (res)
            {
                _db.PhotoBoards.Add(new PhotoBoard
                {
                    PhotoId = photoId,
                    BoardId = boardId
                });
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> ShowAllPhotoInBoard(int boardId)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            IEnumerable<Photo> photo = _db.PhotoBoards
                .Where(p => p.BoardId == boardId && p.Board.UserId == user.Id)
                .Select(x => x.Photo);

            string boardName = _db.Boards.Where(b => b.BoardId == boardId)
                .First()
                .BoardName;

            ShowAllPhotoInBoardViewModal modal = new ShowAllPhotoInBoardViewModal
            {
                Photo = photo,
                BoardName = boardName,
                BoardId = boardId,
            };
            return View(modal);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBoard(int boardId)
        {
            ApplicationUser user = await GetCurrentUserAsync();

            IEnumerable<Board> board = _db.Boards
                .Where(x => x.BoardId == boardId);

            if (!board.IsNullOrEmpty())
            {
                IEnumerable<PhotoBoard> pb = _db.PhotoBoards
                    .Where(x => x.BoardId == boardId);

                if (!pb.IsNullOrEmpty())
                {
                    _db.PhotoBoards.RemoveRange(pb);
                    _db.SaveChanges();
                }
                _db.Boards.RemoveRange(board);
                _db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        public async Task<IActionResult> RemovePhotoFromBoard(int boardId, int photoId)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            var board = _db.Boards
                .Where(x => x.BoardId == boardId && x.UserId == user.Id);
            if (!board.IsNullOrEmpty())
            {
                var pb = _db.PhotoBoards.Where(x => x.BoardId == board
                    .First()
                    .BoardId && x.PhotoId == photoId);

                if (!pb.IsNullOrEmpty())
                {
                    _db.PhotoBoards.RemoveRange(pb);
                    _db.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
