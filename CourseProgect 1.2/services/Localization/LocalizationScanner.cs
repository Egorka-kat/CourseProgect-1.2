using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CourseProgect_1._2.services.Localization
{
    public class LocalizationScanner
    {
        public Dictionary<string, string> ScanAndIntegrate(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentException("Путь к файлу не может быть пустым", nameof(jsonFilePath));

            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException($"Файл не найден: {jsonFilePath}");

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var localizationData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

                if (localizationData == null || localizationData.Count == 0)
                    throw new InvalidOperationException("Не удалось десериализовать JSON файл или файл пуст");

                var firstLanguage = localizationData.First().Value;
                return new Dictionary<string, string>(firstLanguage);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Ошибка парсинга JSON: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сканировании файла: {ex.Message}", ex);
            }
        }
    }
}
