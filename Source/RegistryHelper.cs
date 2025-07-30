using Microsoft.Win32;

namespace SnapText
{
    public static class RegistryHelper
    {
        private const string RegistryPath = @"SOFTWARE\SnapText";
        private const string ApiKeyValueName = "IronOcrApiKey";
        private const string HotkeyModifiersValueName = "HotkeyModifiers";
        private const string HotkeyKeyValueName = "HotkeyKey";

        public static string? GetApiKey()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
                return key?.GetValue(ApiKeyValueName)?.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static bool SetApiKey(string apiKey)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
                key.SetValue(ApiKeyValueName, apiKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ClearApiKey()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true);
                if (key != null)
                {
                    key.DeleteValue(ApiKeyValueName, throwOnMissingValue: false);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasApiKey()
        {
            return !string.IsNullOrWhiteSpace(GetApiKey());
        }

        public static (uint modifiers, uint key) GetHotkey()
        {
            try
            {
                using var regKey = Registry.CurrentUser.OpenSubKey(RegistryPath);
                if (regKey != null)
                {
                    var modifiers = regKey.GetValue(HotkeyModifiersValueName)?.ToString();
                    var key = regKey.GetValue(HotkeyKeyValueName)?.ToString();
                    
                    if (uint.TryParse(modifiers, out uint mod) && uint.TryParse(key, out uint k))
                    {
                        return (mod, k);
                    }
                }
            }
            catch
            {
                // Fall through to default
            }
            
            // Default: Ctrl+Shift+S (MOD_CONTROL | MOD_SHIFT, VK_S)
            return (0x0002 | 0x0004, 0x53);
        }

        public static bool SetHotkey(uint modifiers, uint key)
        {
            try
            {
                using var regKey = Registry.CurrentUser.CreateSubKey(RegistryPath);
                regKey.SetValue(HotkeyModifiersValueName, modifiers.ToString());
                regKey.SetValue(HotkeyKeyValueName, key.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ClearHotkey()
        {
            try
            {
                using var regKey = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true);
                if (regKey != null)
                {
                    regKey.DeleteValue(HotkeyModifiersValueName, throwOnMissingValue: false);
                    regKey.DeleteValue(HotkeyKeyValueName, throwOnMissingValue: false);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetHotkeyDisplayString()
        {
            var (modifiers, key) = GetHotkey();
            var parts = new List<string>();
            
            if ((modifiers & 0x0002) != 0) parts.Add("Ctrl");  // MOD_CONTROL
            if ((modifiers & 0x0001) != 0) parts.Add("Alt");   // MOD_ALT
            if ((modifiers & 0x0004) != 0) parts.Add("Shift"); // MOD_SHIFT
            if ((modifiers & 0x0008) != 0) parts.Add("Win");   // MOD_WIN
            
            // Convert virtual key code to character
            var keyChar = GetKeyDisplayName(key);
            if (!string.IsNullOrEmpty(keyChar))
            {
                parts.Add(keyChar);
            }
            
            return string.Join("+", parts);
        }

        private static string GetKeyDisplayName(uint virtualKey)
        {
            // Common virtual key codes
            return virtualKey switch
            {
                0x41 => "A", 0x42 => "B", 0x43 => "C", 0x44 => "D", 0x45 => "E", 0x46 => "F",
                0x47 => "G", 0x48 => "H", 0x49 => "I", 0x4A => "J", 0x4B => "K", 0x4C => "L",
                0x4D => "M", 0x4E => "N", 0x4F => "O", 0x50 => "P", 0x51 => "Q", 0x52 => "R",
                0x53 => "S", 0x54 => "T", 0x55 => "U", 0x56 => "V", 0x57 => "W", 0x58 => "X",
                0x59 => "Y", 0x5A => "Z",
                0x30 => "0", 0x31 => "1", 0x32 => "2", 0x33 => "3", 0x34 => "4",
                0x35 => "5", 0x36 => "6", 0x37 => "7", 0x38 => "8", 0x39 => "9",
                0x70 => "F1", 0x71 => "F2", 0x72 => "F3", 0x73 => "F4", 0x74 => "F5", 0x75 => "F6",
                0x76 => "F7", 0x77 => "F8", 0x78 => "F9", 0x79 => "F10", 0x7A => "F11", 0x7B => "F12",
                _ => $"Key{virtualKey:X2}"
            };
        }
    }
}