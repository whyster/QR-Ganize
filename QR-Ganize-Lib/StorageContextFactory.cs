using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QR_Ganize_Lib;

public class StorageContextFactory : IDesignTimeDbContextFactory<StorageDbContext>
{
    public StorageDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StorageDbContext>();
        optionsBuilder.UseSqlite("Data Source=storage.db");

        return new StorageDbContext(optionsBuilder.Options);
    }
}