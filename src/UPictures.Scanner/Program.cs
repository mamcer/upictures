using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UPictures.Web.Core;
using UPictures.Web.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace UPictures.Scanner
{
    class Program
    {
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
            var rootPath = configuration["RootDirectory"];
            var imagesPath = configuration["ImagesDirectory"];
            var directoryCount = 0;
            var fileCount = 0;
            
            foreach (var directoryPath in Directory.GetDirectories(rootPath))
            {
                directoryCount += 1;
                Console.WriteLine($"scanning directory:{directoryPath}");
                var albumName = Path.GetFileName(directoryPath);
                if (!upicturesContext.Albums.Any(a => a.Name.Equals(albumName)))
                {
                    var viewDirectory = Path.Combine(imagesPath, "view", albumName);
                    var masterDirectory = Path.Combine(imagesPath, "master", albumName);
                    var thumbnailDirectory = Path.Combine(imagesPath, "thumbnail", albumName);
                    Directory.CreateDirectory(viewDirectory);
                    Directory.CreateDirectory(masterDirectory);
                    Directory.CreateDirectory(thumbnailDirectory);

                    var album = new Album
                    {
                        Name = albumName
                    };

                    foreach (var filePath in Directory.GetFiles(directoryPath))
                    {
                        Console.WriteLine($"        file:{filePath}");
                        var fileName = Path.GetFileName(filePath);
                        if (!fileName.StartsWith("."))
                        {
                            var fileExtension = Path.GetExtension(filePath);

                            if (fileExtension.ToLower(CultureInfo.InvariantCulture) != ".mov" && fileExtension.ToLower(CultureInfo.InvariantCulture) != ".mp4")
                            {
                                // copy master copy
                                var destFile = Path.Combine(masterDirectory, fileName);
                                File.Copy(filePath, destFile, true);

                                // create view copy
                                destFile = Path.Combine(viewDirectory, fileName);
                                using (Image<Rgba32> image = Image.Load(filePath))
                                {
                                    image.Mutate(x => x
                                        .Resize(1024, 0));
                                    image.Save(destFile);
                                }

                                // create thumbnail copy
                                destFile = Path.Combine(thumbnailDirectory, fileName);
                                using (Image<Rgba32> image = Image.Load(filePath))
                                {
                                    image.Mutate(x => x
                                        .Resize(200, 0));
                                    image.Save(destFile);
                                }

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
                                    Console.WriteLine($"        unable to read exif information for: {Path.GetFileName(filePath)}");
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
                                    FileSize = fileSize
                                };

                                album.Pictures.Add(picture);
                                picture.Album = album;
                            }

                            fileCount += 1;
                        }
                    }

                    upicturesContext.Albums.Add(album);
                    upicturesContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"        album '{albumName}' already exists in the database");
                }
            }

            var timespan = DateTime.Now - startingTime;
            Console.WriteLine(
                $"scan finished in {timespan.Hours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}:{timespan.Milliseconds:00}");
            Console.WriteLine($"total directories: {directoryCount}, total files: {fileCount}");
        }
    }
}