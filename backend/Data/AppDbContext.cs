using CharactherGenerator.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CharactherGenerator.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    
}