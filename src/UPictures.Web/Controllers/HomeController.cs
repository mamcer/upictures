using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPictures.Web.Data;
using UPictures.Web.Models;

namespace UPictures.Web.Controllers
{
    public class HomeController : Controller
    {
        private UPicturesContext _context;

        public HomeController(UPicturesContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            var albums = _context.Albums
                            .Include(x => x.Pictures)
                            .OrderBy(x => x.Name);
                            
            var model = new List<IndexViewModel>();
            foreach (var album in albums)
            {
                var indexViewModel = new IndexViewModel
                {
                    Id = album.Id,
                    Name = album.Name,
                    PictureCount = album.Pictures.Count
                };
                
                model.Add(indexViewModel);
            }

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
