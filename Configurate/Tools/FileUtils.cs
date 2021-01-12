using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;

using Microsoft.Win32;

namespace Configurate.Tools
{
    class FileUtils
    {
        // METHODS
        public static Dictionary<string, string> Parse(string path)
        {
            var fileType = Path.GetExtension(path);

            switch (fileType)
            {
                case ".ini": return ParseIni(path);
                case ".json": return ParseJson(path);

                default:
                    MessageBox.Show("Unsupported File Type: " + fileType, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
            }
        }

        public static void Save(Dictionary<string, string> dic, string path)
        {
            var fileType = Path.GetExtension(path);

            switch (fileType)
            {
                case ".ini": SaveIni(dic, path); break;
                case ".json": SaveJson(dic, path); break;

                default:
                    MessageBox.Show("Unsupported File Type: " + fileType, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }
        }

        public static bool SaveAs(Dictionary<string, string> dic, string path)
        {
            var fileToExport = GetFileText(dic, path);

            string fileType = Path.GetExtension(path);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "Export",
                Filter = fileType.ToUpper() + " File (*" + fileType + ")|*" + fileType,
                DefaultExt = "." + fileType
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, fileToExport);
                return true;
            }

            return false;
        }

        public static string GetNewFilePath(string fileType, string windowTitle)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = windowTitle,
                Filter = fileType.ToUpper() + " File (*" + fileType + ")|*" + fileType,
                DefaultExt = "." + fileType
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return dialog.FileName;
            }

            return null;
        }

        public static string GetFileType(string path) => Path.GetExtension(path);

        private static string GetFileText(Dictionary<string, string> dic, string path)
        {
            var fileType = Path.GetExtension(path);

            switch (fileType)
            {
                case ".ini": return GetIniText(path);

                default:
                    MessageBox.Show("Unsupported File Type: " + fileType, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "";
            }
        }

        #region Parse

        private static Dictionary<string, string> ParseIni(string path)
        {
            var answer = new Dictionary<string, string>();

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);
            var allSections = data.Sections;

            foreach (var section in allSections)
            {
                var allKeyPairs = section.Keys;

                foreach (var keyPair in allKeyPairs)
                    answer.Add(keyPair.KeyName, keyPair.Value);
            }

            return answer;
        }

        private static Dictionary<string, string> ParseJson(string path)
        {
            
            var answer = new Dictionary<string, string>();

            var contents = File.ReadAllText(path);
            using (var reader = new JsonTextReader(new StringReader(contents)))
            {
                while (reader.Read())
                {
                    if (reader.Value == null) continue;
                    string key = reader.Path;
                    if (key.Contains('.'))
                    {
                        key = Path.GetExtension(key);
                        key = key.Substring(1);
                    }
                    
                    if (key == reader.Value.ToString()) continue;

                    if (key.Contains('['))
                    {
                        var splitKey = key.Split('[');
                        key = splitKey[0];
                    }

                    //key = string.Format("{0} ({1})", key, Path.GetExtension(reader.ValueType.ToString()).Substring(1));
                    if (!answer.ContainsKey(key))
                        answer.Add(key, reader.Value.ToString());
                    else answer[key] += $", {reader.Value}";
                }
            }

            return answer;
            

            /*
            var dic = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            var answer = new Dictionary<string, string>();

            foreach (var property in dic.Properties())
            {
                if (property.HasValues)
                {
                    foreach (var child in property.Children())
                    {
                        var obj = child.ToObject<JValue>();
                        
                        //foreach (var p in obj.Properties()) answer.Add(p.Name, p.Value.ToString());
                    }
                }
                else
                    answer.Add(property.Name, property.Value.ToString());
            } 

            return answer;
            */
        }

        public static Dictionary<string, string> ParseCurf(string curfPath, Dictionary<string, string> currentDictionary, ref Dictionary<string, string> curfRealDic)
        {
            var answer = new Dictionary<string, string>();

            if (!File.Exists(curfPath)) return currentDictionary;

            string line = "";

            using (var reader = new StreamReader(curfPath))
            {
                
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var valuePair = line.Split('=');
                        string rawVarName = valuePair[0];
                        string newVarName = valuePair[1];

                        if (currentDictionary.ContainsKey(rawVarName))
                        {
                            answer.Add(newVarName, currentDictionary[rawVarName]);
                            curfRealDic.Add(newVarName, rawVarName);
                        }
                    }
                }
            }

            return answer;
        }
        #endregion

        #region File Getters

        private static string GetIniText(string path)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);

            return data.ToString();
        }

        #endregion

        #region Save

        private static void SaveIni(Dictionary<string, string> dic, string path)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);
            var allSections = data.Sections;

            foreach (var section in allSections)
            {
                var allKeyPairs = section.Keys;

                foreach (var keyPair in allKeyPairs)
                {
                    if (!dic.ContainsKey(keyPair.KeyName)) continue;

                    keyPair.Value = dic[keyPair.KeyName];
                }
            }

            parser.WriteFile(path, data);
        }

        private static void SaveJson(Dictionary<string, string> dic, string path)
        {
            var contents = File.ReadAllText(path);
        }

        #endregion
    }
}
