using Microsoft.EntityFrameworkCore;
using UPictures.Web.Core;

namespace UPictures.Web.Data
{
    public class UPicturesContext : DbContext
    {
        public UPicturesContext(DbContextOptions<UPicturesContext> options) : base(options)
        { }
        
        public DbSet<Picture> Pictures { get; set; }

        public DbSet<Album> Albums { get; set; }
    }    
}