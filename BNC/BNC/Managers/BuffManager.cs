using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BNC
{
    public static class BuffManager
    {
        public static Dictionary<string, BuffOption> CommonBuffs = new Dictionary<string, BuffOption>();
        public static Dictionary<string, BuffOption> CombatBuffs = new Dictionary<string, BuffOption>();

        public static Random rand = new Random();

        public static List<BuffOption> queuedBuff = new List<BuffOption>();

        public static void Init()
        {
            AddBuff(new BuffOption("greenthumb", "Green Thumb").add_farming(4).addShortDesc("Buff farming +4"));
            AddBuff(new BuffOption("brownthumb", "Brown Thumb").add_farming(-4).addShortDesc("Debuff farming -4"));
            AddBuff(new BuffOption("goodfish","Good Fishing Advice").add_fishing(5).addShortDesc("Buff fishing +5"));
            AddBuff(new BuffOption("badfish", "Bad Fishing Advice").add_fishing(-5).addShortDesc("Debuff fishing -5"));
            AddBuff(new BuffOption("w2", "+2 Weapon").add_attack(2).addShortDesc("Buff Attack +2"));
            AddBuff(new BuffOption("w4", "+4 Weapon").add_attack(4).addShortDesc("Buff Attack +4"));
            AddBuff(new BuffOption("beserk","Beserker").add_attack(10).addShortDesc("Buff Attack +10"));
            AddBuff(new BuffOption("rusted","Rusted Weapon").add_attack(-2).addShortDesc("Debuff Attack -2"));
            AddBuff(new BuffOption("broken","Broken Weapon").add_attack(-4).addShortDesc("Debuff Attack -4"));
            AddBuff(new BuffOption("missing","Missing Weapon").add_attack(-8).addShortDesc("Debuff Attack -8"));
            AddBuff(new BuffOption("a2","+2 Armor").add_defense(2).addShortDesc("Buff Defense +2"));
            AddBuff(new BuffOption("a3","+3 Armor").add_defense(3).addShortDesc("Buff Defense +3"));
            AddBuff(new BuffOption("a-2","-2 Armor").add_defense(-2).addShortDesc("Debuff Defense -3"));
            AddBuff(new BuffOption("a-3", "-3 Armor").add_defense(-3).addShortDesc("Debuff Defense -3"));
            AddBuff(new BuffOption("hippie", "Hippie").add_foraging(2).addShortDesc("Buff Foraging +2"));
            AddBuff(new BuffOption("vegan", "Vegan").add_foraging(4).addShortDesc("Buff Foraging +4"));
            AddBuff(new BuffOption("lumber", "Lumberjack").add_foraging(8).addShortDesc("Buff Defense +8"));
            AddBuff(new BuffOption("yuppie", "Yuppies").add_foraging(-2).addShortDesc("Debuff Defense -2"));
            AddBuff(new BuffOption("carnivore","Carnivore").add_foraging(-4).addShortDesc("Debuff Defense -4"));
            AddBuff(new BuffOption("worksout", "Works Out").add_maxStamina(80).addShortDesc("Buff Stamina +80"));
            AddBuff(new BuffOption("potato", "Couch Potato").add_maxStamina(-80).addShortDesc("Debuff Stamina -80"));
            AddBuff(new BuffOption("bionic","Bionic Legs").add_speed(4).addShortDesc("Buff Speed +4"));
            AddBuff(new BuffOption("short", "I'm Just Short").add_speed(-2).addShortDesc("Debuff Speed -2"));
            AddBuff(new BuffOption("clover", "Lucky").add_luck(2).addShortDesc("Buff Luck +2"));
            AddBuff(new BuffOption("stepped", "UnLucky").add_luck(-2).addShortDesc("Debuff Luck -2"));
            AddBuff(new BuffOption("lottery", "Lottery Winner").add_luck(4).addShortDesc("Buff Luck +4"));
            AddBuff(new BuffOption("mirror", "Broke a Mirror").add_luck(-8).addShortDesc("Debuff Luck -8"));
        }

        public static void AddBuff(BuffOption buff)
        {
            if (!CommonBuffs.ContainsKey(buff.id))
                CommonBuffs.Add(buff.id, buff);
        }

        public static void AddComBatBuff(BuffOption buff)
        {
            if (!CombatBuffs.ContainsKey(buff.id))
                CombatBuffs.Add(buff.id, buff);
        }

        public static BuffOption[] getRandomBuff(int count)
        {
            List<BuffOption> list = Enumerable.ToList(CommonBuffs.Values);
            List<BuffOption> returnList = new List<BuffOption>();
            while(returnList.Count < 3)
            {
                BuffOption item = list[rand.Next(list.Count - 1)];

                if(!returnList.Contains(item))
                    returnList.Add(item);
            }
            return returnList.ToArray(); 
        }

        public static BuffOption getIDtoBuff(string id)
        {
            if (CommonBuffs.ContainsKey(id))
            {
                return CommonBuffs[id];
            }
            else if (CombatBuffs.ContainsKey(id))
            {
                return CombatBuffs[id];
            }
            return null;
        }

        public static void AddBuffToQueue(BuffOption buff) {
            queuedBuff.Add(buff);
        }


        public static void UpdateTick()
        {
            if(queuedBuff.Count() >= 1)
            {
                buffPlayer(queuedBuff[0]);
                queuedBuff.RemoveAt(0);
            }
        }

        public static void UpdateDay()
        {

            if (BNC_Core.BNCSave.nextBuffDate <= -1)
                BNC_Core.BNCSave.nextBuffDate = setNextBuffDay(BNC_Core.config.Random_Day_Buff_Min_Max[0], BNC_Core.config.Random_Day_Buff_Min_Max[1]);

            BNC_Core.Logger.Log("buff countdown: " + BNC_Core.BNCSave.nextBuffDate);
            if (BNC_Core.BNCSave.nextBuffDate-- == 0)
            {
                BuffOption[] buffs = getRandomBuff(3);

                foreach (BuffOption buff in buffs)
                    BNC_Core.Logger.Log($"buff selecting from {buff.displayName}");

                if (TwitchIntergration.isConnected() && BNC_Core.config.Random_Day_Buffs)
                {
                    TwitchIntergration.StartBuffPoll(buffs);
                }
                else
                {
                    BuffOption selected = buffs[rand.Next(buffs.Count() - 1)];
                    BNC_Core.Logger.Log($"Selected {selected.displayName}");
                    buffPlayer(selected);
                    BNC_Core.BNCSave.nextBuffDate = setNextBuffDay(BNC_Core.config.Random_Day_Buff_Min_Max[0], BNC_Core.config.Random_Day_Buff_Min_Max[1]);
                }
            }

        }

        public static void buffPlayer(BuffOption buff)
        {
            Buff buffselected = new Buff(buff.farming, buff.fishing, buff.mining, 0, 0, buff.foraging, buff.crafting, buff.maxStamina, buff.magneticRadius, buff.speed, buff.defense, buff.attack, buff.duration, buff.displayName, buff.displayName);
            Game1.buffsDisplay.addOtherBuff(buffselected);
        }

        public static int setNextBuffDay(int number1, int number2)
        {
            int min = Math.Min(number1, number2);
            int max = Math.Max(number1, number2);
            int random = min != max ? rand.Next(max - min) + min : rand.Next(min - 1) + 1;
            return random;
        }


        public class BuffOption
        {
            //Overrides to use a vanilla buff
            public int which = -1;
            internal string hudMessage { get; set; }

            public int farming { get; set; }
            public int fishing { get; set; }
            public int mining { get; set; }
            public int luck { get; set; }
            public int foraging { get; set; }
            public int crafting { get; set; }
            public int maxStamina { get; set; }
            public int magneticRadius { get; set; }
            public int speed { get; set; }
            public int defense { get; set; }
            public int attack { get; set; }
            public int duration { get; set; }
            private bool isBuff { get; set; }
            public string displayName { get; set; }
            public string id { get; set; }
            public string description { get; set; } = "null";
            public string shortdesc { get; set; } = "null";

            public BuffOption(string id, string name, bool isBuff = true, int duration = 1200)
            {
                this.id = id;
                this.displayName = name;
                this.duration = duration;
                this.isBuff = isBuff;
            }

            public BuffOption addDescription(string desc)
            {
                this.description = description;
                return this;
            }

            public BuffOption addShortDesc(string desc)
            {
                this.shortdesc = desc;
                return this;
            }
            public BuffOption add_farming(int value)
            {
                this.farming = value;
                return this;
            }

            public BuffOption add_fishing(int value)
            {
                this.fishing = value;
                return this;
            }

            public BuffOption add_mining(int value)
            {
                this.mining = value;
                return this;
            }

            public BuffOption add_luck(int value)
            {
                this.luck = value;
                return this;
            }

            public BuffOption add_foraging(int value)
            {
                this.foraging = value;
                return this;
            }

            public BuffOption add_crafting(int value)
            {
                this.crafting = value;
                return this;
            }

            public BuffOption add_maxStamina(int value)
            {
                this.maxStamina = value;
                return this;
            }

            public BuffOption add_magneticRadius(int value)
            {
                this.magneticRadius = value;
                return this;
            }

            public BuffOption add_speed(int value)
            {
                this.speed = value;
                return this;
            }

            public BuffOption add_defense(int value)
            {
                this.defense = value;
                return this;
            }

            public BuffOption add_attack(int value)
            {
                this.attack = value;
                return this;
            }

            public BuffOption setDuration(int value)
            {
                this.duration = value;
                return this;
            }


        }

    }
}
