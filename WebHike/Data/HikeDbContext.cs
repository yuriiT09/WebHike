using Microsoft.EntityFrameworkCore;
using WebHike.Data.Entities;

namespace WebHike.Data;

public class HikeDbContext : DbContext
{
    public HikeDbContext(DbContextOptions<HikeDbContext> options)
        : base(options)
    {
    }

    public DbSet<CategoryEntity> Categories { get; set; }

    public DbSet<ItemEntity> Items { get; set; }

    public DbSet<ItemImageEntity> ItemImages { get; set; }

    public DbSet<UserEntity> Users { get; set; }
}