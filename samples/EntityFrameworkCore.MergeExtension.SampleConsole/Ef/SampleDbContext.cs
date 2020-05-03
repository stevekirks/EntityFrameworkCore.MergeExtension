using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.MergeExtension.SampleConsole.Ef
{
    public partial class SampleDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public SampleDbContext()
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SampleItem> SampleItems { get; set; }
    }
}
