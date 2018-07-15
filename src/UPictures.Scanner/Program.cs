using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using UPictures.Data;
using UPictures.Core;
using System.Security.Cryptography;
using System.Text;

namespace UPictures.Scanner
{
    class Program
    {
        public static string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2").ToLower());

            return sb.ToString();
        }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();
            var options =
                new DbContextOptionsBuilder<UPicturesContext>().UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            var upicturesContext = new UPicturesContext(options.Options);

            var startingTime = DateTime.Now;
            Console.WriteLine($"scan started {startingTime.Hour}:{startingTime.Minute}:{startingTime.Second}");
            var rootDirectory = configuration["RootDirectory"];
            var picturesDirectory = configuration["PicturesDirectory"];
            var fileCount = 0;

            var validFileExtensions = new List<string> {".jpg", ".jpeg"};
            var directories = new List<string> {rootDirectory};
            var unitOfWork = new EntityFrameworkUnitOfWork(upicturesContext);
            var pictureRepository = new PictureRepository(upicturesContext);
            var initialDirectory = Directory.GetDirectories(rootDirectory).OrderByDescending(x => x).FirstOrDefault();
            int initialDirectoryNumber = string.IsNullOrEmpty(initialDirectory) ? 0 : Convert.ToInt32(initialDirectory);
            for (int i = 0; i < directories.Count; i++)
            {
                var directoryName = $"{i + initialDirectoryNumber:00000}";
                var viewDirectory = Path.Combine(picturesDirectory, "view", directoryName);
                var masterDirectory = Path.Combine(picturesDirectory, "master", directoryName);
                var thumbnailDirectory = Path.Combine(picturesDirectory, "thumbnail", directoryName);
                Directory.CreateDirectory(viewDirectory);
                Directory.CreateDirectory(masterDirectory);
                Directory.CreateDirectory(thumbnailDirectory);

                foreach (var filePath in Directory.GetFiles(directories[i]))
                {
                    var fileExtension = Path.GetExtension(filePath).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(filePath);

                    if (fileName.StartsWith("."))
                    {
                        Console.WriteLine($"        file name starts with dot:{filePath}");
                        continue;
                    }

                    if (!validFileExtensions.Contains(fileExtension))
                    {
                        Console.WriteLine($"        unknown file extension:{filePath}");
                        continue;
                    }

                    // compute hash
                    string hash;
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        using (HashAlgorithm hashAlgorithm = MD5.Create())
                        {
                            byte[] hashByte = hashAlgorithm.ComputeHash(fs);
                            hash = ToHexString(hashByte);
                        }
                    }

                    // check if file already exists in the database
                    if (pictureRepository.ContainsHash(hash))
                    {
                        Console.WriteLine($"        file exists:{filePath}");
                        continue;
                    }

                    Console.WriteLine($"        processing file:{filePath}");

                    // copy master copy
                    var destFile = Path.Combine(masterDirectory, fileName + fileExtension);
                    File.Copy(filePath, destFile, true);

                    // create view copy
                    destFile = Path.Combine(viewDirectory, fileName + fileExtension);
                    using (Image<Rgba32> image = Image.Load(filePath))
                    {
                        image.Mutate(x => x
                            .Resize(1024, 0));
                        image.Save(destFile);
                    }

                    // create thumbnail copy
                    destFile = Path.Combine(thumbnailDirectory, fileName + fileExtension);
                    using (Image<Rgba32> image = Image.Load(filePath))
                    {
                        image.Mutate(x => x
                            .Resize(200, 0));
                        image.Save(destFile);
                    }

                    // read file metadata
                    DateTime datePictureTaken = DateTime.Today;
                    string cameraMaker = string.Empty, cameraModel = string.Empty;
                    uint height = 0, width = 0;
                    double fileSize;
                    try
                    {
                        using (ExifReader reader = new ExifReader(filePath))
                        {
                            reader.GetTagValue(ExifTags.DateTimeDigitized, out datePictureTaken);
                            reader.GetTagValue(ExifTags.Make, out cameraMaker);
                            reader.GetTagValue(ExifTags.Model, out cameraModel);
                            reader.GetTagValue(ExifTags.PixelXDimension, out width);
                            reader.GetTagValue(ExifTags.PixelYDimension, out height);
                        }
                    }
                    catch
                    {
                        Console.WriteLine(
                            $"        unable to read exif information:{filePath}");
                    }

                    using (var file = File.OpenRead(filePath))
                    {
                        fileSize = file.Length;
                    }

                    var picture = new Picture
                    {
                        Name = Path.GetFileNameWithoutExtension(fileName),
                        Path = filePath,
                        DateTaken = datePictureTaken,
                        CameraMaker = cameraMaker,
                        CameraModel = cameraModel,
                        Extension = fileExtension,
                        Height = (int) height,
                        Width = (int) width,
                        FileSize = fileSize,
                        Hash = hash,
                        DirectoryName = directoryName
                    };

                    fileCount += 1;
                    pictureRepository.Create(picture);
                }

                unitOfWork.SaveChanges();
                directories.AddRange(Directory.GetDirectories(directories[i]));
            }
            
            var timespan = DateTime.Now - startingTime;
            Console.WriteLine(
                $"scan finished in {timespan.Hours:00}h:{timespan.Minutes:00}m:{timespan.Seconds:00}s:{timespan.Milliseconds:00}ms");
            Console.WriteLine($"total directories: {directories.Count}, total files: {fileCount}");
        }
    }
}