using UPictures.Core;

namespace UPictures.Data
{
    public interface IPictureRepository : IRepository<Picture, int>
    {
        bool ContainsHash(string hash);
    }
}