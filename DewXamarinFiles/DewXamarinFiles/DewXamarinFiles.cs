using System;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DewCore.Xamarin.Files
{
    /// <summary>
    /// Dew Xamarin Files Class
    /// </summary>
    public static class DewXamarinFiles
    {
        /// <summary>
        /// Local settings name (without extension), default: __dew_loc_sett
        /// </summary>
        public static string SettingsName = "__dew_loc_sett";
        /// <summary>
        /// Default buffer size for file write/read
        /// </summary>
        public static int BufferSize = 0;
        /// <summary>
        /// Read a file as byte array (if app, pass DewXamarinFiles.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<byte[]> ReadLocalFileAsync(string path)
        {
            byte[] result = null;
            using (Stream s = File.Open(path, FileMode.Open))
            {
                result = new byte[s.Length];
                var offset = 0;
                while (await s.ReadAsync(result, offset, BufferSize) > 0)
                {
                    offset += BufferSize;
                }
            }
            return result;
        }
        /// <summary>
        /// Read a file as string (if app, pass DewXamarinFiles.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<string> ReadLocalFileTextAsync(string path)
        {
            string result = null;
            using (Stream s = File.Open(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    result = await sr.ReadToEndAsync();
                }
            }
            return result;
        }
        /// <summary>
        /// Write a file as byte array (if app, pass DewXamarinFiles.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="s">data</param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task WriteLocalFileAsync(byte[] s, string name, string path = null)
        {
            using (FileStream file = File.Create(path + Path.DirectorySeparatorChar + name))
            {
                BufferSize = BufferSize == 0 ? s.Length : BufferSize;
                var offset = 0;
                while (offset < s.Length)
                {
                    await file.WriteAsync(s, offset, BufferSize);
                    offset += BufferSize;
                }
            }
        }
        /// <summary>
        /// Write a file as string (if app, pass DewXamarinFiles.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="s">data</param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task WriteLocalFileTextAsync(string s, string name, string path = null)
        {
            using (FileStream file = File.Create(path + Path.DirectorySeparatorChar + name))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    await sw.WriteAsync(s);
                }
            }
        }
        /// <summary>
        /// Return the base path for the current platform
        /// </summary>
        /// <returns></returns>
        public static string ApplicationPath()
        {
            var assembly = Application.Current.GetType().Assembly;
            string osv = "";
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                case Device.macOS:
                    {
                        osv = "Library";
                        break;
                    }
                case Device.Android:
                    break;
                case Device.UWP:
                case Device.WinRT:
                case Device.WinPhone:
                    {
                        osv = "LocalState";
                        break;
                    }
            }
            return Path.Combine(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), osv);

        }
        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="file">the path to the file</param>
        /// <returns></returns>
        public static bool CheckFileExists(string file)
        {
            var fil = new FileInfo(file);
            return fil.Exists;
        }
        /// <summary>
        /// Check if a directory exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckDirectoryExists(string path)
        {
            var dir = new DirectoryInfo(path);
            return dir.Exists;
        }
        /// <summary>
        /// Write a local setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async static Task WriteLocalSetting(string key, string option)
        {
            var path = ApplicationPath();
            if (!CheckFileExists(path + Path.DirectorySeparatorChar + SettingsName + ".json"))
            {
                await WriteLocalFileTextAsync("{}", SettingsName + ".json", path + Path.DirectorySeparatorChar);
            }
            string json = await ReadLocalFileTextAsync(path + Path.DirectorySeparatorChar + SettingsName + ".json");
            Dictionary<string, string> settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (settings.ContainsKey(key))
            {
                string serializate = option;
                settings[key] = serializate;
            }
            else
                settings.Add(key, option.ToString());
            await WriteLocalFileTextAsync(Newtonsoft.Json.JsonConvert.SerializeObject(settings), SettingsName + ".json", path + Path.DirectorySeparatorChar);
        }
        /// <summary>
        /// Write a local setting of type T (T must be serializable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async static Task WriteLocalSetting<T>(string key, T option)
        {
            var path = ApplicationPath();
            if (!CheckFileExists(path + Path.DirectorySeparatorChar + SettingsName + ".json"))
            {
                await WriteLocalFileTextAsync("{}", SettingsName + ".json", path + Path.DirectorySeparatorChar);
            }
            string json = await ReadLocalFileTextAsync(path + Path.DirectorySeparatorChar + SettingsName + ".json");
            Dictionary<string, string> settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (settings.ContainsKey(key))
            {
                string serializate = SerializeObject(option);
                settings[key] = serializate;
            }
            else
                settings.Add(key, option.ToString());
            await WriteLocalFileTextAsync(Newtonsoft.Json.JsonConvert.SerializeObject(settings), SettingsName + ".json", path + Path.DirectorySeparatorChar);
        }
        /// <summary>
        /// Check if a local setting exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<bool> CheckLocalSettingExists(string key)
        {
            var path = ApplicationPath();
            if (!CheckFileExists(Path.Combine(path, SettingsName + ".json")))
            {
                await WriteLocalFileTextAsync("{}", SettingsName + ".json", path + Path.DirectorySeparatorChar);
            }
            string json = await ReadLocalFileTextAsync(Path.Combine(ApplicationPath(), SettingsName + ".json"));
            Dictionary<string, string> settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return settings.ContainsKey(key);
        }
        /// <summary>
        /// Read a local setting of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<T> ReadLocalSetting<T>(string key)
        {
            if (!CheckFileExists(Path.Combine(ApplicationPath(), SettingsName + ".json")))
            {
                throw new FileNotFoundException();
            }
            string json = await ReadLocalFileTextAsync(Path.Combine(ApplicationPath(), SettingsName + ".json"));
            Dictionary<string, string> settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var deserialized = DeserializeObject<T>(settings[key]);
            return deserialized;
        }
        /// <summary>
        /// Read a local setting as string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<string> ReadLocalSetting(string key)
        {
            if (!CheckFileExists(Path.Combine(ApplicationPath(), SettingsName + ".json")))
            {
                throw new FileNotFoundException();
            }
            string json = await ReadLocalFileTextAsync(Path.Combine(ApplicationPath(), SettingsName + ".json"));
            Dictionary<string, string> settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return settings[key];
        }
        /// <summary>
        /// Serialize to json an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T toSerialize)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(toSerialize);
        }
        /// <summary>
        /// Serialize to json an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDeserialize"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string toDeserialize)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(toDeserialize);
        }
    }
}

