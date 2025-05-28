namespace MilitaryServices.App.Enums
{
    public enum Discharged
    {
        IN_OPERATION,
        DISCHARGED
    }

    public static class DischargedExtensions
    {
        public static string GetInOperation()
        {
            return Discharged.IN_OPERATION.ToString().ToLower().Replace("_", " ");
        }

        public static string GetDischarged()
        {
            return Discharged.DISCHARGED.ToString().ToLower();
        }

        public static string GetDischarged(bool discharged)
        {
            return discharged ? GetDischarged() : GetInOperation();
        }
    }
}
