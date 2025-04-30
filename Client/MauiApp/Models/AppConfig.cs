using Medflix.Utils;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Medflix.Models
{
    public class AppConfig
    {
        public static AppConfig Instance
        {
            get
            {
                if(_instance == null)
                    _instance = Load();

                return _instance;
            }
        }
        private static AppConfig _instance;
        private static string AppConfigFilePath => Path.Combine(FileSystem.Current.AppDataDirectory, Consts.AppCongifFileName);

        private AppConfigData Data;
        private class AppConfigData
        {
            public string MedflixServiceAddress { get; set; }
            public string AppIdentifier { get; set; }

            public AppConfigData()
            {

            }
        }

        public string MedflixServiceAddress => Data.MedflixServiceAddress;
        public string AppIdentifier => Data.AppIdentifier;

        private AppConfig(AppConfigData appConfigData)
        {
            Data = appConfigData;
        }

        private static AppConfig Load()
        {
            AppConfigData data = null;
            try
            {
                if (File.Exists(AppConfigFilePath))
                {
                    var text = File.ReadAllText(AppConfigFilePath);
                    if (!string.IsNullOrEmpty(text))               
                        data = JsonSerializer.Deserialize<AppConfigData>(text);
                }
            }
            catch (Exception)
            {

            }

            if(data == null)
                data = new AppConfigData { AppIdentifier = $"MEDFLIX_CLIENT_{Guid.NewGuid()}" };

            return new AppConfig(data);
        }

        public void UpdateHostServiceAddress(string newHostServiceAddress)
        {
            Data.MedflixServiceAddress = newHostServiceAddress;
            Save();
        }

        private void Save()
        {
            try
            {
                var appConfigJson = JsonSerializer.Serialize(this.Data);
                File.WriteAllText(AppConfigFilePath, appConfigJson);
            }
            catch (Exception)
            {


            }

        }
    }
}
