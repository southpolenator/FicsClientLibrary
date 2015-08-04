namespace Internet.Chess.Server.Fics
{
    public class Player
    {
        /// <summary>
        /// Player's username (handle) on server.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Player's Rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Is player rated?
        /// </summary>
        public bool IsRated { get { return this.Rating > -1; } }

        /// <summary>
        /// Is player registered or is he a guest on server?
        /// </summary>
        public bool IsRegistered { get { return this.Rating > -2; } }

        /// <summary>
        /// Player's status
        /// </summary>
        public PlayerStatus Status { get; set; }

        /// <summary>
        /// Player's account status
        /// </summary>
        public AccountStatus AccountStatus { get; set; }

        /// <summary>
        /// Player's provisional rating
        /// </summary>
        public PlayerProvisionalRating ProvisionalRating { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string rating = this.Rating.ToString();

            if (!IsRegistered)
            {
                rating = "++++";
            }
            else if (!IsRated)
            {
                rating = "----";
            }

            return rating + Status.GetSingleAttribute<ServerVariableNameAttribute>().Name + this.Username;
        }
    }
}
