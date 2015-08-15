namespace GameCrawler.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GameMove")]
    public partial class GameMove
    {
        public int Id { get; set; }

        public int? GameId { get; set; }

        public bool? WhiteMove { get; set; }

        [Column("GameMove")]
        [StringLength(10)]
        public string Move { get; set; }

        public TimeSpan? MoveTime { get; set; }

        public int? MoveNumber { get; set; }

        public virtual Game Game { get; set; }
    }
}
