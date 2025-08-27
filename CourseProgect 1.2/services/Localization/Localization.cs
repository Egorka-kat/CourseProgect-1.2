using CourseProgect_1._2.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.IsolatedStorage;

namespace CourseProgect_1._2.services.Localization
{

    class ALocalization : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action LanguageChanged;

        public ObservableCollection<string> language = new ObservableCollection<string>();
        public ObservableCollection<Dictionary<string, string>> Translations = new ObservableCollection<Dictionary<string, string>>();

        private string _currentLanguage;
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnPropertyChanged(nameof(CurrentLanguage));
                    OnPropertyChanged(nameof(CurrentTranslations));
                    LanguageChanged?.Invoke();
                }
            }
        }

        public Dictionary<string, string> CurrentTranslations
        {
            get
            {
                if (CurrentLanguage == null || !language.Contains(CurrentLanguage))
                    return Translations.FirstOrDefault() ?? new Dictionary<string, string>();

                int index = language.IndexOf(CurrentLanguage);
                return Translations[index];
            }
        }

        public string this[string key]
        {
            get
            {
                if (CurrentTranslations.TryGetValue(key, out string value))
                    return value;
                return key;
            }
        }

        public ALocalization()
        {
            var directoryInfo = new DirectoryInfo(@"Language\LanguageFiles");
            foreach (var directory in directoryInfo.GetFiles())
            {
                language.Add(Path.GetFileNameWithoutExtension(directory.FullName));
                Translations.Add(new LocalizationScanner().ScanAndIntegrate(directory.FullName));
            }

            if (!LanguageFileExists("LanguageCustomization.config"))
            {
                if (language.Count > 0)
                {
                    WriteToIsolatedStorage("LanguageCustomization.config", language[0]);
                    CurrentLanguage = language[0];
                }
            }
            else
            {
                CurrentLanguage = ReadFromIsolatedStorage("LanguageCustomization.config");
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool LanguageFileExists(string fileName)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    return storage.FileExists(fileName);
                }
            }
            catch
            {
                return false;
            }
        }

        public void WriteToIsolatedStorage(string fileName, string content)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(
                    fileName,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    storage))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи: {ex.Message}");
            }
        }

        public string ReadFromIsolatedStorage(string fileName)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (storage.FileExists(fileName))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(
                            fileName,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read,
                            4096,
                            storage))
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения: {ex.Message}");
            }

            return null;
        }

        public void OverwriteFile(string fileName, string newContent)
        {
            WriteToIsolatedStorage(fileName, newContent); 
        }
    }
}
