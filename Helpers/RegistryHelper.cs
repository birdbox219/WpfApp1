using Microsoft.Win32;
using System;

namespace WpfApp1.Helpers
{
    public static class RegistryHelper
    {
        public static object GetValue(RegistryHive hive, string subKey, string valueName)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
                using (var key = baseKey.OpenSubKey(subKey))
                {
                    return key?.GetValue(valueName);
                }
            }
            catch
            {
                return null;
            }
        }

        public static string GetStringValue(RegistryHive hive, string subKey, string valueName)
        {
            return GetValue(hive, subKey, valueName) as string;
        }

        public static bool KeyExists(RegistryHive hive, string subKey)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
                using (var key = baseKey.OpenSubKey(subKey))
                {
                    return key != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
