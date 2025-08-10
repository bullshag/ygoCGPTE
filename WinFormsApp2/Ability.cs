namespace WinFormsApp2
{
    public class Ability
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Cost { get; set; }
        public int Slot { get; set; }
        public int Priority { get; set; }
    }
}
