using JarD3obfuscator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JarD3obfuscator
{
    class Deobfuscator
    {
        private readonly string[] BannedWords = { "con","com", "prn", "aux", "nul" };

        private string[] BannedWordsRegEx;

        private Dictionary<string, string> ReplacedWords;

        private const string ReplacedWordMark = "REP_";
        public Deobfuscator()
        {
            ReplacedWords = new Dictionary<string, string>();

            BannedWordsRegEx = new string[BannedWords.Length];
            for(int i = 0; i< BannedWords.Length; i++)
            {
                BannedWordsRegEx[i] = BannedWords[i] + @"(\d)?\b";
            }
        }

        public void Save(string path)
        {
            FileHelper.Save(ReplacedWords, path);
        }

        private void DeobfuscateJava(string file)
        {
            using var archive = new ZipArchive(File.Open(file, FileMode.Open, FileAccess.ReadWrite), ZipArchiveMode.Update);
            var entries = archive.Entries.ToArray();
            Logger.Log($"Got {entries.Length} files");
            foreach (var entry in entries)
            {
                if (entry.Name == string.Empty || entry.CompressedLength == 0)
                    continue;
                if (!entry.Name.EndsWith(".class") && !entry.Name.EndsWith(".java"))
                    continue;
                if (entry.Name.EndsWith(".class"))
                    Logger.LogWarning("Class file found, was jar not properly decomplied? " + entry.FullName);
                ZipArchiveEntry newEntry = null;
                foreach (var bannedWordRegEx in BannedWordsRegEx)
                {
                    var match = Regex.Match(entry.Name, bannedWordRegEx, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string oldName = match.Groups[0].Value;
                        string newName = string.Empty;
                        if (ReplacedWords.ContainsKey(oldName))
                        {
                            newName = ReplacedWords[oldName];
                        }
                        else
                        {
                            newName = ReplacedWordMark + ReplacedWords.Count;
                            ReplacedWords.Add(oldName, newName);
                            Logger.LogInfo($"Replaced word {oldName} => {newName} from file {entry.FullName}");
                        }

                        newEntry = archive.CreateEntry(Path.GetDirectoryName(entry.FullName) + @"\" + newName + Path.GetExtension(entry.Name));
                        break;
                    }
                }

                bool isOldFile = false;
                bool hasReplacedOnce = false;

                if (newEntry is null)
                {
                    newEntry = archive.CreateEntry(entry.FullName + ".temp");
                    isOldFile = true;
                }

                {
                    using var stream = entry.Open();
                    using var newStream = newEntry.Open();

                    using StreamReader reader = new StreamReader(stream);
                    using StreamWriter writer = new StreamWriter(newStream);

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line is null)
                            continue;

                        bool wereChangesMade = false;
                        do
                        {
                            wereChangesMade = false;
                            foreach (var bannedWordRegEx in BannedWordsRegEx)
                            {
                                var matches = Regex.Match(line, bannedWordRegEx, RegexOptions.IgnoreCase);
                                if (matches.Success)
                                {
                                    string oldName = matches.Groups[0].Value;
                                    string newName = string.Empty;
                                    if (ReplacedWords.ContainsKey(oldName))
                                    {
                                        newName = ReplacedWords[oldName];
                                    }
                                    else
                                    {
                                        newName = ReplacedWordMark + ReplacedWords.Count;
                                        ReplacedWords.Add(oldName, newName);
                                        Logger.LogInfo($"Replaced word {oldName} => {newName} from file {entry.FullName}");
                                    }
                                    line = line.Replace(oldName, newName);

                                    hasReplacedOnce = true;
                                    wereChangesMade = true;
                                }
                            }
                        } while (wereChangesMade);
                        writer.WriteLine(line);
                    }
                }
                if (hasReplacedOnce && isOldFile)
                {
                    entry.Delete();
                    var replaceEntry = archive.CreateEntry(Path.GetDirectoryName(newEntry.FullName) + @"\" + Path.GetFileNameWithoutExtension(newEntry.FullName));
                    using var newStream = newEntry.Open();
                    using var replaceStream = replaceEntry.Open();

                    newStream.CopyTo(replaceStream);
                    newStream.Close();
                    newEntry.Delete();
                }
                else if (!hasReplacedOnce && isOldFile)
                {
                    newEntry.Delete();
                }
                else
                {
                    entry.Delete();
                }
            }
        }

        private void DeobfuscateNonJava(string file)
        {
            Logger.LogWarning("Java fix is disabled");
            using var archive = new ZipArchive(File.Open(file, FileMode.Open, FileAccess.ReadWrite), ZipArchiveMode.Update);
            var entries = archive.Entries.ToArray();
            Logger.Log($"Got {entries.Length} files");
            foreach (var entry in entries)
            {
                if (entry.Name == string.Empty || entry.CompressedLength == 0)
                    continue;
                ZipArchiveEntry newEntry = null;
                foreach (var bannedWordRegEx in BannedWordsRegEx)
                {
                    var match = Regex.Match(entry.Name, bannedWordRegEx, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string oldName = match.Groups[0].Value;
                        string newName = string.Empty;
                        if (ReplacedWords.ContainsKey(oldName))
                        {
                            newName = ReplacedWords[oldName];
                        }
                        else
                        {
                            newName = ReplacedWordMark + ReplacedWords.Count;
                            ReplacedWords.Add(oldName, newName);
                            Logger.LogInfo($"Replaced word {oldName} => {newName} from file {entry.FullName}");
                        }

                        newEntry = archive.CreateEntry(Path.GetDirectoryName(entry.FullName) + @"\" + newName + Path.GetExtension(entry.Name));
                        break;
                    }
                }
                if (newEntry is not null)
                {
                    entry.Delete();
                }
            }
        }

        public void Deobfuscate(string file, bool disableJavaFix)
        {
            if (!File.Exists(file))
            {
                Logger.LogError("File not found");
                return;
            }
            if (disableJavaFix)
                DeobfuscateNonJava(file);
            else
                DeobfuscateJava(file);

            Logger.Log("Done.");
        }

        internal bool Load(string path)
        {
            ReplacedWords = FileHelper.Read(path);
            return !(ReplacedWords == null || ReplacedWords.Count == 0);
        }
    }
}
