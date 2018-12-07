using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System.Xml.Serialization;

namespace BNC.Twitch
{
    class TwitchBat : Bat, ITwitchMonster
    {
        public TwitchBat(Vector2 position, int mineLevel) : base(position, mineLevel)
        {
        }

        [XmlIgnore]
        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
        }
    }
}
