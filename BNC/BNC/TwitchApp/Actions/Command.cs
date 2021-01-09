using System;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using static BNC.BuffManager;

namespace BNC.Actions
{
    class Command : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "cmd")]
        public string cmd;


        public override ActionResponse Handle()
        {
            Game1.game1.parseDebugInput(cmd);

            return ActionResponse.Done;
        }
    }
}
