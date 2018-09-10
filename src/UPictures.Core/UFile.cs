using System;

namespace UPictures.Core
{
    public class UFile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public long Size { get; set; }

        public DateTime CreationDate { get; set; }

        public string Hash { get; set; }

        public string Path { get; set; }
    }
}
