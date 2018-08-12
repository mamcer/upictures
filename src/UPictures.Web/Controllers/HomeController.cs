using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UPictures.Web.Models;
using UPictures.Data;

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
                var pictureCount = _context.Pictures.Count(p => p.DateTaken.Year == year);
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
                var pictureCount = _context.Pictures.Count(p => p.DateTaken.Year == year && p.DateTaken.Month == month);
                pictures.Add(new MonthViewModel
                {
                    Month = month,
                    PictureCount = pictureCount
                });
            }

            return View(pictures);
        }

        public IActionResult Month(int year, int month, int pageIndex, int pageSize)
        {
            var  pictures = _context.Pictures
                            .Where(p => p.DateTaken.Year == year && p.DateTaken.Month == month);

            var picturesReturn = pictures                                                        
                                .Select(p => new PictureViewModel
                                {
                                    Id = p.Id,
                                    FileName = p.FileName,
                                    DirectoryName = p.DirectoryName,
                                    DateTaken = p.DateTaken
                                })
                                .OrderBy(p => p.DateTaken.Day)
                                .Skip((pageIndex - 1)*pageSize)
                                .Take(pageSize)
                                .ToList();

            return View(new PagePictureViewModel
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Total = pictures.Count(),
                Results = picturesReturn
            });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
