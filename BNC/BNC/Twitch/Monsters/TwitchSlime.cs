using BNC.Twitch;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace BNC
{
    class TwitchSlime : GreenSlime, ITwitchMonster
    {

        public TwitchSlime(Vector2 position) : base(position) { }

        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        public override void collisionWithFarmerBehavior()
        {
            if (Game1.random.NextDouble() < 0.3 && !this.Player.temporarilyInvincible && (!this.Player.isWearingRing(520) && Game1.buffsDisplay.addOtherBuff(new Buff(13))))
                this.currentLocation.playSound("slime");
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
        }
    }
}
