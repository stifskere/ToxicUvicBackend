using Microsoft.EntityFrameworkCore;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Structures;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Post> Posts { get; init; } = default!;

    public DbSet<SessionToken> SessionTokens { get; init; } = default!;

    public DbSet<Feedback> FeedBacks { get; init; } = default!;

    public DbSet<Attachment> Attachments { get; init; } = default!;
}