namespace SnapText
{
    public partial class SettingsForm : Form
    {
        private uint _currentModifiers = 0;
        private uint _currentKey = 0;
        private bool _isCapturingHotkey = false;

        public SettingsForm()
        {
            InitializeComponent();
            LoadCurrentApiKey();
            LoadCurrentHotkey();
            SetupHotkeyCapture();
        }

        private void LoadCurrentApiKey()
        {
            var apiKey = RegistryHelper.GetApiKey();
            if (!string.IsNullOrEmpty(apiKey))
            {
                apiKeyTextBox.Text = apiKey;
                statusLabel.Text = "Custom API key loaded";
                statusLabel.ForeColor = Color.FromArgb(46, 204, 113);
            }
            else
            {
                statusLabel.Text = "Using default IronOCR license";
                statusLabel.ForeColor = Color.FromArgb(150, 150, 150);
            }
        }

        private void OnSaveButtonClick(object? sender, EventArgs e)
        {
            bool apiKeySuccess = SaveApiKey();
            bool hotkeySuccess = SaveHotkey();

            if (apiKeySuccess && hotkeySuccess)
            {
                statusLabel.Text = "Settings saved successfully";
                statusLabel.ForeColor = Color.FromArgb(46, 204, 113);
                
                string restartMessage = "Settings saved successfully!";
                if (!string.IsNullOrEmpty(apiKeyTextBox.Text.Trim()))
                {
                    restartMessage += " Restart the application for API key changes to take effect.";
                }
                
                MessageBox.Show(restartMessage, "Settings Saved", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                // Update main form hotkey display
                UpdateMainFormHotkey();
            }
            else if (!apiKeySuccess && !hotkeySuccess)
            {
                statusLabel.Text = "Failed to save settings";
                statusLabel.ForeColor = Color.FromArgb(232, 65, 24);
            }
            else if (!apiKeySuccess)
            {
                statusLabel.Text = "Hotkey saved, API key failed";
                statusLabel.ForeColor = Color.FromArgb(232, 65, 24);
            }
            else
            {
                statusLabel.Text = "API key saved, hotkey failed";
                statusLabel.ForeColor = Color.FromArgb(232, 65, 24);
            }
        }

        private bool SaveApiKey()
        {
            var apiKey = apiKeyTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(apiKey))
            {
                return RegistryHelper.ClearApiKey();
            }

            if (ValidateApiKey(apiKey))
            {
                return RegistryHelper.SetApiKey(apiKey);
            }
            else
            {
                MessageBox.Show("Please enter a valid IronOCR API key.", "Invalid API Key", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private bool SaveHotkey()
        {
            if (_currentModifiers == 0 || _currentKey == 0)
            {
                // Use current registry values if no new hotkey was set
                return true;
            }

            return RegistryHelper.SetHotkey(_currentModifiers, _currentKey);
        }

        private void UpdateMainFormHotkey()
        {
            // Find the main form and update its hotkey display
            var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            if (mainForm != null)
            {
                mainForm.UpdateHotkeyDisplay();
                
                // Re-register the hotkey with the new combination
                var hotkeyManager = mainForm.GetType()
                    .GetField("_hotkeyManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(mainForm) as GlobalHotkeyManager;
                    
                if (hotkeyManager != null)
                {
                    hotkeyManager.RegisterCustomHotkey(_currentModifiers, _currentKey);
                }
            }
        }

        private void OnCancelButtonClick(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void OnClearButtonClick(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("This will remove your custom API key and use the default IronOCR license.\n\nAre you sure?", 
                "Clear API Key", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                apiKeyTextBox.Clear();
                if (RegistryHelper.ClearApiKey())
                {
                    statusLabel.Text = "API key cleared - using default license";
                    statusLabel.ForeColor = Color.FromArgb(150, 150, 150);
                    MessageBox.Show("API key cleared successfully!", "Settings Updated", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private bool ValidateApiKey(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return false;

            if (apiKey.Length < 10)
                return false;

            return true;
        }

        private void LoadCurrentHotkey()
        {
            var (modifiers, key) = RegistryHelper.GetHotkey();
            _currentModifiers = modifiers;
            _currentKey = key;
            hotkeyTextBox.Text = RegistryHelper.GetHotkeyDisplayString();
        }

        private void SetupHotkeyCapture()
        {
            hotkeyTextBox.Enter += (s, e) => 
            {
                _isCapturingHotkey = true;
                hotkeyTextBox.Text = "Press key combination...";
                hotkeyTextBox.ForeColor = Color.Yellow;
            };

            hotkeyTextBox.Leave += (s, e) => 
            {
                _isCapturingHotkey = false;
                hotkeyTextBox.ForeColor = Color.White;
                if (string.IsNullOrEmpty(hotkeyTextBox.Text) || hotkeyTextBox.Text == "Press key combination...")
                {
                    hotkeyTextBox.Text = RegistryHelper.GetHotkeyDisplayString();
                }
            };

            hotkeyTextBox.KeyDown += OnHotkeyTextBoxKeyDown;
        }

        private void OnHotkeyTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (!_isCapturingHotkey) return;

            e.Handled = true;
            e.SuppressKeyPress = true;

            // Don't capture modifier keys alone
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || 
                e.KeyCode == Keys.Alt || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return;
            }

            // Build modifiers
            uint modifiers = 0;
            if (e.Control) modifiers |= GlobalHotkeyManager.MOD_CONTROL;
            if (e.Shift) modifiers |= GlobalHotkeyManager.MOD_SHIFT;
            if (e.Alt) modifiers |= GlobalHotkeyManager.MOD_ALT;

            // Require at least one modifier
            if (modifiers == 0)
            {
                hotkeyTextBox.Text = "Hotkey must include Ctrl, Shift, or Alt";
                hotkeyTextBox.ForeColor = Color.FromArgb(232, 65, 24);
                return;
            }

            uint vkCode = (uint)e.KeyCode;
            
            // Validate key
            if (!IsValidHotkeyKey(e.KeyCode))
            {
                hotkeyTextBox.Text = "Invalid key. Use letters, numbers, or function keys.";
                hotkeyTextBox.ForeColor = Color.FromArgb(232, 65, 24);
                return;
            }

            _currentModifiers = modifiers;
            _currentKey = vkCode;

            // Display the captured hotkey
            var displayString = GetHotkeyDisplayString(modifiers, vkCode);
            hotkeyTextBox.Text = displayString;
            hotkeyTextBox.ForeColor = Color.FromArgb(46, 204, 113);
            
            statusLabel.Text = $"New hotkey: {displayString}";
            statusLabel.ForeColor = Color.FromArgb(46, 204, 113);
        }

        private bool IsValidHotkeyKey(Keys key)
        {
            // Allow letters A-Z
            if (key >= Keys.A && key <= Keys.Z) return true;
            
            // Allow numbers 0-9
            if (key >= Keys.D0 && key <= Keys.D9) return true;
            
            // Allow function keys F1-F12
            if (key >= Keys.F1 && key <= Keys.F12) return true;
            
            return false;
        }

        private string GetHotkeyDisplayString(uint modifiers, uint key)
        {
            var parts = new List<string>();
            
            if ((modifiers & GlobalHotkeyManager.MOD_CONTROL) != 0) parts.Add("Ctrl");
            if ((modifiers & GlobalHotkeyManager.MOD_ALT) != 0) parts.Add("Alt");
            if ((modifiers & GlobalHotkeyManager.MOD_SHIFT) != 0) parts.Add("Shift");
            
            // Convert key to display name
            var keyName = ((Keys)key).ToString();
            parts.Add(keyName);
            
            return string.Join("+", parts);
        }
    }
}