using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JarD3obfuscator.Utils
{
    class FileHelper
    {
        private const string IGNORED_PREFIX = "//";
        private const string CFG_START = "============CONFIG_START============";
        private const string CFG_END = "============CONFIG_END============";
        private const string SEPARATOR = " => ";

        public static void Save(Dictionary<string,string> replacedDict, string path = "ReplacedConfig.txt")
        {
            path ??= "ReplacedConfig.txt";
            try
            {
                using StreamWriter writer = new StreamWriter(path, false);
                writer.WriteLine(IGNORED_PREFIX + "All replacemens done prevoiusly");
                writer.WriteLine(CFG_START);
                foreach (var replacement in replacedDict)
                {
                    writer.WriteLine(replacement.Key + SEPARATOR + replacement.Value);
                }
                writer.WriteLine(CFG_END);
            }
            catch(Exception e)
            {
                Logger.LogError("While writing: " + e.ToString());
            }
        }
        public static Dictionary<string,string> Read(string path = "ReplacedConfig.txt")
        {
            path ??= "ReplacedConfig.txt";
            if (!File.Exists(path))
            {
                Logger.LogError("Config not found");
                return null;
            }
            using StreamReader writer = new StreamReader(path);
            Dictionary<string, string> toret = new Dictionary<string, string>();
            try
            {
                while (!writer.EndOfStream)
                {
                    var line = writer.ReadLine();
                    if (string.Equals(line, CFG_START))
                    {
                        string replacement = string.Empty;
                        do
                        {
                            replacement = writer.ReadLine();
                            if (string.Equals(replacement, CFG_END))
                            {
                                Logger.Log("Read config");
                                break;
                            }
                            var keyValuePair = replacement.Split(SEPARATOR);
                            if (keyValuePair.Length > 2)
                                Logger.LogWarning("Got multiple replacements for singe word, (corrupted file?)");
                            if (keyValuePair.Length < 2)
                            {
                                Logger.LogError("Word - replacement pair is corrupted, insufficient info");
                            }
                            else
                            {
                                Logger.LogInfo($"Got word {keyValuePair[0]} replacement {keyValuePair[1]}");
                                toret.Add(keyValuePair[0], keyValuePair[1]);
                            }
                        } while (!writer.EndOfStream);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("While reading: " + e.ToString());
            }
            return toret;
        }
    }
}
