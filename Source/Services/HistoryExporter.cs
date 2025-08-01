using System.Text;
using System.Text.Json;
using SnapText.Models;

namespace SnapText.Services
{
    public static class HistoryExporter
    {
        public static async Task<bool> ExportToCsvAsync(List<HistoryEntry> entries, string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
                
                await writer.WriteLineAsync("Timestamp,Text,Tags,Category,Character Count,Image Path");
                
                foreach (var entry in entries.OrderByDescending(e => e.Timestamp))
                {
                    var timestamp = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                    var text = EscapeCsvField(entry.ExtractedText);
                    var tags = EscapeCsvField(string.Join("; ", entry.Tags));
                    var category = EscapeCsvField(entry.Category);
                    var charCount = entry.CharacterCount.ToString();
                    var imagePath = EscapeCsvField(entry.ImagePath);
                    
                    await writer.WriteLineAsync($"{timestamp},{text},{tags},{category},{charCount},{imagePath}");
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> ExportToTxtAsync(List<HistoryEntry> entries, string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
                
                await writer.WriteLineAsync("SnapText History Export");
                await writer.WriteLineAsync($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                await writer.WriteLineAsync($"Total Entries: {entries.Count}");
                await writer.WriteLineAsync(new string('=', 50));
                await writer.WriteLineAsync();
                
                foreach (var entry in entries.OrderByDescending(e => e.Timestamp))
                {
                    await writer.WriteLineAsync($"Date: {entry.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    await writer.WriteLineAsync($"Category: {entry.Category}");
                    
                    if (entry.Tags.Any())
                    {
                        await writer.WriteLineAsync($"Tags: {string.Join(", ", entry.Tags)}");
                    }
                    
                    await writer.WriteLineAsync($"Characters: {entry.CharacterCount}");
                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync("Text:");
                    await writer.WriteLineAsync(entry.ExtractedText);
                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync(new string('-', 30));
                    await writer.WriteLineAsync();
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> ExportToJsonAsync(List<HistoryEntry> entries, string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                var json = JsonSerializer.Serialize(entries.OrderByDescending(e => e.Timestamp), options);
                await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "\"\"";

            var needsQuotes = field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r');
            
            if (needsQuotes)
            {
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }
            
            return field;
        }
    }
}