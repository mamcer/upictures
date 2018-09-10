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
using log4net;
using System.Reflection;
using log4net.Config;

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

        public static string GetSizeString(long size)
        {
            string unit;
            double finalSize;
            if (size < 1024)
            {
                unit = "bytes";
                finalSize = size;
            }
            else if (size < 1024 * 1024)
            {
                unit = "Kb";
                finalSize = Convert.ToDouble(size) / 1024;
            }
            else if (size < 1024 * 1024 * 1024)
            {
                unit = "Mb";
                finalSize = Convert.ToDouble(size) / (1024 * 1024);
            }
            else
            {
                unit = "Gb";
                finalSize = Convert.ToDouble(size) / (1024 * 1024 * 1024);
            }

            return string.Format("{0} {1}", Math.Round(finalSize, 2), unit);
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

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

            var startingTime = DateTime.Now;
            Console.WriteLine($"scan started {startingTime.Hour:00}:{startingTime.Minute:00}:{startingTime.Second:00}");
            var rootDirectory = configuration["RootDirectory"];
            var fileCount = 0;

            var directories = new List<string> {rootDirectory};
            var unitOfWork = new EntityFrameworkUnitOfWork(upicturesContext);
            var pictureRepository = new PictureRepository(upicturesContext);
            for (int i = 0; i < directories.Count; i++)
            {
                var localDirectories = Directory.GetFiles(directories[i]);

                if (localDirectories.Length > 0)
                {
                    foreach (var filePath in localDirectories)
                    {
                        var fileExtension = Path.GetExtension(filePath).ToLower();
                        var fileName = Path.GetFileNameWithoutExtension(filePath);

                        if (fileName.StartsWith("."))
                        {
                            Console.WriteLine($"        file name starts with dot:{filePath}");
                            log.Error($"file name starts with dot:{filePath}");
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

                        //// check if file already exists in the database
                        //if (pictureRepository.ContainsHash(hash))
                        //{
                        //    Console.WriteLine($"        file exists:{filePath}");
                        //    log.Error($"file exists:{filePath}");
                        //    continue;
                        //}

                        //Console.WriteLine($"        processing file:{filePath}");

                        long fileSize;
                        using (var file = File.OpenRead(filePath))
                        {
                            fileSize = file.Length;
                        }

                        var ufile = new UFile
                        {
                            Name = Path.GetFileNameWithoutExtension(fileName),
                            Extension = fileExtension,
                            Size = fileSize,
                            CreationDate = File.GetLastWriteTime(filePath),
                            Hash = hash,
                            Path = Path.GetDirectoryName(filePath)
                        };

                        Console.WriteLine($"        name:{ufile.Name}, extension:{ufile.Extension}, size:{GetSizeString(ufile.Size)}, creationDate:{ufile.CreationDate.ToShortDateString()}, hash:{ufile.Hash}, path:{ufile.Path}");

                        fileCount += 1;
                        //pictureRepository.Create(picture);
                    }

                    //unitOfWork.SaveChanges();
                }

                directories.AddRange(Directory.GetDirectories(directories[i]));
            }
            
            var timespan = DateTime.Now - startingTime;
            Console.WriteLine(
                $"scan finished in {timespan.Hours:00}h:{timespan.Minutes:00}m:{timespan.Seconds:00}s:{timespan.Milliseconds:00}ms");
            Console.WriteLine($"total directories: {directories.Count}, total files: {fileCount}");
        }
    }
}