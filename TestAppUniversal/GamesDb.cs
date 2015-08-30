using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace TestAppUniversal.DB
{
    [DataContract]
    public class Game
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Player WhitePlayer { get; set; }

        [DataMember]
        public Player BlackPlayer { get; set; }

        [DataMember]
        public Game PartnersGame { get; set; }

        [DataMember]
        public int GameType { get; set; }

        [DataMember]
        public DateTime GameStarted { get; set; }

        [DataMember]
        public bool Rated { get; set; }

        [DataMember(Name = "ClockStart")]
        public string ClockStartString { get; set; }

        public TimeSpan ClockStart
        {
            get { return TimeSpan.Parse(ClockStartString); }
        }

        [DataMember(Name = "TimeIncrement")]
        public string TimeIncrementString { get; set; }

        public TimeSpan TimeIncrement
        {
            get { return TimeSpan.Parse(TimeIncrementString); }
        }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public GameMove[] WhitePlayerMoves { get; set; }

        [DataMember]
        public GameMove[] BlackPlayerMoves { get; set; }
    }

    [DataContract]
    public class Player
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public int Rating { get; set; }
    }

    [DataContract]
    public class GameMove
    {
        [DataMember]
        public string Move { get; set; }

        [DataMember(Name = "MoveTime")]
        public string MoveTimeString { get; set; }

        public TimeSpan MoveTime
        {
            get { return TimeSpan.Parse(MoveTimeString); }
        }
    }

    public class GamesDb
    {
        private const string ServiceUrl = "http://icscrawlertest.cloudapp.net/api/games";
        private readonly HttpClient _client = new HttpClient();

        public async Task<Game> GetGame(int id)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(string.Format("{0}/game?id={1}", ServiceUrl, id));
                var jsonSerializer = CreateDataContractJsonSerializer(typeof(Game));
                var text = await response.Content.ReadAsStringAsync();
                var stream = await response.Content.ReadAsStreamAsync();
                return (Game)jsonSerializer.ReadObject(stream);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DataContractJsonSerializer CreateDataContractJsonSerializer(Type type)
        {
            const string dateFormat = "yyyy-MM-ddTHH:mm:ss";
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat(dateFormat)
            };

            return new DataContractJsonSerializer(type, settings);
        }
    }
}
