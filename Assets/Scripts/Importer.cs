using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DefaultNamespace
{

    public class Importer
    {
        static string project_dir = Path.GetDirectoryName(Path.GetFullPath(Environment.CurrentDirectory));
        static string jobListPath = Path.Combine(project_dir, "data", "JobList.csv");
        private static string workerListPath = "Resources/Data/WorkerList.csv";

        // public static List<List<string>> ImportWorker()
        // {
        //     List<List<string>> list = new List<List<string>>();
        //     using (StreamReader reader = new StreamReader(workerListPath))
        //     {
        //         string line;
        //         while ((line = reader.ReadLine()) != null)
        //         {
        //             List<string> row = line.Split(',').ToList();
        //             list.Add(row);
        //         }
        //     }
        //     return list.Skip(1).ToList();
        // }

        public static List<List<string>> ImportJob()
        {
            List<List<string>> list = new List<List<string>>();
            using (StreamReader reader = new StreamReader(jobListPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> row = line.Split(',').ToList();
                    list.Add(row);
                }
            }
            return list.Skip(1).ToList();
        }
        
    }
}