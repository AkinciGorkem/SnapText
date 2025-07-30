using IronOcr;

namespace SnapText
{
    public partial class MainForm : Form
    {
        private const int WM_HOTKEY = 0x0312;
        private GlobalHotkeyManager? _hotkeyManager;
        private NotifyIcon? _notifyIcon;
        private IronTesseract? _ocrEngine;

        public MainForm()
        {
            InitializeComponent();
            InitializeHotkeys();
            InitializeSystemTray();
            InitializeOcr();
            InitializeEventHandlers();
            UpdateHotkeyDisplay();
        }

        private void InitializeHotkeys()
        {
            _hotkeyManager = new GlobalHotkeyManager(this.Handle);
            _hotkeyManager.HotkeyPressed += OnHotkeyPressed;
        }

        private void InitializeSystemTray()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = SystemIcons.Application;
            _notifyIcon.Text = "SnapText - OCR Screenshot Tool";
            _notifyIcon.Visible = true;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, OnShowClick);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Settings", null, OnSettingsMenuClick);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, OnExitClick);
            _notifyIcon.ContextMenuStrip = contextMenu;

            _notifyIcon.DoubleClick += OnNotifyIconDoubleClick;
        }

        private void InitializeOcr()
        {
            var apiKey = RegistryHelper.GetApiKey();
            if (!string.IsNullOrEmpty(apiKey))
            {
                try
                {
                    IronOcr.License.LicenseKey = apiKey;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Invalid IronOCR API key: {ex.Message}\n\nUsing default license.", "API Key Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            
            _ocrEngine = new IronTesseract();
            _ocrEngine.Language = OcrLanguage.English;
        }

        private void InitializeEventHandlers()
        {
            captureButton.Click += (s, e) => CaptureScreenshot();
            copyButton.Click += OnCopyButtonClick;
            clearButton.Click += OnClearButtonClick;
            settingsButton.Click += OnSettingsButtonClick;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                _hotkeyManager?.HandleHotkeyMessage(m.WParam.ToInt32());
            }
            base.WndProc(ref m);
        }

        private void OnHotkeyPressed(object? sender, EventArgs e)
        {
            CaptureScreenshot();
        }

        private void CaptureScreenshot()
        {
            try
            {
                this.Hide();
                
                using (var overlayForm = new ScreenshotOverlayForm())
                {
                    overlayForm.ScreenshotCaptured += OnScreenshotCaptured;
                    overlayForm.ScreenshotCancelled += OnScreenshotCancelled;
                    overlayForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during screenshot capture: {ex.Message}", "SnapText Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        private async void OnScreenshotCaptured(object? sender, ScreenshotCapturedEventArgs e)
        {
            this.Show();
            await ProcessOcrAsync(e.FilePath);
        }

        private async Task ProcessOcrAsync(string imagePath)
        {
            try
            {
                statusLabel.Text = "Processing OCR...";
                ocrResultsTextBox.Text = "Processing...";
                
                await Task.Run(() =>
                {
                    if (_ocrEngine != null)
                    {
                        using var ocrInput = new OcrInput();
                        ocrInput.LoadImage(imagePath);
                        var result = _ocrEngine.Read(ocrInput);
                        
                        this.Invoke(() =>
                        {
                            var cleanedText = CleanOcrText(result.Text);
                            var displayText = string.IsNullOrWhiteSpace(cleanedText) ? "No text detected in the image." : cleanedText;
                            var characterCount = string.IsNullOrWhiteSpace(cleanedText) ? 0 : cleanedText.Length;
                            
                            ocrResultsTextBox.Text = displayText;
                            statusLabel.Text = characterCount > 0 
                                ? $"OCR completed - {characterCount} characters extracted"
                                : "OCR completed - no text found";
                        });
                    }
                });
                
                _notifyIcon?.ShowBalloonTip(3000, "SnapText", "OCR processing completed!", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ocrResultsTextBox.Text = $"OCR Error: {ex.Message}";
                statusLabel.Text = "OCR failed";
                MessageBox.Show($"OCR processing failed: {ex.Message}", "OCR Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnScreenshotCancelled(object? sender, EventArgs e)
        {
            this.Show();
        }

        private string CleanOcrText(string? rawText)
        {
            if (string.IsNullOrEmpty(rawText))
                return string.Empty;

            // Remove invisible characters and normalize whitespace
            var cleaned = System.Text.RegularExpressions.Regex.Replace(rawText, @"[\u0000-\u001F\u007F-\u009F]", "");
            
            // Normalize line endings and excessive whitespace
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\r\n|\r|\n", "\r\n");
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"[ \t]+", " ");
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"(\r\n\s*){3,}", "\r\n\r\n");
            
            return cleaned.Trim();
        }

        private async Task<bool> TrySetClipboardTextAsync(string text)
        {
            const int maxRetries = 3;
            const int delayMs = 100;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(text))
                        return false;

                    // Ensure we're on the UI thread for clipboard operations
                    if (InvokeRequired)
                    {
                        return await Task.FromResult(false);
                    }

                    Clipboard.Clear();
                    await Task.Delay(50); // Small delay after clear
                    Clipboard.SetText(text, TextDataFormat.UnicodeText);
                    
                    // Verify the clipboard was set correctly
                    await Task.Delay(50);
                    var clipboardText = Clipboard.GetText();
                    return clipboardText == text;
                }
                catch (Exception)
                {
                    if (i < maxRetries - 1)
                    {
                        await Task.Delay(delayMs * (i + 1)); // Exponential backoff
                    }
                }
            }
            
            return false;
        }

        public void UpdateHotkeyDisplay()
        {
            var hotkeyText = RegistryHelper.GetHotkeyDisplayString();
            captureButton.Text = $"Capture ({hotkeyText})";
            
            // Update initial text in OCR results box
            if (ocrResultsTextBox.Text == "OCR results will appear here...\r\n\r\nPress hotkey to capture a screenshot")
            {
                ocrResultsTextBox.Text = $"OCR results will appear here...\r\n\r\nPress {hotkeyText} to capture a screenshot";
            }
            
            // Update tooltip if needed
            if (_notifyIcon != null)
            {
                _notifyIcon.Text = $"SnapText - OCR Screenshot Tool ({hotkeyText})";
            }
        }

        private void OnShowClick(object? sender, EventArgs e)
        {
            ShowForm();
        }

        private void OnExitClick(object? sender, EventArgs e)
        {
            ExitApplication();
        }

        private async void OnCopyButtonClick(object? sender, EventArgs e)
        {
            var textToCopy = ocrResultsTextBox.Text?.Trim();
            var hotkeyText = RegistryHelper.GetHotkeyDisplayString();
            var defaultText = $"OCR results will appear here...\r\n\r\nPress {hotkeyText} to capture a screenshot";
            
            if (string.IsNullOrWhiteSpace(textToCopy) || 
                textToCopy == defaultText ||
                textToCopy == "No text detected in the image.")
            {
                statusLabel.Text = "No text to copy";
                return;
            }

            var success = await TrySetClipboardTextAsync(textToCopy);
            if (success)
            {
                statusLabel.Text = "Text copied to clipboard";
                _notifyIcon?.ShowBalloonTip(2000, "SnapText", "Text copied to clipboard!", ToolTipIcon.Info);
            }
            else
            {
                statusLabel.Text = "Failed to copy to clipboard";
                MessageBox.Show("Failed to copy text to clipboard. The clipboard might be in use by another application.\n\nTry again in a moment.", 
                    "Clipboard Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnClearButtonClick(object? sender, EventArgs e)
        {
            var hotkeyText = RegistryHelper.GetHotkeyDisplayString();
            ocrResultsTextBox.Text = $"OCR results will appear here...\r\n\r\nPress {hotkeyText} to capture a screenshot";
            statusLabel.Text = "Ready";
        }


        private void OnSettingsButtonClick(object? sender, EventArgs e)
        {
            using var settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void OnSettingsMenuClick(object? sender, EventArgs e)
        {
            this.Show();
            this.BringToFront();
            using var settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void OnNotifyIconDoubleClick(object? sender, EventArgs e)
        {
            ShowForm();
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private void ExitApplication()
        {
            _ocrEngine = null;
            _notifyIcon?.Dispose();
            _hotkeyManager?.Dispose();
            Application.Exit();
        }

        protected override void SetVisibleCore(bool value)
        {
            if (_notifyIcon == null)
            {
                base.SetVisibleCore(value);
                return;
            }

            base.SetVisibleCore(value);
            if (value && _hotkeyManager != null)
            {
                _hotkeyManager.RegisterHotkey();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                if (_notifyIcon != null)
                {
                    var hotkeyText = RegistryHelper.GetHotkeyDisplayString();
                    _notifyIcon.ShowBalloonTip(2000, "SnapText", $"Application minimized to tray. Use {hotkeyText} for screenshots.", ToolTipIcon.Info);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                if (_notifyIcon != null)
                {
                    _notifyIcon.ShowBalloonTip(2000, "SnapText", "Application minimized to tray. Right-click the tray icon to exit.", ToolTipIcon.Info);
                }
            }
            else
            {
                ExitApplication();
            }
        }
    }
}