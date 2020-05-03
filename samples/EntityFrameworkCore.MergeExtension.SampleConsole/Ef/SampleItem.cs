using System;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.MergeExtension.SampleConsole.Ef
{
    public class SampleItem
    {
        [Key]
        public int Id {get;set;}
        public string Description {get;set;}
        public DateTime Timestamp {get;set;}
    }
}