using System;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using static BNC.BuffManager;

namespace BNC.Actions
{
    class ScreenFlash : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "color")]
        public String color;

        public override ActionResponse Handle()
        {
            Farmer who = Game1.player;

            Game1.screenGlow = false;
            var prop = typeof(Color).GetProperty(color);
            if (prop != null)
                Game1.screenGlowOnce((Color)prop.GetValue(color), hold: false);
            else
                Game1.screenGlowOnce(Color.SlateBlue, hold: false);

            return ActionResponse.Done;
        }
    }
}
