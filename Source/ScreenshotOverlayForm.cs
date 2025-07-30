using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SnapText
{
    public partial class ScreenshotOverlayForm : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        private const int SRCCOPY = 0x00CC0020;

        private Bitmap? _desktopScreenshot;
        private bool _isSelecting;
        private Point _startPoint;
        private Point _endPoint;
        private Rectangle _selectionRectangle;

        public event EventHandler<ScreenshotCapturedEventArgs>? ScreenshotCaptured;
        public event EventHandler? ScreenshotCancelled;

        public ScreenshotOverlayForm()
        {
            InitializeComponent();
            SetupOverlay();
            CaptureDesktop();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(800, 600);
            Cursor = Cursors.Cross;
            FormBorderStyle = FormBorderStyle.None;
            Name = "ScreenshotOverlayForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            WindowState = FormWindowState.Maximized;

            ResumeLayout(false);
        }

        private void SetupOverlay()
        {
            Rectangle bounds = GetVirtualScreenBounds();
            this.Bounds = bounds;
            this.BackColor = Color.Black;
            this.Opacity = 0.3;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            
            KeyPreview = true;
            KeyDown += ScreenshotOverlayForm_KeyDown;
            MouseDown += ScreenshotOverlayForm_MouseDown;
            MouseMove += ScreenshotOverlayForm_MouseMove;
            MouseUp += ScreenshotOverlayForm_MouseUp;
            Paint += ScreenshotOverlayForm_Paint;
        }

        private Rectangle GetVirtualScreenBounds()
        {
            int left = int.MaxValue;
            int top = int.MaxValue;
            int right = int.MinValue;
            int bottom = int.MinValue;

            foreach (Screen screen in Screen.AllScreens)
            {
                left = Math.Min(left, screen.Bounds.Left);
                top = Math.Min(top, screen.Bounds.Top);
                right = Math.Max(right, screen.Bounds.Right);
                bottom = Math.Max(bottom, screen.Bounds.Bottom);
            }

            return new Rectangle(left, top, right - left, bottom - top);
        }

        private void CaptureDesktop()
        {
            try
            {
                Rectangle bounds = GetVirtualScreenBounds();
                _desktopScreenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(_desktopScreenshot))
                {
                    IntPtr desktopDC = GetDC(IntPtr.Zero);
                    IntPtr memoryDC = g.GetHdc();
                    
                    BitBlt(memoryDC, 0, 0, bounds.Width, bounds.Height, desktopDC, bounds.X, bounds.Y, SRCCOPY);
                    
                    g.ReleaseHdc(memoryDC);
                    ReleaseDC(IntPtr.Zero, desktopDC);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing desktop: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void ScreenshotOverlayForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ScreenshotCancelled?.Invoke(this, EventArgs.Empty);
                Close();
            }
        }

        private void ScreenshotOverlayForm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isSelecting = true;
                _startPoint = e.Location;
                _endPoint = e.Location;
                _selectionRectangle = new Rectangle();
                Invalidate();
            }
        }

        private void ScreenshotOverlayForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_isSelecting)
            {
                _endPoint = e.Location;
                UpdateSelectionRectangle();
                Invalidate();
            }
        }

        private void ScreenshotOverlayForm_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_isSelecting && e.Button == MouseButtons.Left)
            {
                _isSelecting = false;
                _endPoint = e.Location;
                UpdateSelectionRectangle();

                if (_selectionRectangle.Width > 5 && _selectionRectangle.Height > 5)
                {
                    CaptureSelectedArea();
                }
                else
                {
                    ScreenshotCancelled?.Invoke(this, EventArgs.Empty);
                }
                
                Close();
            }
        }

        private void UpdateSelectionRectangle()
        {
            int x = Math.Min(_startPoint.X, _endPoint.X);
            int y = Math.Min(_startPoint.Y, _endPoint.Y);
            int width = Math.Abs(_endPoint.X - _startPoint.X);
            int height = Math.Abs(_endPoint.Y - _startPoint.Y);

            _selectionRectangle = new Rectangle(x, y, width, height);
        }

        private void CaptureSelectedArea()
        {
            if (_desktopScreenshot == null || _selectionRectangle.IsEmpty)
                return;

            try
            {
                using (Bitmap selectedArea = new Bitmap(_selectionRectangle.Width, _selectionRectangle.Height))
                {
                    using (Graphics g = Graphics.FromImage(selectedArea))
                    {
                        g.DrawImage(_desktopScreenshot, 
                            new Rectangle(0, 0, _selectionRectangle.Width, _selectionRectangle.Height),
                            _selectionRectangle, GraphicsUnit.Pixel);
                    }

                    string savedPath = SaveImage(selectedArea);
                    ScreenshotCaptured?.Invoke(this, new ScreenshotCapturedEventArgs(savedPath, selectedArea));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing selected area: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SaveImage(Bitmap image)
        {
            string roamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnapText");
            Directory.CreateDirectory(roamingPath);

            string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string fullPath = Path.Combine(roamingPath, fileName);

            image.Save(fullPath, ImageFormat.Png);
            return fullPath;
        }

        private void ScreenshotOverlayForm_Paint(object? sender, PaintEventArgs e)
        {
            if (_desktopScreenshot != null)
            {
                e.Graphics.DrawImage(_desktopScreenshot, 0, 0);
            }

            if (_isSelecting && !_selectionRectangle.IsEmpty)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, _selectionRectangle);
                }

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    e.Graphics.FillRectangle(brush, _selectionRectangle);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _desktopScreenshot?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class ScreenshotCapturedEventArgs : EventArgs
    {
        public string FilePath { get; }
        public Bitmap Image { get; }

        public ScreenshotCapturedEventArgs(string filePath, Bitmap image)
        {
            FilePath = filePath;
            Image = image;
        }
    }
}