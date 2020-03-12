using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestDirFileManager
{
    class Program
    {

        public class FileStruct
        {
            public string Name { get; set; }
            public string Size { get; set; }
            public string Path { get; set; }
        }

         public class DirStruct
        {
            public string Name { get; set; }
            public string DateCreated { get; set; }
            public IList<FileStruct> Files { get; set; }
            public IList<DirStruct> Children { get; set; }
        }

        public static DirStruct ParseDir(string dirName)
        {
            DirStruct dir = new DirStruct { Name = Directory.GetParent(dirName).Name, DateCreated = Directory.GetCreationTime(dirName).ToString() };
            string[] files = Directory.GetFiles(dirName);
            
            IList<FileStruct> listOfFiles = new List<FileStruct>();

            foreach (string s in files)
            {
                FileInfo fileInfo = new FileInfo(s);
                FileStruct file = new FileStruct { Name = fileInfo.Name, Size = fileInfo.Length.ToString() + " B", Path = s };
                listOfFiles.Add(file);
            }

            dir.Files = listOfFiles;

            string[] dirs = Directory.GetDirectories(dirName);
            IList<DirStruct> listOfDirs = new List<DirStruct>();
            foreach (string s in dirs)
            {
                DirStruct childDir = new DirStruct { Name = Directory.GetParent(s + @"\").Name, DateCreated = Directory.GetCreationTime(s).ToString() };
                listOfDirs.Add(childDir);
            }

            if (listOfDirs.Count > 0)
            {
                IList<DirStruct> tempListOfDirs = new List<DirStruct>();

                foreach (DirStruct directories in listOfDirs)
                {
                    string tempDirPath = dirName + directories.Name + @"\";
                    tempListOfDirs.Add(ParseDir(tempDirPath));
                }
                listOfDirs.Clear();
                listOfDirs = tempListOfDirs;
            }

            dir.Children = listOfDirs;

            return dir;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter directory path:");
            string dirName = Console.ReadLine();
            dirName += @"\";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            if (Directory.Exists(dirName))
            {
                string json = JsonSerializer.Serialize(ParseDir(dirName), options);
                Console.WriteLine(json);
                StreamWriter sw = new StreamWriter("file_hierarchy.json");
                sw.Write(json);
                sw.Close();
            }
        }
    }
}
