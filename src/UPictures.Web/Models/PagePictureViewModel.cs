using System.Collections.Generic;

namespace UPictures.Web.Models
{
    public class PagePictureViewModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }

        public IEnumerable<PictureViewModel> Results { get; set; }
    }
}