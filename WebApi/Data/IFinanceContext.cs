using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IFinanceContext
    {
        DbSet<Transaction> Transaction { get; set; }
        DbSet<UserEntity> UserEntity { get; set; }
        int SaveChanges();
        void Dispose();
    }
}
