using System.Collections.Generic;

namespace UPictures.Web.Models
{
    public class AlbumViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<PictureViewModel> Pictures { get; set; }
    }
}