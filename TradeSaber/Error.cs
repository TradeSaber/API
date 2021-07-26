using System.Text.Json.Serialization;

namespace TradeSaber
{
    public class Error
    {
        [JsonPropertyName("error")]
        public string? ErrorMessage { get; set; }

        public static object Create(string message)
        {
            string error = message;
            return new { error };
        }
    }
}