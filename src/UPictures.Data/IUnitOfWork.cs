using System;

namespace UPictures.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}