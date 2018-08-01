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
            var picturesArray = pictures.ToArray();
            
            var nextPictureId = id;
            if(picturesArray.Length > index + 1)
            {
                var next = picturesArray[index + 1];
                nextPictureId = next.Id;
            }    

            return RedirectToAction("View", new { id = nextPictureId });
        }

        public ActionResult Previous(int id)
        {
            var picture = _context.Pictures.First(p => p.Id == id);
            var  pictures = _context.Pictures
                .Where(p => p.DateTaken.Year == picture.DateTaken.Year && p.DateTaken.Month == picture.DateTaken.Month)
                .OrderBy(p => p.DateTaken.Day);
            var index = pictures.IndexOf(picture);
            var nextPictureId = id;
            if(index > 0)
            {
                var picturesArray = pictures.ToArray();
                var next = picturesArray[index - 1];
                nextPictureId = next.Id;
            }    

            return RedirectToAction("View", new { id = nextPictureId });
        }
    }
}