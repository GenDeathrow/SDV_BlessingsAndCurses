using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace BNC.Twitch
{
    class TwitchBug : Bug, ITwitchMonster
    {
        public TwitchBug(Vector2 position) : base(position, 0)
        {
            this.Health = health / 2;
            this.MaxHealth = maxHealth / 2;
        }

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
