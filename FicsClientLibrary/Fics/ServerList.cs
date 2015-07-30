namespace Internet.Chess.Server.Fics
{
    using System.Collections.Generic;

    public class ServerList : ICollection<string>
    {
        public const string CensoredList = "censor";
        public const string WontPlayList = "noplay";
        public const string ListeningChannelsList = "channel";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerList"/> class.
        /// </summary>
        /// <param name="ficsClient">The FICS client.</param>
        internal ServerList(FicsClient ficsClient)
        {
            this.ficsClient = ficsClient;
        }

        public string Name { get; internal set; }
        public bool Public { get; internal set; }
        public bool Personal
        {
            get { return !Public; }
            internal set { Public = !value; }
        }

        private HashSet<string> set = new HashSet<string>();
        private bool refreshed = false;
        private FicsClient ficsClient;

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(string item)
        {
            RefreshIfNeeded();
            if (!this.set.Contains(item))
            {
                ficsClient.AddListEntry(this.Name, item);
                this.set.Add(item);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            RefreshIfNeeded();
            foreach (string item in this.set)
            {
                ficsClient.RemoveListEntry(this.Name, item);
            }

            this.set.Clear();
        }

        public bool Contains(string item)
        {
            RefreshIfNeeded();
            return this.set.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            RefreshIfNeeded();
            this.set.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get
            {
                RefreshIfNeeded();
                return this.set.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.Public; }
        }

        public bool Remove(string item)
        {
            RefreshIfNeeded();
            if (this.set.Contains(item))
            {
                ficsClient.RemoveListEntry(this.Name, item);
                return this.set.Remove(item);
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            RefreshIfNeeded();
            return this.set.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            RefreshIfNeeded();
            return this.set.GetEnumerator();
        }

        public void ForceRefresh()
        {
            foreach (string element in this.ficsClient.GetListEntries(this.Name).Result)
            {
                this.set.Add(element);
            }

            refreshed = true;
        }

        private void RefreshIfNeeded()
        {
            if (!refreshed)
            {
                ForceRefresh();
            }
        }
    }
}
