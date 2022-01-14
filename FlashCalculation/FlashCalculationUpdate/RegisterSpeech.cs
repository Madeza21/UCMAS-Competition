using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace FlashCalculationUpdate
{
    internal class RegisterSpeech
    {
        public static List<string> CopySpeechRegistryEntryFromOneCore()
        {
            var voices = new List<string>();
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Speech_OneCore\Voices\Tokens"))
                using (RegistryKey newKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Speech\Voices\Tokens", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
                {
                    if (key != null)
                    {
                        var keys = key.GetSubKeyNames();
                        foreach (var item in keys)
                        {
                            if (newKey.OpenSubKey(item) == null)
                            {
                                RegistryKey voice = newKey.CreateSubKey(item);
                                foreach (var subKey in key.OpenSubKey(item).GetValueNames())
                                {
                                    var value = key.OpenSubKey(item).GetValue(subKey);
                                    if (value is string) value = (value as string).Replace(@"C:\WINDOWS", "%windir%");
                                    var kind = key.OpenSubKey(item).GetValueKind(subKey);
                                    voice.SetValue(subKey, value, kind);
                                }
                                foreach (var subAttribute in key.OpenSubKey(item).GetSubKeyNames())
                                {
                                    RegistryKey attributes = voice.CreateSubKey(subAttribute);
                                    foreach (var attribute in key.OpenSubKey(item).OpenSubKey(subAttribute).GetValueNames())
                                    {
                                        var value = key.OpenSubKey(item).OpenSubKey(subAttribute).GetValue(attribute);
                                        var kind = key.OpenSubKey(item).OpenSubKey(subAttribute).GetValueKind(attribute);
                                        if (attribute != "SayAsSupport")
                                        {
                                            attributes.SetValue(attribute, value, kind);
                                        }
                                    }
                                }
                                voices.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                voices.Add($"Error: {ex.Message}");
                return voices;
            }
            return voices;
        }
    }
}
