using System;

namespace WinFormsApp2
{
    public class ArenaCoin : Item
    {
        public ArenaCoin()
        {
            Name = "Arena Coin";
            Description = "A token earned in the battle arena. Sell for 200 gold.";
            Stackable = true;
            Price = 200;
        }
    }
}
