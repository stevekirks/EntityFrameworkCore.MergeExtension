using System;
using System.IO;
using EntityFrameworkCore.MergeExtension.Tests.Fakes;
using Xunit;

namespace EntityFrameworkCore.MergeExtension.Tests
{
    public class MergeExtensionTests
    {
        [Fact]
        public void GeneratesExpectedMergeRawSql()
        {
            var expectedSqlMerge = File.ReadAllText("GeneratesExpectedMergeRawSqlExpectedResult.sql");

            using var ctx = new FakeDbContext();
            
            var actualSqlMerge = ctx.GetRawSqlMerge<FakeItem>("SourceTable", "TargetTable");

            Assert.Equal(expectedSqlMerge, actualSqlMerge);
        }
    }
}
