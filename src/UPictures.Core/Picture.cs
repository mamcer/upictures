using System;

namespace UPictures.Core
{
    public class Picture
    {
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

        public string Hash { get; set; }

        public string DirectoryName { get; set; }        
    }
}