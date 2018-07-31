using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using UPictures.Web.Models;
using UPictures.Data;

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
                    DirectoryName = p.DirectoryName,
                    DateTaken = p.DateTaken
                })
                .FirstOrDefault();

            return View(pictureViewModel);
        }
        
        public ActionResult Next(int id)
        {
            var picture = _context.Pictures.First(p => p.Id == id);
            var  pictures = _context.Pictures
                .Where(p => p.DateTaken.Year == picture.DateTaken.Year && p.DateTaken.Month == picture.DateTaken.Month)
                .OrderBy(p => p.DateTaken.Day);
            var index = pictures.IndexOf(picture);
            var next = pictures[index + 1];
                var returnValue = new PictureViewModel
                {
                    Id = next.Id,
                    FileName = next.FileName,
                    DirectoryName = next.DirectoryName,
                    DateTaken = next.DateTaken
                })
               

            return RedirectToAction("View", new { id = nextPictureId });
        }

        public ActionResult Previous(int id)
        {
            var previousPictureId = id;
            // var picture = _context.Pictures.Include(p => p.Album).FirstOrDefault(p => p.Id == id);
            // if (picture != null)
            // {
            //     var album = _context.Albums.Include(a => a.Pictures).FirstOrDefault(a => a.Id == picture.Album.Id);
            //     if (album != null)
            //     {
            //         var pictures = album.Pictures.OrderBy(p => p.Id).ToList();
            //         var index = pictures.FindIndex(p => p.Id == id);
            //         previousPictureId = pictures[index].Id;

            //         if (index > 0)
            //         {
            //             previousPictureId = pictures[index - 1].Id;
            //         }
            //     }
            // }

            return RedirectToAction("View", new { id = previousPictureId });
        }
    }
}