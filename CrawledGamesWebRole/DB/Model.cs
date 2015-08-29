namespace CrawledGamesWebRole.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.SqlServer;

    public partial class Model : DbContext
    {
        class MyConfiguration : DbConfiguration
        {
            public MyConfiguration()
            {
                SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            }
        }

        static Model()
        {
            DbConfiguration.SetConfiguration(new MyConfiguration());
        }

        public Model(string connectionString)
            : base(connectionString)
        {
        }

        public static Model SqlAzure(string serverName, string databaseName, string username, string password)
        {
            return new Model(string.Format("Server=tcp:{0}.database.windows.net;Database={3};User ID ={1}@{0}; Password={2};Trusted_Connection=False;Encrypt=True;", serverName, username, password, databaseName));
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameMove> GameMoves { get; set; }
        public virtual DbSet<Logger> Loggers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .Property(e => e.WhitePlayer)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .Property(e => e.BlackPlayer)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .Property(e => e.Result)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.AllPartnerGames)
                .WithOptional(e => e.PartnersGame)
                .HasForeignKey(e => e.PartnerGameId);

            modelBuilder.Entity<GameMove>()
                .Property(e => e.Move)
                .IsUnicode(false);
        }
    }
}
