using System.Drawing;
using SnapText.Data;
using SnapText.Models;
using SnapText.Services;

namespace SnapText
{
    public partial class HistoryForm : Form
    {
        private readonly IHistoryRepository _historyRepository;
        private List<HistoryEntry> _currentEntries = new List<HistoryEntry>();
        private HistoryEntry? _selectedEntry;

        public HistoryForm()
        {
            InitializeComponent();
            _historyRepository = new JsonHistoryRepository();
            SetupDataGridView();
            SetupEventHandlers();
            LoadHistory();
            LoadTags();
        }

        private void SetupDataGridView()
        {
            historyDataGridView.AutoGenerateColumns = false;
            historyDataGridView.AllowUserToResizeRows = false;
            historyDataGridView.RowHeadersVisible = false;
            historyDataGridView.RowTemplate.Height = 60;

            var thumbnailColumn = new DataGridViewImageColumn
            {
                Name = "Thumbnail",
                HeaderText = "Preview",
                Width = 70,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                DataPropertyName = "ThumbnailImage"
            };
            historyDataGridView.Columns.Add(thumbnailColumn);

            var timestampColumn = new DataGridViewTextBoxColumn
            {
                Name = "Timestamp",
                HeaderText = "Date/Time",
                Width = 130,
                DataPropertyName = "TimestampString"
            };
            historyDataGridView.Columns.Add(timestampColumn);

            var textColumn = new DataGridViewTextBoxColumn
            {
                Name = "ExtractedText",
                HeaderText = "Text Preview",
                Width = 300,
                DataPropertyName = "TextPreview"
            };
            historyDataGridView.Columns.Add(textColumn);

            var tagsColumn = new DataGridViewTextBoxColumn
            {
                Name = "Tags",
                HeaderText = "Tags",
                Width = 150,
                DataPropertyName = "TagsString"
            };
            historyDataGridView.Columns.Add(tagsColumn);

            var categoryColumn = new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Category",
                Width = 100,
                DataPropertyName = "Category"
            };
            historyDataGridView.Columns.Add(categoryColumn);

            var charCountColumn = new DataGridViewTextBoxColumn
            {
                Name = "CharacterCount",
                HeaderText = "Characters",
                Width = 80,
                DataPropertyName = "CharacterCount"
            };
            historyDataGridView.Columns.Add(charCountColumn);

