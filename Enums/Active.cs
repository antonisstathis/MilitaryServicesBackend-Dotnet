namespace MilitaryServices.App.Enums
{
    public enum Active
    {
        ACTIVE,
        FREE_OF_DUTY
    }

    public static class ActiveExtensions
    {
        public static string GetFreeOfDuty()
        {
            return Active.FREE_OF_DUTY.ToString().ToLower().Replace("_", " ");
        }
    }
}
