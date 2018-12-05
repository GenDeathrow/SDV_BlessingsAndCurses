using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace BNC.Twitch
{
    class TwitchFly : Fly, ITwitchMonster
    {
        public TwitchFly(Vector2 position) : base(position)
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
