# SnapText

**SnapText** is a powerful Windows OCR (Optical Character Recognition) screenshot tool that instantly extracts text from any area of your screen. Perfect for digitizing documents, capturing quotes, extracting code snippets, or converting any visual text into editable content.

## Features

- **Lightning-Fast OCR** - Instant text recognition powered by IronOCR
- **Global Hotkeys** - Capture screenshots from anywhere with customizable shortcuts (default: Ctrl+Shift+S)
- **Precision Selection** - Select exactly the area you need with visual feedback overlay
- **Smart Clipboard** - Automatically copies extracted text with intelligent retry logic
- **System Tray Integration** - Runs quietly in the background, always ready when you need it
- **Customizable Settings** - Configure API keys, shortcuts, and preferences
- **Lightweight & Fast** - Minimal resource usage, maximum performance

## Quick Start

### Installation

1. **Download** the latest release from the [Releases](../../releases) page
2. **Extract** the ZIP file to your preferred location
3. **Run** `SnapText.exe` - no installation required!
4. The app will automatically minimize to the system tray and is ready to use

### API Key Configuration

SnapText uses IronOCR with a default license that covers basic usage. For extended commercial use:

1. Obtain an IronOCR license key from [Iron Software](https://ironsoftware.com/csharp/ocr/)
2. Open **Settings** in SnapText
3. Enter your API key in the **IronOCR API Key** field
4. Click **Save** and restart the application

### Dependencies

- **IronOcr** (v2025.7.19) - OCR functionality
- **Guna.UI2.WinForms** (v2.0.4.7) - Modern UI components
- **.NET 8.0** - Target framework

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## Credits

- [Iron Software](https://ironsoftware.com/) for the excellent IronOCR library
- [Guna UI](https://gunaui.com/) for the beautiful modern UI components
- The open-source community for inspiration and feedback
