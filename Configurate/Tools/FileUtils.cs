using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;

using Microsoft.Win32;

using System.Linq;
using System.Xml.Linq;
using System.Reflection;

using System.Diagnostics;

namespace Configurate.Tools
{
    class FileUtils
    {
        // METHODS
        public static Dictionary<string, string> Parse(string path, string parserFile)
        {
            switch (parserFile)
            {
                case "Ini": return ParseIni(path);
                case "Json": return ParseJson(path);
                case "Xml": return ParseXml(path);
                
                default: break;
            }

            try
            {
                var result = PythonParse(parserFile, path);

                string errorCheck = result.Substring(0, 6);
                if (errorCheck == "Error:")
                {
                    MessageBox.Show(result, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                return ParseJsonText(result);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static void Save(Dictionary<string, string> dic, string path, string parserFile)
        {
            switch (parserFile)
            {
                case "Ini":
                    try
                    {
                        SaveIni(dic, path); 
                        MessageBox.Show("Saved Successfully.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch
                    {
                        MessageBox.Show("There was an error. Try again.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;

                case "Json":
                    try
                    {
                        SaveJson(dic, path);
                        MessageBox.Show("Saved Successfully.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch
                    {
                        MessageBox.Show("There was an error. Try again.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;

                case "Xml":
                    try
                    {
                        SaveXml(dic, path);
                        MessageBox.Show("Saved Successfully.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch
                    {
                        MessageBox.Show("There was an error. Try again.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;

                default: break;
            }

            try
            {
                string args = JsonConvert.SerializeObject(dic);
                MessageBox.Show(args);

                var result = PythonSave(parserFile, path, args);

                string errorCheck = result.Substring(0, 6);
                if (errorCheck == "Error:")
                {
                    MessageBox.Show(result, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show(result, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var contents = File.ReadAllText(path);
            return ParseJsonText(contents);
        }

        private static Dictionary<string, string> ParseJsonText(string contents)
        {
            var answer = new Dictionary<string, string>();

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

                    if (!answer.ContainsKey(key))
                        answer.Add(key, reader.Value.ToString());
                    else answer[key] += $", {reader.Value}";
                }
            }

            return answer;
        }

        private static Dictionary<string, string> ParseXml(string path)
        {
            var answer = new Dictionary<string, string>();

            XElement xmlObject = XElement.Load(path);
            var root = xmlObject.Elements();

            RecursiveXmlFetch(root, ref answer);

            return answer;
        }

        private static Dictionary<string, string> RecursiveXmlFetch(IEnumerable<XElement> elements, ref Dictionary<string, string> answer)
        {
            foreach (var element in elements)
            {
                if (element.HasElements)
                {
                    RecursiveXmlFetch(element.Elements(), ref answer);
                }
                else
                {
                    if (!answer.ContainsKey(element.Name.LocalName))
                        answer.Add(element.Name.LocalName, element.Value);
                }
            }

            return answer;
        }

        public static Dictionary<string, Dictionary<string, string>> ParseCurf(string curfPath, Dictionary<string, string> currentDictionary, ref Dictionary<string, string> curfRealDic)
        {
            var answer = new Dictionary<string, Dictionary<string, string>>();

            if (currentDictionary == null) return null;
            if (currentDictionary.Count == 0) return null;

            if (!File.Exists(curfPath))
            {
                foreach (var key in currentDictionary.Keys)
                {
                    answer.Add(key, new Dictionary<string, string>() { { "Value", currentDictionary[key] } });
                    curfRealDic.Add(key, key);
                }

                return answer;
            }

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
                        string properties = valuePair[1];

                        string[] propertyList = properties.Split('[');
                        string newVarName = propertyList[0];

                        string description = "No Description Available";

                        if (propertyList.Length > 1)
                        {
                            description = propertyList[1].Substring(0, propertyList[1].Length - 1);
                        }

                        if (currentDictionary.ContainsKey(rawVarName))
                        {
                            var propertiesDic = new Dictionary<string, string>()
                            {
                                { "Value", currentDictionary[rawVarName] },
                                { "Description", description }
                            };

                            if (!answer.ContainsKey(newVarName)) {
                                answer.Add(newVarName, propertiesDic);
                                curfRealDic.Add(newVarName, rawVarName);
                            }
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
            var result = JsonConvert.SerializeObject(dic, Newtonsoft.Json.Formatting.Indented);

            if (!File.Exists(path))
            {
                MessageBox.Show("Oops!", "File path is corrupted", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            File.WriteAllText(path, result);
        }

        private static void SaveXml(Dictionary<string, string> dic, string path)
        {
            XElement root = XElement.Load(path);
            var childs = root.Elements();

            RecursiveXmlSave(ref root, childs, dic);

            root.Save(path);
        }

        private static void RecursiveXmlSave(ref XElement root, IEnumerable<XElement> elements, Dictionary<string, string> dic)
        {
            foreach (var element in elements)
            {
                if (element.HasElements)
                {
                    var newElements = element.Elements();
                    RecursiveXmlSave(ref root, newElements, dic);
                }
                else
                {
                    if (dic.ContainsKey(element.Name.LocalName))
                    {
                        root.SetElementValue(element.Name.LocalName, dic[element.Name.LocalName]);
                    }
                }
            }
        }

        // ============================
        // item["test"]["nth"] = "updated";
        // ============================

        /*
        private static void JsonSaveHelper(JProperty prop, List<string> additionalPath, ref JObject obj, Dictionary<string, string> dic)
        {
            if (prop.Value.Type == JTokenType.Object)
            {
                var newObj = prop.Value.ToObject<JObject>();
                additionalPath.Add(prop.Name);

                foreach (var newProp in newObj.Properties().ToList())
                {
                    JsonSaveHelper(newProp, additionalPath, ref newObj, dic);
                }
            }

            else if (prop.Value.Type == JTokenType.Array)
            {
                if (obj.ContainsKey(prop.Name) && dic.ContainsKey(prop.Name))
                {
                    JArray newArr = new JArray();

                    var split = dic[prop.Name].Split(',');

                    for (int i = 0; i < split.Length; i++)
                    {
                        split[i] = split[i].Trim();
                        newArr.Add(split[i]);
                    }

                    obj[prop.Name] = new JArray(newArr);
                    //MessageBox.Show(prop.Name + ": " + obj[prop.Name]);
                }
                else MessageBox.Show("Object doesn't contain " + prop.Name + $".\nProp: {prop.Name} (Parent: {prop.Parent.Path})");
            }

            else
            {

                if (obj.ContainsKey(prop.Name) && dic.ContainsKey(prop.Name))
                {
                    obj[prop.Name] = dic[prop.Name];
                    //MessageBox.Show(prop.Name + ": " + obj[prop.Name]);
                }
                else MessageBox.Show("Object doesn't contain " + prop.Name + $".\nProp: {prop.Name} (Root: {prop.Root.Path})");

            }
        }
        */
        #endregion

        // Returns a json representation of the dictionary
        private static string PythonParse(string parserFileName, string filePath)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python34\\python.exe";
            start.Arguments = parserFileName + " \"" + filePath + "\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        private static string PythonSave(string parserFileName, string filePath, string jsonArgs)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python34\\python.exe";
            start.Arguments = parserFileName + " \"" + filePath + "\" \'" + jsonArgs.Replace("\"", "\"\"\"") + "\'";
            MessageBox.Show(start.Arguments);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }


    public class DDTestFormat
    {
        public class Values
        {
            public int[] controller_enabled;
            public int[] hold_required_in_dungeon;
            public int[] left_stick_interact;
            public int[] controller_vibration;
            public int[] fullscreen;
            public int[] monitor_number;
            public int[] resolution;
            public int[] gamma;
            public int[] combat_pivot_camera;
            public int[] blur;
            public string subtitles;
            public int[] mute_when_window_not_focussed;
            public int[] master_volume;
            public int[] sfx_volume;
            public int[] music_volume;
            public int[] narration_volume;
            public int[] video_volume;
            public int[] tutorial;
            public int[] metrics;
            public int[] extra_bark_time;
            public int[] bark_dismissal;
            public int[] roster_sort_party_to_top;
            public int[] debug_output;
            public string language;
            public int[] map_follow_party;
            public int[] framerate;

            public Values(Dictionary<string, string> dic)
            {
                FieldInfo[] fields = typeof(Values).GetFields(BindingFlags.Public | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(int[]))
                    {
                        field.SetValue(this, dic[field.Name].Split(", ").Select(n => Convert.ToInt32(n)).ToArray());
                    }
                    else field.SetValue(this, dic[field.Name]);
                }
            }
        }

        public class Data
        {
            public Values values;
        }

        public int version;
        public Data data;

        public DDTestFormat(Dictionary<string, string> dic)
        {
            version = int.Parse(dic["version"]);
            data = new Data();
            data.values = new Values(dic);
        }
    }
}
