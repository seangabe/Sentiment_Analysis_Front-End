namespace TWIL1
{
    public class ServiceAccount
    {
            public static string GetCredentialsPath()
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "App_Data/credentials.json");
            }

            public static string GetCredentials()
            {
                return File.ReadAllText(GetCredentialsPath());
            }
    }
}
