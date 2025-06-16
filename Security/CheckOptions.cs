using System;
using MilitaryServices.App.Enums;

namespace MilitaryServices.App.Security
{
    public static class CheckOptions
    {
        public static bool AreValidOptions(string situation, string active)
        {
            return CheckSituation(situation) && CheckActive(active);
        }

        public static bool CheckSituation(string situation)
        {
            if (string.IsNullOrEmpty(situation))
                return false;

            string normalized = situation.ToLowerInvariant();

            if (normalized == Situation.ARMED.ToString().ToLowerInvariant())
                return true;

            if (normalized == Situation.UNARMED.ToString().ToLowerInvariant())
                return true;

            return false;
        }

        public static bool CheckActive(string active)
        {
            if (string.IsNullOrEmpty(active))
                return false;

            string normalized = active.ToLowerInvariant();

            if (normalized == ActiveExtensions.GetFreeOfDuty().ToLowerInvariant())
                return true;

            if (normalized == Active.ACTIVE.ToString().ToLowerInvariant())
                return true;

            return false;
        }
    }
}
