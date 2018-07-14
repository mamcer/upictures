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
            var years = _context.Pictures.Select(p => p.DateTaken.Year).Distinct().OrderBy(p => p).ToList();
            var pictures = new List<YearViewModel>();
            foreach(var year in years)
            {
                var pictureCount = _context.Pictures.Where(p => p.DateTaken.Year == year).Count();
                pictures.Add(new YearViewModel
                {
                    Year = year,
                    PictureCount = pictureCount
                });
            }

            return View(pictures);
        }

        public IActionResult Year(int year)
        {
            ViewData["Year"] = year.ToString();
            var months = _context.Pictures.Where(p => p.DateTaken.Year == year).Select(p => p.DateTaken.Month).OrderBy(p => p).Distinct().ToList();
            var pictures = new List<MonthViewModel>();
            foreach(var month in months)
            {
                var pictureCount = _context.Pictures.Where(p => p.DateTaken.Year == year && p.DateTaken.Month == month).Count();
                pictures.Add(new MonthViewModel
                {
                    Month = month,
                    PictureCount = pictureCount
                });
            }

            return View(pictures);
        }

        public IActionResult Month(int year, int month)
        {
            var  pictures = _context.Pictures
                            .Where(p => p.DateTaken.Year == year && p.DateTaken.Month == month)
                            .Select(p => new PictureViewModel
                            {
                                Id = p.Id,
                                FileName = p.FileName,
                                AlbumName = p.Album.Name,
                                AlbumId = p.Album.Id,
                                DateTaken = p.DateTaken
                            })
                            .OrderBy(p => p.DateTaken.Day)
                            .ToList();

            return View(pictures);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
