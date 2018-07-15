using System.Linq;
using UPictures.Core;
using Microsoft.EntityFrameworkCore;

namespace UPictures.Data
{
    public class PictureRepository : EntityFrameworkRepository<Picture, int>, IPictureRepository
    {
        public PictureRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public bool ContainsHash(string hash)
        {
            return _dbContext.Set<Picture>().Any(p => p.Hash == hash);
        }
    }
}