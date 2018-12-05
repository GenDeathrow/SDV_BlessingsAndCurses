using BNC.Twitch;
using Microsoft.Xna.Framework;
using StardewValley.Characters;

namespace BNC
{
    class TwitchJunimo : Junimo, ITwitchMonster
    {
        public TwitchJunimo(Vector2 position) : base(position, 0, true) { }

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
