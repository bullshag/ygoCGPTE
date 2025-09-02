namespace WinFormsApp2
{
    public class Weapon : Item
    {
        public Weapon()
        {
            Stackable = true;
        }
        public double StrScaling { get; init; }
        public double DexScaling { get; init; }
        public double IntScaling { get; init; }
        public double MinMultiplier { get; init; }
        public double MaxMultiplier { get; init; }
        public double CritChanceBonus { get; init; }
        public double CritDamageBonus { get; init; }
        public double AttackSpeedMod { get; init; }
        public bool TwoHanded { get; init; }

        public double ProcChance { get; set; }
        public Ability? ProcAbility { get; set; }
    }
}
