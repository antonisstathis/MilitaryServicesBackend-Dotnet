using System.Security.Cryptography;
using System.Text;

namespace MilitaryServicesBackendDotnet.Security
{
    public static class RSAKeyGenerator
    {
        public static void ProduceKeys()
        {
            if (!File.Exists("private_key.pem") || !File.Exists("public_key.pem"))
            {
                using var rsa = RSA.Create(2048);
                File.WriteAllText("private_key.pem", ExportPrivateKey(rsa));
                File.WriteAllText("public_key.pem", ExportPublicKey(rsa));
            }
        }

        public static RSA LoadPrivateKey()
        {
            var pem = File.ReadAllText("private_key.pem");
            return ImportPrivateKey(pem);
        }

        public static RSA LoadPublicKey()
        {
            var pem = File.ReadAllText("public_key.pem");
            return ImportPublicKey(pem);
        }

        private static string ExportPrivateKey(RSA rsa)
        {
            var key = rsa.ExportPkcs8PrivateKey();
            var base64 = Convert.ToBase64String(key);
            return $"-----BEGIN PRIVATE KEY-----\n{InsertLineBreaks(base64)}\n-----END PRIVATE KEY-----";
        }

        private static string ExportPublicKey(RSA rsa)
        {
            var key = rsa.ExportSubjectPublicKeyInfo();
            var base64 = Convert.ToBase64String(key);
            return $"-----BEGIN PUBLIC KEY-----\n{InsertLineBreaks(base64)}\n-----END PUBLIC KEY-----";
        }

        private static RSA ImportPrivateKey(string pem)
        {
            var base64 = ExtractBase64(pem);
            var key = Convert.FromBase64String(base64);
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(key, out _);
            return rsa;
        }

        private static RSA ImportPublicKey(string pem)
        {
            var base64 = ExtractBase64(pem);
            var key = Convert.FromBase64String(base64);
            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(key, out _);
            return rsa;
        }

        private static string ExtractBase64(string pem) =>
            string.Join("", pem.Split('\n')
                .Where(line => !line.StartsWith("-----"))
                .Select(line => line.Trim()));

        private static string InsertLineBreaks(string input, int lineLength = 64)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i += lineLength)
            {
                sb.AppendLine(input.Substring(i, Math.Min(lineLength, input.Length - i)));
            }
            return sb.ToString().TrimEnd();
        }
    }
}
