using Encurtador_De_Links.Models;
using Microsoft.EntityFrameworkCore;

namespace Encurtador_De_Links.Data;

public class LinkContext : DbContext
{
    public DbSet<Link>? Links { get; set; }

    public LinkContext(DbContextOptions<LinkContext> options) : base(options)
    {
    }
}
