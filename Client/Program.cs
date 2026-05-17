using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Common;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EEG WCF Client");

            string eegPath = ConfigurationManager.AppSettings["EegDataPath"];
            string[] csvFiles = Directory.GetFiles(eegPath, "subject_*_results.csv", SearchOption.AllDirectories);


            foreach (string csvFile in csvFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(csvFile); // subject_X_results
                string[] parts = fileName.Split('_');
                string participantId = parts[1]; // X

                List<EegSample> samples = CsvParser.Parse(csvFile);
                Console.WriteLine($"Participant {participantId}: parsirano {samples.Count} redova.");
                // TODO:

                // 3. Pozvati StartSession
                // 4. Slati sample po sample
                // 5. Pozvati EndSession
            }
                Console.ReadLine();
        }
    }
}