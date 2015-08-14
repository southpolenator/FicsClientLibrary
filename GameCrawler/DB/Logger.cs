namespace GameCrawler.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Logger")]
    public partial class Logger
    {
        public int Id { get; set; }

        public DateTime? DateTime { get; set; }

        public string Message { get; set; }
    }
}
