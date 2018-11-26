using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace BNC
{
    class TwitchSlime : GreenSlime
    {
        public TwitchSlime(Vector2 position) : base(position) {
            this.Health = health / 2;
            this.MaxHealth = maxHealth / 2;
        }
    }
}
