namespace LocalizationTool
{
    public static class CredentialHelper
    {
        public static string SourceRepoUserName = "SourceRepoUserName";
        public static string SourceRepoPassword = "SourceRepoPassword";
        public static string TargetRepoUserName = "TargetRepoUserName";
        public static string TargetRepoPassword = "TargetRepoPassword";

        public static void SaveInRegistery(string key,string value)
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            exampleRegistryKey?.SetValue(key,value);
            exampleRegistryKey?.Close();
        }

        public static string GetFormRegistery(string key)
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            var result =  (string) exampleRegistryKey?.GetValue(key);
            exampleRegistryKey?.Close();
            return result;
        }
    }
}