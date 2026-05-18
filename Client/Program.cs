using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using Common;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EEG WCF Client");

            string eegPath = ConfigurationManager.AppSettings["EegDataPath"] ?? "EEG";

            if (!Directory.Exists(eegPath))
            {
                Console.WriteLine($"[Greska] Direktorijum '{eegPath}' ne postoji.");
                Console.ReadLine();
                return;
            }

            string[] csvFiles = Directory.GetFiles(eegPath, "subject_*_results.csv", SearchOption.AllDirectories);

            foreach (string csvFile in csvFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(csvFile);
                string[] parts = fileName.Split('_');
                string participantId = parts.Length > 1 ? parts[1] : fileName;

                List<EegSample> samples = CsvParser.Parse(csvFile);
                Console.WriteLine($"[Klijent] Participant {participantId}: parsirano {samples.Count} redova.");

                if (samples.Count == 0) continue;

                var meta = new EegMeta(participantId, Path.GetFileName(csvFile), samples.Count);
                var binding = new NetTcpBinding
                {
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxReceivedMessageSize = 10485760,
                    ReceiveTimeout = TimeSpan.FromMinutes(10),
                    SendTimeout = TimeSpan.FromMinutes(10),
                    OpenTimeout = TimeSpan.FromMinutes(1),
                    CloseTimeout = TimeSpan.FromMinutes(1)
                };
                var address = new EndpointAddress("net.tcp://localhost:5000/EegService");

                ChannelFactory<IEegService> factory = null;
                IEegService proxy = null;

                try
                {
                    factory = new ChannelFactory<IEegService>(binding, address);
                    proxy = factory.CreateChannel();
                    ((IClientChannel)proxy).Open();

                    // StartSession
                    AckResponse startAck = proxy.StartSession(meta);
                    Console.WriteLine($"[Klijent] StartSession ACK={startAck.IsAck} | {startAck.Message}");

                    if (!startAck.IsAck) continue;

                    // PushSample po jedan red
                    int sentCount = 0;
                    foreach (EegSample sample in samples)
                    {
                        try
                        {
                            proxy.PushSample(sample);
                            sentCount++;
                        }
                        catch (FaultException<DataFormatFault> ex)
                        {
                            Console.WriteLine($"[Klijent] DataFormatFault red {ex.Detail.RowIndex}: {ex.Detail.Message}");
                        }
                        catch (FaultException<ValidationFault> ex)
                        {
                            Console.WriteLine($"[Klijent] ValidationFault red {ex.Detail.RowIndex}: {ex.Detail.Message}");
                        }
                    }

                    // EndSession
                    AckResponse endAck = proxy.EndSession();
                    Console.WriteLine($"[Klijent] EndSession ACK={endAck.IsAck} | {endAck.Message} | Poslato: {sentCount}/{samples.Count}");

                    ((IClientChannel)proxy).Close();
                    factory.Close();
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"[Klijent] Greska u komunikaciji za {participantId}: {ex.Message}");
                    ((IClientChannel)proxy)?.Abort();
                    factory?.Abort();
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"[Klijent] Timeout za {participantId}: {ex.Message}");
                    ((IClientChannel)proxy)?.Abort();
                    factory?.Abort();
                }
            }

            Console.WriteLine("[Klijent] Gotovo. Pritisnite ENTER za izlaz.");
            Console.ReadLine();
        }
    }
}