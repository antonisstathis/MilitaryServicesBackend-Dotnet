using System.Globalization;

namespace MilitaryServices.App.Services
{
    public interface IMessageService
    {
        string GetMessage(string code, CultureInfo culture);
    }
}
