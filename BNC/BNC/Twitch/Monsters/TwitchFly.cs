using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System.Xml.Serialization;

namespace BNC.Twitch
{
    class TwitchFly : Fly, ITwitchMonster
    {
        public TwitchFly(Vector2 position, bool hard) : base(position, hard)
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
