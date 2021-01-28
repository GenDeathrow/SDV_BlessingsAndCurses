using BNC.TwitchApp;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using System;

namespace BNC.Managers.Augments
{

    public class Speed : BaseAugment
    {
        private float boost = 1.2f;
        
        public Speed()
        {
            this.DisplayName = "Monsters are Faster";
            this.desc = "They were given coffee.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate() { return ActionResponse.Done; }

        public BaseAugment setBoostAmount(float x)
        {
            this.boost = x;
            return this;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster m)
        {
            m.speed += (int)Math.Round(m.speed * boost);
            return ActionResponse.Done;
        }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
