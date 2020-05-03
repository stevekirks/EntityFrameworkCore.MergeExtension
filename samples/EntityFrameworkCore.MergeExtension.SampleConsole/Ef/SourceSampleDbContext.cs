using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.MergeExtension.SampleConsole.Ef
{
    public partial class SourceSampleDbContext : SampleDbContext
    {
        public SourceSampleDbContext()
        {
        }

        public SourceSampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("source");

            base.OnModelCreating(modelBuilder);
        }
    }
}
