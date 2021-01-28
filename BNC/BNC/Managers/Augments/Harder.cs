﻿using BNC.TwitchApp;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using System;
using System;
namespace BNC.Managers.Augments
{

    public class Harder : BaseAugment
    {
        private float boost = 1.5f;

        public Harder()
        {
            this.DisplayName = "Monster Hit Harder!";
            this.desc = "Monster do 1.5x more Damage.";
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
            m.DamageToFarmer = (int)Math.Round(m.DamageToFarmer * boost);
            return ActionResponse.Done;
        }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
