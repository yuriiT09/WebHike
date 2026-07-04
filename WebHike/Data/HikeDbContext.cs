using Microsoft.EntityFrameworkCore;
using WebHike.Data.Entities;

namespace WebHike.Data;

public class HikeDbContext : DbContext
{
    public HikeDbContext(DbContextOptions<HikeDbContext> options)
        : base(options)
    {}

    public DbSet<CategoryEntity> Categories { get; set; }
}