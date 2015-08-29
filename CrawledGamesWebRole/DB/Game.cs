namespace CrawledGamesWebRole.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Game")]
    public partial class Game
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Game()
        {
            AllPartnerGames = new HashSet<Game>();
            GameMoves = new HashSet<GameMove>();
        }

        public int Id { get; set; }

        [StringLength(255)]
        public string WhitePlayer { get; set; }

        public int? WhitePlayerRating { get; set; }

        [StringLength(255)]
        public string BlackPlayer { get; set; }

        public int? BlackPlayerRating { get; set; }

        public int? PartnerGameId { get; set; }

        public int? GameType { get; set; }

        public DateTime? GameStarted { get; set; }

        public bool? Rated { get; set; }

        public TimeSpan? ClockStart { get; set; }

        public TimeSpan? TimeIncrement { get; set; }

        [StringLength(100)]
        public string Result { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Game> AllPartnerGames { get; set; }

        public virtual Game PartnersGame { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GameMove> GameMoves { get; set; }
    }
}
