namespace PUBG
{
    public class PUBGApiOptions
    {

        /// <summary>
        /// kakao - Kakao
        /// stadia - Stadia
        /// steam - Steam
        /// tournament - Tournaments
        /// psn - PSN
        /// xbox - Xbox
        /// console - PSN/Xbox(used for the /matches and /samples endpoints)
        /// default value is steam
        /// </summary>
        public string Shard { get; set; } = "steam";

        /// <summary>
        /// ApiKey
        /// </summary>
        public string ApiKey { get; set; } 
    }
}
