
namespace BNC.Configs
{
    public class Config
    {
        public bool Random_Day_Buffs { get; set; } = true;

        public int[] Random_Day_Buff_Min_Max { get; set; } = new int[] { 2, 4 };

        public bool Use_Bits_To_Spawn_Mobs { get; set; } = true;

        public int Bits_To_Mobs_Bit_Amount { get; set; } = 100;

        public bool Spawn_Subscriber_Junimo { get; set; } = true;
    }
}
