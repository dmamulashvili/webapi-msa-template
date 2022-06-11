using Microsoft.EntityFrameworkCore;

namespace MSA.Template.Infrastructure;

public class SlaveDbContext : MasterDbContext
{
    public SlaveDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }
}