using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.MergeExtension.Tests.Fakes
{
    public partial class FakeDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public virtual DbSet<FakeItem> FakeItems { get; set; }

        public FakeDbContext()
            : base(
                new DbContextOptionsBuilder<FakeDbContext>()
                    .UseInMemoryDatabase("TestDatabase")
                    .Options)
        {
        }
    }
}
