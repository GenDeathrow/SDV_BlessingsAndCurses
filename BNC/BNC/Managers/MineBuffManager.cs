using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BNC
{
    static class MineBuffManager
    {

        public static String CurrentAugment = null;
        public static bool finishedLoop = false;

        public static Dictionary<String, AugmentOption> Augments = new Dictionary<string, AugmentOption>();

        public static List<Monster> queue = new List<Monster>();

        public static void Init()
        {
            Augments.Add("health", new AugmentOption("health", "Monster Health Increase", "Monster have 1.5x more health."));
            Augments.Add("regen", new AugmentOption("regen", "Health Regen", "Player regens health over time."));
            Augments.Add("harder", new AugmentOption("harder", "Monster Attack Increase", "Monster have 1.5x more attack power"));
            Augments.Add("exp", new AugmentOption("exp", "Extra Exp!", "Monster have 1.5x more experiance."));
            //Augments.Add("crabs", new AugmentOption("crabs", "You have crabs!", "Spawns Crabs on each level"));
            //Augments.Add("extra", new AugmentOption("extra", "More Mobs", "More mobs per level."));
            Augments.Add("loss", new AugmentOption("loss", "Bleeding Out", "Player loses health over time."));
        }

        public static void mineLevelChanged(object sender, EventArgsMineLevelChanged e)
        {
            if (shouldUpdateAugment(e))
            {
                if (TwitchIntergration.isConnected())
                {
                   if(!TwitchIntergration.hasMinePollStarted)
                        TwitchIntergration.StartMinePoll(getRandomBuff(3));
                }
                else
                {
                    AugmentOption aug = getRandomBuff(1)[0];
                    CurrentAugment = aug.id;
                    Game1.addHUDMessage(new HUDMessage(aug.DisplayName + ": " + aug.desc, null));
                }
            }

            if(CurrentAugment != null)
                UpdateLocation();

        }

        public static AugmentOption[] getRandomBuff(int count)
        {
            List<AugmentOption> list = Enumerable.ToList(Augments.Values);
            List<AugmentOption> returnList = new List<AugmentOption>();
            while (returnList.Count < 3)
            {
                AugmentOption item = list[Game1.random.Next(list.Count - 1)];

                if (!returnList.Contains(item))
                    returnList.Add(item);
            }
            return returnList.ToArray();
        }

        public static void UpdateMineAugments()
        {

        }

        public static bool shouldUpdateAugment(EventArgsMineLevelChanged e)
        {
            if (e.CurrentMineLevel % 1 == 0 || CurrentAugment == null)
                return true;
            return false;
        }


        public static bool isMineShaft()
        {
            if (Game1.currentLocation.Name.StartsWith("UndergroundMine"))
                return true;
            return false;
        }
        
        // Runs on each mine location update
        public static void UpdateLocation()
        {
            if (!isMineShaft())
                return;

            foreach (NPC npc in Game1.player.currentLocation.characters)
            {
                if (!(npc is Monster)) continue;
                Monster m = (Monster)npc;
                switch (CurrentAugment)
                {
                    case "health":
                        BoostHealth(m);
                        break;
                    case "harder":
                        Harden(m);
                        break;
                    case "exp":
                        BoostExp(m);
                        break;
                    case "crabs":
                        int cnt = Game1.random.Next(2) + 1;
                        for (int i = 0; i < cnt; i++) 
                            queue.Add(new RockCrab(Vector2.Zero));
                        BNC_Core.Logger.Log($"Added {cnt} crabs");
                        break;
                    case "extra":
                        foreach (NPC n in Game1.currentLocation.characters)
                        {
                            if (!(n is Monster)) continue;
                            int flag = Game1.random.Next(1);
                            if (flag.Equals(1))
                            {
                                int type = Game1.random.Next(3);
                                switch (type)
                                {
                                    case 0:
                                        queue.Add(new GreenSlime(Vector2.Zero));
                                        break;
                                    case 1:
                                        queue.Add(new Fly(Vector2.Zero));
                                        break;
                                    case 2:
                                        queue.Add(new RockCrab(Vector2.Zero));
                                        break;
                                    case 3:
                                        queue.Add(new Grub(Vector2.Zero));
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private static int augmentTick = 0;
        // Runs ever second
        public static void UpdateTick()
        {
            if (!isMineShaft())
                return;

            if (queue.Count > 0)
            {
                GameLocation location = Game1.player.currentLocation;
                bool flag = location.addCharacterAtRandomLocation(queue[0]);
                if (flag)
                    queue.RemoveAt(0);
            }

            switch (CurrentAugment)
            {
                case "regen":
                    if (augmentTick++ > 0)
                    {
                        if(Game1.player.health+1 < Game1.player.maxHealth)
                            Game1.player.health += 1;

                        augmentTick = 0;
                    }
                    break;
                case "loss":
                    if (augmentTick++ > 5)
                    {
                        if (Game1.player.health > (Game1.player.health * 0.25))
                            Game1.player.health -= 1;
                        augmentTick = 0;
                    }
                    break;
                default:
                    break;

            }

        }

        private static void BoostHealth(Monster m)
        {
            m.MaxHealth = (int)Math.Round(m.MaxHealth * 1.5);
            m.Health = m.MaxHealth;
        }

        private static void BoostExp(Monster m)
        {
            m.ExperienceGained = (int)Math.Round(m.ExperienceGained * 1.5);
        }

        private static void Harden(Monster m)
        {
            m.DamageToFarmer = (int)Math.Round(m.DamageToFarmer * 1.5);
        }


        public class AugmentOption
        {
            public String DisplayName;
            public String id;
            public String desc;

            public AugmentOption(String id, String name, String desc)
            {
                this.DisplayName = name;
                this.id = id;
                this.desc = desc;
            }

        }
    }
}
