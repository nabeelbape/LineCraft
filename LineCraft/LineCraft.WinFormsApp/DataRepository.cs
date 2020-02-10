using LineCraft.WinFormsApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace LineCraft.WinFormsApp
{
    public class DataRepository
    {
        private readonly string appDataPath;
        private readonly string profilePath;

        public DataRepository()
        {
            appDataPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "AppData");
            profilePath = Path.Combine(appDataPath, "Profiles");
        }

        public SettingsModel GetSettings()
        {
            string fileContent = Path.Combine(appDataPath, "settings.json");
            var settings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(fileContent));
            settings.Profile = GetAllProfiles().FirstOrDefault(p => p.Name == settings.DefaultProfile);

            return settings;
        }

        public List<ProfileModel> GetAllProfiles()
        {
            var profiles = new List<ProfileModel>();
            var files = Directory.GetFiles(profilePath, "*.json");

            foreach (var file in files)
            {
                string fileContent = File.ReadAllText(file);
                var profile = JsonConvert.DeserializeObject<ProfileModel>(fileContent);
                profiles.Add(profile);
            }

            return profiles;
        }

        public void SaveProfile(ProfileModel profile)
        {
            string filePath = Path.Combine(profilePath, profile.Name + ".json");
            string fileContent = JsonConvert.SerializeObject(profile);

            using (var writer = new StreamWriter(filePath,false))
            {
                writer.Write(fileContent);
            }
        }
    }
}