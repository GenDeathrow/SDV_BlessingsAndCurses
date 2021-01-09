using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System.Xml.Serialization;

namespace BNC.Twitch
{
    class TwitchBug : Bug, ITwitchMonster
    {
        public TwitchBug() { }
        public TwitchBug(Vector2 position, int minelevel) : base(position, minelevel)  { }

        [XmlIgnore]
        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
            this.Name = username;
            this.displayName = username;
        }
    }
}
