using System.Text.Json;

namespace Kosuma.Services
{
    public static class NameProvider
    {
        public static List<string> LoadNames()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "names.json");
            var jsonData = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<string>>(jsonData);
        }
    }
}