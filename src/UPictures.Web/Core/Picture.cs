using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace UPictures.Web.Core
{
    [Table("Picture")]
    public class Picture
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Extension { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public DateTime DateTaken { get; set; }

        public string CameraMaker { get; set; }

        public string CameraModel { get; set; }

        public double FileSize { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string FileName => $"{Name}{Extension}";

        public virtual Album Album { get; set; }
    }
}