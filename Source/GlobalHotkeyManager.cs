using System.Runtime.InteropServices;

namespace SnapText
{
    public class GlobalHotkeyManager : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;
        public const int MOD_WIN = 0x0008;
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly IntPtr _windowHandle;
        private readonly int _hotkeyId;
        private bool _isRegistered;
        private uint _currentModifiers;
        private uint _currentKey;

        public event EventHandler? HotkeyPressed;

        public GlobalHotkeyManager(IntPtr windowHandle, int hotkeyId = 1)
        {
            _windowHandle = windowHandle;
            _hotkeyId = hotkeyId;
        }

        public bool RegisterHotkey()
        {
            if (_isRegistered)
                return true;

            // Get hotkey configuration from registry
            var (modifiers, key) = RegistryHelper.GetHotkey();
            _currentModifiers = modifiers;
            _currentKey = key;

            _isRegistered = RegisterHotKey(_windowHandle, _hotkeyId, modifiers, key);
            return _isRegistered;
        }

        public bool RegisterCustomHotkey(uint modifiers, uint key)
        {
            // Unregister current hotkey if registered
            if (_isRegistered)
            {
                UnregisterHotkey();
            }

            _currentModifiers = modifiers;
            _currentKey = key;

            _isRegistered = RegisterHotKey(_windowHandle, _hotkeyId, modifiers, key);
            return _isRegistered;
        }

        public (uint modifiers, uint key) GetCurrentHotkey()
        {
            return (_currentModifiers, _currentKey);
        }

        public void UnregisterHotkey()
        {
            if (_isRegistered)
            {
                UnregisterHotKey(_windowHandle, _hotkeyId);
                _isRegistered = false;
            }
        }

        public void HandleHotkeyMessage(int hotkeyId)
        {
            if (hotkeyId == _hotkeyId)
            {
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            UnregisterHotkey();
        }
    }
}