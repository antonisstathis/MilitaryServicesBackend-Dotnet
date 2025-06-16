using Ganss.Xss;

namespace MilitaryServices.App.Security
{
    public static class SanitizationUtil
    {
        private static readonly HtmlSanitizer _sanitizer;

        static SanitizationUtil()
        {
            _sanitizer = new HtmlSanitizer();
            _sanitizer.AllowedTags.Clear(); 
            _sanitizer.AllowedTags.Add("b");
            _sanitizer.AllowedTags.Add("i");
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("u");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("em");
        }

        public static string Sanitize(string input)
        {
            return _sanitizer.Sanitize(input);
        }
    }
}
