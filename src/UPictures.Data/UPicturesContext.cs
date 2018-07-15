using Microsoft.EntityFrameworkCore;
using UPictures.Core;

namespace UPictures.Data
{
    public class UPicturesContext : DbContext
    {
        public UPicturesContext(DbContextOptions<UPicturesContext> options) : base(options)
        { }
        
        public DbSet<Picture> Pictures { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Picture>().HasKey(x => x.Id);

            builder.Entity<Picture>(config =>
            {
                config.Property(u => u.Hash).HasMaxLength(32);
                config.Property(u => u.Name).HasMaxLength(255);
                config.Property(u => u.CameraMaker).HasMaxLength(255);
                config.Property(u => u.CameraModel).HasMaxLength(255);
                config.Property(u => u.Extension).HasMaxLength(50);
                config.Property(u => u.DirectoryName).HasMaxLength(5);
                config.ToTable("Picture"); 
            });
        }
    }    
}