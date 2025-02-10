namespace AceJobAgency.Helpers
{
    public class OtpGenerator
    {
        private static Random random = new Random();

        public static string GenerateOtp()
        {
            return random.Next(100000, 999999).ToString();
        }
    }
}
