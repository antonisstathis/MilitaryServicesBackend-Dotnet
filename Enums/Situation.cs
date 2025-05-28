namespace MilitaryServices.App.Enums
{
    public enum Situation
    {
        ARMED,
        UNARMED
    }

    public static class SituationExtensions
    {
        public static string GetNameOfColumn()
        {
            return "situation";
        }
    }
}
