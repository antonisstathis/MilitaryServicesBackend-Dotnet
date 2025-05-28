namespace MilitaryServices.App.Dto
{
    public class SoldierProportion
    {
        public int SoldId { get; set; }
        public float Proportion { get; set; }

        public SoldierProportion() { }

        public SoldierProportion(int soldId, float proportion)
        {
            SoldId = soldId;
            Proportion = proportion;
        }
    }
}
