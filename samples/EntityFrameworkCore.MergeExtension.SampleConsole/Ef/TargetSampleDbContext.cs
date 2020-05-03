using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.MergeExtension.SampleConsole.Ef
{
    public partial class TargetSampleDbContext : SampleDbContext
    {
        public TargetSampleDbContext()
        {
        }

        public TargetSampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("target");

            base.OnModelCreating(modelBuilder);
        }
    }
}
