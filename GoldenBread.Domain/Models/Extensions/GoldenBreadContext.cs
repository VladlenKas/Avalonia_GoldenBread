using GoldenBread.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Domain.Models;

public partial class GoldenBreadContext 
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasQueryFilter(u => u.Dismissed == 0);
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.Deleted == 0);
    }

    public IQueryable<User> RegularUsers => Users.Where(x => x.AccountType == AccountType.User);
    public IQueryable<User> Companies => Users.Where(x => x.AccountType == AccountType.Company);
}
