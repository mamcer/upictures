using System;
using Microsoft.EntityFrameworkCore;

namespace UPictures.Data
{
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public EntityFrameworkUnitOfWork(DbContext context)
        {
            _context = context;   
        }

        public void Dispose()
        {}

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}