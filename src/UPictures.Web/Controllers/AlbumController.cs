using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UPictures.Web.Data;
using UPictures.Web.Models;

namespace UPictures.Web.Controllers
{
    public class AlbumController : Controller
    {
        private readonly UPicturesContext _context;

        public AlbumController(UPicturesContext context)
        {
            _context = context;
        }

        public IActionResult View(int id)
        {
            var albumViewModel = _context.Albums
                                    .Where(a => a.Id == id)
                                    .Select(a => new AlbumViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Pictures = a.Pictures
                                .OrderBy(p => p.Name)
                                .Select(p => new PictureViewModel
                                {
                                    Id = p.Id,
                                    FileName = p.FileName
                                })
                                .ToList()
            }).FirstOrDefault();
            
            return View(albumViewModel);
        }
    }
}