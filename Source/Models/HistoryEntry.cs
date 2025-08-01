using System.Text.Json.Serialization;

namespace SnapText.Models
{
    public class HistoryEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [JsonPropertyName("extractedText")]
        public string ExtractedText { get; set; } = string.Empty;

        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; } = string.Empty;

        [JsonPropertyName("thumbnailPath")]
        public string ThumbnailPath { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [JsonPropertyName("category")]
        public string Category { get; set; } = "General";

        [JsonPropertyName("characterCount")]
        public int CharacterCount { get; set; }

        public HistoryEntry()
        {
        }

        public HistoryEntry(string extractedText, string imagePath, string thumbnailPath)
        {
            ExtractedText = extractedText;
            ImagePath = imagePath;
            ThumbnailPath = thumbnailPath;
            CharacterCount = extractedText?.Length ?? 0;
        }
    }
}