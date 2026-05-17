using System;
using System.Collections.Generic;
using System.IO;
using Common;

namespace Client
{
    public static class CsvParser
    {
        public static List<EegSample> Parse(string path)
        {
            var samples = new List<EegSample>();
            var errors = new List<string>();
            int rowIndex = 0;

            using (StreamReader reader = new StreamReader(path))
            {
                string header = reader.ReadLine(); // preskoci header red
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    rowIndex++;
                    try
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length < 16)
                            throw new FormatException("Nedovoljan broj kolona.");

                        var sample = new EegSample
                        {
                            Timestamp = DateTime.Parse(parts[0].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            AF3 = double.Parse(parts[1].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            T7 = double.Parse(parts[2].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Pz = double.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            T8 = double.Parse(parts[4].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            AF4 = double.Parse(parts[5].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Attention = double.Parse(parts[6].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Engagement = double.Parse(parts[7].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Excitement = double.Parse(parts[8].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Interest = double.Parse(parts[9].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Relaxation = double.Parse(parts[10].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Stress = double.Parse(parts[11].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                            Battery = int.Parse(parts[12].Trim()),
                            ContactQuality = int.Parse(parts[13].Trim()),
                            SlideIndex = int.Parse(parts[14].Trim()),
                            SetIndex = int.Parse(parts[15].Trim()),
                            RowIndex = rowIndex - 1
                        };
                        samples.Add(sample);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"[Row {rowIndex}] {path}: {ex.Message} ? \"{line}\"");
                    }
                }
            }

            if (errors.Count > 0)
                File.AppendAllLines("client_errors.log", errors);

            return samples;
        }
    }
}