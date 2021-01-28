﻿using BNC.TwitchApp;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using System;
using System;
namespace BNC.Managers.Augments
{
    public class Exp : BaseAugment
    {
        private float boost = 1.5f;

        public Exp()
        {
            this.DisplayName = "Monsters Extra Exp!";
            this.desc = "Monster have 1.5x more experiance.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            return ActionResponse.Done;
        }

        public BaseAugment setBoostAmount(float x)
        {
            this.boost = x;
            return this;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster m)
        {
            m.ExperienceGained = (int)Math.Round(m.ExperienceGained * boost);
            return ActionResponse.Done;
        }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
