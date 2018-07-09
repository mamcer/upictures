using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPictures.Web.Data;
using UPictures.Web.Models;

namespace UPictures.Web.Controllers
{
    public class PictureController : Controller
    {
        private readonly UPicturesContext _context;

        public PictureController(UPicturesContext context)
        {
            _context = context;
        }

        public IActionResult View(int id)
        {
            var pictureViewModel = _context.Pictures
                .Where(p => p.Id == id)
                .Select(p => new PictureViewModel
                {
                    Id = p.Id,
                    FileName = p.FileName,
                    AlbumId = p.Album.Id,
                    AlbumName = p.Album.Name,
                    DateTaken = p.DateTaken
                })
                .FirstOrDefault();

            return View(pictureViewModel);
        }
        
        public ActionResult Next(int id)
        {
            var nextPictureId = id;
            var picture = _context.Pictures.Include(p => p.Album).FirstOrDefault(p => p.Id == id);
            if (picture != null)
            {
                var album = _context.Albums.Include(a => a.Pictures).FirstOrDefault(a => a.Id == picture.Album.Id);
                if (album != null)
                {
                    var pictures = album.Pictures.OrderBy(p => p.Id).ToList();
                    var index = pictures.FindIndex(p => p.Id == id);
                    nextPictureId = pictures[index].Id;

                    if (pictures.Count > index + 1)
                    {
                        nextPictureId = pictures[index + 1].Id;
                    }
                }
            }

            return RedirectToAction("View", new { id = nextPictureId });
        }

        public ActionResult Previous(int id)
        {
            var previousPictureId = id;
            var picture = _context.Pictures.Include(p => p.Album).FirstOrDefault(p => p.Id == id);
            if (picture != null)
            {
                var album = _context.Albums.Include(a => a.Pictures).FirstOrDefault(a => a.Id == picture.Album.Id);
                if (album != null)
                {
                    var pictures = album.Pictures.OrderBy(p => p.Id).ToList();
                    var index = pictures.FindIndex(p => p.Id == id);
                    previousPictureId = pictures[index].Id;

                    if (index > 0)
                    {
                        previousPictureId = pictures[index - 1].Id;
                    }
                }
            }

            return RedirectToAction("View", new { id = previousPictureId });
        }
    }
}