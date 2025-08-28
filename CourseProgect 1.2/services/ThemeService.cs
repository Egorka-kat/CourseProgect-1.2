using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace CourseProgect_1._2.Services
{
    public enum Theme
    {
        Light,
        Dark
    }

    public static class ThemeService
    {
        private const string ThemeKey = "CurrentTheme";

        public static Theme CurrentTheme { get; private set; } = Theme.Light;

        // Событие для уведомления об изменении темы
        public static event Action ThemeChanged;
        public static void Initialize()
        {
            CurrentTheme = LoadTheme();
            ApplyTheme(CurrentTheme, true);
        }

        public static void ApplyTheme(Theme theme, bool isInitial = false)
        {
            if (CurrentTheme == theme && !isInitial) return;

            CurrentTheme = theme;

            var app = Application.Current;
            if (app == null) return;

            try
            {
                // Создаем новый словарь ресурсов
                var newResources = new ResourceDictionary();

                // Добавляем выбранную тему
                var themeUri = theme == Theme.Light
                    ? new Uri("Themes/LightTheme.xaml", UriKind.Relative)
                    : new Uri("Themes/DarkTheme.xaml", UriKind.Relative);

                var themeDict = new ResourceDictionary { Source = themeUri };
                newResources.MergedDictionaries.Add(themeDict);

                // Сохраняем старые ресурсы (конвертеры)
                if (app.Resources != null)
                {
                    foreach (var key in app.Resources.Keys)
                    {
                        if (app.Resources[key] is ResourceDictionary existingDict)
                        {
                            newResources.MergedDictionaries.Add(existingDict);
                        }
                        else
                        {
                            newResources.Add(key, app.Resources[key]);
                        }
                    }
                }

                // Применяем новые ресурсы
                app.Resources = newResources;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading theme: {ex.Message}");

                // Fallback - пробуем загрузить напрямую
                LoadThemeFallback(theme);
            }

            if (!isInitial)
            {
                SaveTheme(theme);
                ThemeChanged?.Invoke();
            }
        }
        private static void LoadThemeFallback(Theme theme)
        {
            try
            {
                string themeName = theme == Theme.Light ? "LightTheme.xaml" : "DarkTheme.xaml";
                string themePath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Themes",
                    themeName);

                if (System.IO.File.Exists(themePath))
                {
                    using (var stream = System.IO.File.OpenRead(themePath))
                    {
                        var themeDict = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(stream);
                        Application.Current.Resources = themeDict;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fallback theme loading failed: {ex.Message}");
            }
        }

        private static Theme LoadTheme()
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (storage.FileExists(ThemeKey))
                    {
                        using (var stream = new IsolatedStorageFileStream(ThemeKey, FileMode.Open, storage))
                        using (var reader = new System.IO.StreamReader(stream))
                        {
                            string themeName = reader.ReadToEnd();
                            if (Enum.TryParse<Theme>(themeName, out Theme theme))
                            {
                                return theme;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки загрузки
            }
            return Theme.Light;
        }

        private static void SaveTheme(Theme theme)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                using (var stream = new IsolatedStorageFileStream(ThemeKey, FileMode.Create, storage))
                using (var writer = new System.IO.StreamWriter(stream))
                {
                    writer.Write(theme.ToString());
                }
            }
            catch
            {
                // Игнорируем ошибки сохранения
            }
        }

    }
}