            historyDataGridView.DefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);
            historyDataGridView.DefaultCellStyle.ForeColor = Color.White;
            historyDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            historyDataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            historyDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            historyDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            historyDataGridView.EnableHeadersVisualStyles = false;
        }

        private void SetupEventHandlers()
        {
            searchButton.Click += OnSearchButtonClick;
            clearSearchButton.Click += OnClearSearchButtonClick;
            searchTextBox.KeyDown += OnSearchTextBoxKeyDown;
            
            tagsComboBox.SelectedIndexChanged += OnTagsComboBoxSelectedIndexChanged;
            addTagButton.Click += OnAddTagButtonClick;
            newTagTextBox.KeyDown += OnNewTagTextBoxKeyDown;
            
            historyDataGridView.SelectionChanged += OnHistoryDataGridViewSelectionChanged;
            historyDataGridView.CellDoubleClick += OnHistoryDataGridViewCellDoubleClick;
            
            exportButton.Click += OnExportButtonClick;
            deleteButton.Click += OnDeleteButtonClick;
            clearAllButton.Click += OnClearAllButtonClick;
        }

        private async void LoadHistory()
        {
            try
            {
                var entries = await _historyRepository.GetAllAsync();
                DisplayEntries(entries);
                statusLabel.Text = $"Loaded {entries.Count} entries";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error loading history: {ex.Message}";
            }
        }

        private void DisplayEntries(List<HistoryEntry> entries)
        {
            _currentEntries = entries;
            var displayEntries = new List<HistoryDisplayEntry>();

            foreach (var entry in entries)
            {
                var displayEntry = new HistoryDisplayEntry(entry);
                displayEntries.Add(displayEntry);
            }

            historyDataGridView.DataSource = displayEntries;
        }

        private async void LoadTags()
        {
            try
            {
                var tags = await _historyRepository.GetAllTagsAsync();
                tagsComboBox.Items.Clear();
                tagsComboBox.Items.Add("All Tags");
                foreach (var tag in tags)
                {
                    tagsComboBox.Items.Add(tag);
                }
                tagsComboBox.SelectedIndex = 0;
            }
            catch
            {
                tagsComboBox.Items.Clear();
                tagsComboBox.Items.Add("All Tags");
                tagsComboBox.SelectedIndex = 0;
            }
        }

        private async void OnSearchButtonClick(object? sender, EventArgs e)
        {
            await PerformSearch();
        }

        private async void OnClearSearchButtonClick(object? sender, EventArgs e)
        {
            searchTextBox.Text = "";
            tagsComboBox.SelectedIndex = 0;
            LoadHistory();
        }

        private async void OnSearchTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await PerformSearch();
            }
        }

        private async Task PerformSearch()
        {
            var searchTerm = searchTextBox.Text.Trim();
            
            try
            {
                List<HistoryEntry> results;
                
                if (string.IsNullOrEmpty(searchTerm))
                {
                    results = await _historyRepository.GetAllAsync();
                }
                else
                {
                    results = await _historyRepository.SearchAsync(searchTerm);
                }
                
                DisplayEntries(results);
                statusLabel.Text = $"Found {results.Count} entries";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Search error: {ex.Message}";
            }
        }

        private async void OnTagsComboBoxSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tagsComboBox.SelectedIndex <= 0) return;
            
            var selectedTag = tagsComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTag)) return;
            
            try
            {
                var results = await _historyRepository.GetByTagAsync(selectedTag);
                DisplayEntries(results);
                statusLabel.Text = $"Found {results.Count} entries with tag '{selectedTag}'";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Filter error: {ex.Message}";
            }
        }

        private async void OnAddTagButtonClick(object? sender, EventArgs e)
        {
            await AddTagToSelectedEntry();
        }

        private async void OnNewTagTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await AddTagToSelectedEntry();
            }
        }

        private async Task AddTagToSelectedEntry()
        {
            if (_selectedEntry == null)
            {
                statusLabel.Text = "Please select an entry first";
                return;
            }

            var newTag = newTagTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newTag))
            {
                statusLabel.Text = "Please enter a tag";
                return;
            }

            try
            {
                if (!_selectedEntry.Tags.Contains(newTag, StringComparer.OrdinalIgnoreCase))
                {
                    _selectedEntry.Tags.Add(newTag);
                    await _historyRepository.UpdateAsync(_selectedEntry);
                    
                    newTagTextBox.Text = "";
                    LoadTags();
                    LoadHistory();
                    statusLabel.Text = $"Tag '{newTag}' added";
                }
                else
                {
                    statusLabel.Text = "Tag already exists";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error adding tag: {ex.Message}";
            }
        }

        private void OnHistoryDataGridViewSelectionChanged(object? sender, EventArgs e)
        {
            if (historyDataGridView.SelectedRows.Count > 0)
            {
                var selectedIndex = historyDataGridView.SelectedRows[0].Index;
                if (selectedIndex >= 0 && selectedIndex < _currentEntries.Count)
                {
                    _selectedEntry = _currentEntries[selectedIndex];
                }
            }
            else
            {
                _selectedEntry = null;
            }
        }

        private void OnHistoryDataGridViewCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (_selectedEntry != null && !string.IsNullOrEmpty(_selectedEntry.ExtractedText))
            {
                try
                {
                    Clipboard.SetText(_selectedEntry.ExtractedText);
                    statusLabel.Text = "Text copied to clipboard";
                }
                catch
                {
                    statusLabel.Text = "Failed to copy to clipboard";
                }
            }
        }

        private async void OnExportButtonClick(object? sender, EventArgs e)
        {
            if (!_currentEntries.Any())
            {
                statusLabel.Text = "No entries to export";
                return;
            }

            using var saveDialog = new SaveFileDialog
            {
                Title = "Export History",
                Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json",
                FilterIndex = 1,
                FileName = $"SnapText_History_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    bool success = false;
                    var extension = Path.GetExtension(saveDialog.FileName).ToLowerInvariant();
                    
                    switch (extension)
                    {
                        case ".csv":
                            success = await HistoryExporter.ExportToCsvAsync(_currentEntries, saveDialog.FileName);
                            break;
                        case ".txt":
                            success = await HistoryExporter.ExportToTxtAsync(_currentEntries, saveDialog.FileName);
                            break;
                        case ".json":
                            success = await HistoryExporter.ExportToJsonAsync(_currentEntries, saveDialog.FileName);
                            break;
                    }

                    statusLabel.Text = success ? "Export completed successfully" : "Export failed";
                }
                catch (Exception ex)
                {
                    statusLabel.Text = $"Export error: {ex.Message}";
                }
            }
        }

        private async void OnDeleteButtonClick(object? sender, EventArgs e)
        {
            if (_selectedEntry == null)
            {
                statusLabel.Text = "Please select an entry to delete";
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to delete this entry? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _historyRepository.DeleteAsync(_selectedEntry.Id);
                    LoadHistory();
                    LoadTags();
                    statusLabel.Text = "Entry deleted";
                    _selectedEntry = null;
                }
                catch (Exception ex)
                {
                    statusLabel.Text = $"Delete error: {ex.Message}";
                }
            }
        }

        private async void OnClearAllButtonClick(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete ALL history entries? This action cannot be undone.",
                "Confirm Clear All",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _historyRepository.DeleteAllAsync();
                    LoadHistory();
                    LoadTags();
                    statusLabel.Text = "All entries cleared";
                    _selectedEntry = null;
                }
                catch (Exception ex)
                {
                    statusLabel.Text = $"Clear error: {ex.Message}";
                }
            }
        }
    }

    public class HistoryDisplayEntry
    {
        public Image? ThumbnailImage { get; set; }
        public string TimestampString { get; set; }
        public string TextPreview { get; set; }
        public string TagsString { get; set; }
        public string Category { get; set; }
        public int CharacterCount { get; set; }

        public HistoryDisplayEntry(HistoryEntry entry)
        {
            TimestampString = entry.Timestamp.ToString("yyyy-MM-dd HH:mm");
            TextPreview = entry.ExtractedText.Length > 50 
                ? entry.ExtractedText.Substring(0, 50) + "..."
                : entry.ExtractedText;
            TagsString = string.Join(", ", entry.Tags);
            Category = entry.Category;
            CharacterCount = entry.CharacterCount;

            if (!string.IsNullOrEmpty(entry.ThumbnailPath) && File.Exists(entry.ThumbnailPath))
            {
                try
                {
                    ThumbnailImage = Image.FromFile(entry.ThumbnailPath);
                }
                catch
                {
                    ThumbnailImage = null;
                }
            }
        }
    }
}