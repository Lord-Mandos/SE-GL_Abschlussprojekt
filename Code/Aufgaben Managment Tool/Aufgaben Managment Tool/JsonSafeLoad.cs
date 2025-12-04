using System.Text.Json;

namespace Aufgaben_Managment_Tool
{
    internal class JsonSafeLoad<T>
    {
        public static List<T> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
        }
        public static void Save(string filePath, List<T> data)
        {
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
    }
}
