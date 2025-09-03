using System;

namespace WinFormsApp2
{
    public class AbilityTome : Item
    {
        public int AbilityId { get; }

        public AbilityTome(int abilityId, string abilityName, string abilityDescription)
        {
            AbilityId = abilityId;
            Name = $"Tome: {abilityName}";
            Description = abilityDescription;
            Stackable = false;
        }
    }
}
