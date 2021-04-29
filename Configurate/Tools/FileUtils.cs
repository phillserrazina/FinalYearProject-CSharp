using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System.Xml.Linq;

using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Win32;

namespace Configurate.Tools
{
    static class FileUtils
    {
        // METHODS
        public static Dictionary<string, string> Parse(string path, string parserFile)
        {
            // Attempt to use source parsers
            try
            {
                switch (parserFile.ToLower())
                {
                    case "ini": return ParseIni(path);
                    case "json": return ParseJson(path);
                    case "xml": return ParseXml(path);
                
                    default: break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went wrong with the parsing process! Please try a different parser.\n\n Full Error Message:\n" + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            // If none of the source parsers work, try external parsers
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
            // Attempt to use source savers
            try
            {
                switch (parserFile.ToLower())
                {
                    case "ini": 
                        SaveIni(dic, path);
                        MessageBox.Show("Saved Successfully.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "json": 
                        SaveJson(dic, path);
                        MessageBox.Show("Saved Successfully.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "xml": 
                        SaveXml(dic, path);
                        MessageBox.Show("Saved Successfully.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    default: break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error:\n\n" + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // If none of the source savers work, try external savers
            try
            {
                // Convert settings dictionary into a json object
                string args = JsonConvert.SerializeObject(dic);
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

        public static bool SaveAs(string path)
        {
            // Get file's text
            var fileToExport = GetFileText(path);
            // Get file's type
            string fileType = Path.GetExtension(path);

            // Open a dialog to export a file of the same type as the intended file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "Export",
                Filter = fileType.ToUpper() + " File (*" + fileType + ")|*" + fileType,
                DefaultExt = "." + fileType
            };

            // Write all text of the target file to the new file location
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, fileToExport);
                return true;
            }

            return false;
        }

        public static string GetNewFilePath(string fileType, string windowTitle)
        {
            // Open a dialog to export a file of the same type as the intended file
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = windowTitle,
                Filter = fileType.ToUpper() + " File (*" + fileType + ")|*" + fileType,
                DefaultExt = "." + fileType
            };

            // Get result of the users selection in the dialog
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                // Return the select dialog path
                return dialog.FileName;
            }

            return null;
        }

        public static string GetFileType(string path) => Path.GetExtension(path);

        private static string GetFileText(string path)
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
                {
                    //string key = $"[{section.SectionName}] {keyPair.KeyName}";
                    string key = keyPair.KeyName;
                    if (!answer.ContainsKey(key))
                    {
                        answer.Add(key, keyPair.Value);
                    }
                }
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

            // If the provided dictionary doesn't exist or is empty,
            // then there is no point in running this method
            if (currentDictionary == null) return null;
            if (currentDictionary.Count == 0) return null;

            // If the CURF does not exist, just return a dictionary
            // with the original keys, instead of the CURF keys
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

            // Read the CURF
            using (var reader = new StreamReader(curfPath))
            {
                // Go through all the lines
                while (line != null)
                {
                    // Read the current line
                    line = reader.ReadLine();
                    
                    // If the line isn't empty
                    if (!string.IsNullOrEmpty(line))
                    {
                        // Get the original and the properties
                        var valuePair = line.Split('=');
                        string rawVarName = valuePair[0];
                        string properties = valuePair[1];

                        // Get all the properties (new name, description, etç)
                        string[] propertyList = properties.Split('[');
                        string newVarName = propertyList[0];

                        string description = "No Description Available";

                        // Get description
                        if (propertyList.Length > 1)
                        {
                            description = propertyList[1].Substring(0, propertyList[1].Length - 1);
                        }

                        // Populate dictionary with the new variable name
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

            /*
            foreach (var pair in dic)
            {
                var split = pair.Key.Split(']');
                string section = split[0].Substring(1, split[0].Length-1);
                string key = split[1].Substring(1, split[1].Length-1);

                data[section][key] = pair.Value;
            }
            */
            
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
                        element.Value = dic[element.Name.LocalName];
                    }
                }
            }
        }

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
}
