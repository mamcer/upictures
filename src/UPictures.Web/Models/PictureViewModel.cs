using System;

namespace UPictures.Web.Models
{
    public class PictureViewModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string DirectoryName { get; set; }

        public DateTime DateTaken { get; set; }
    }
}