using System;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.MergeExtension.Tests.Fakes
{
    public class FakeItem
    {
        [Key]
        public int Id {get;set;}
        public string Description {get;set;}
        public DateTime Timestamp {get;set;}
    }
}