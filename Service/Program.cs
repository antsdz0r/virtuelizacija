using System;
using System.ServiceModel;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                EegService svc = new EegService();

                svc.OnTransferStarted += (s, e) =>
                    Console.WriteLine($"[EVENT] Započet prenos: Participant={e.Meta.ParticipantId}, File={e.Meta.FileName}");

                svc.OnSampleReceived += (s, e) =>
                    Console.WriteLine($"[EVENT] Primljen uzorak: Participant={e.ParticipantId}, Row={e.Sample.RowIndex}");

                svc.OnTransferCompleted += (s, e) =>
                    Console.WriteLine($"[EVENT] Završen prenos: Participant={e.ParticipantId}, ukupno={e.TotalReceived}");

                svc.OnWarningRaised += (s, e) =>
                    Console.WriteLine($"[UPOZORENJE] {e.RaisedAt:HH:mm:ss} Participant={e.ParticipantId} Row={e.RowIndex}: {e.Message}");

                svc.OnPoorContact += (s, e) =>
                    Console.WriteLine($"[ANALITIKA] PoorContact     Participant={e.ParticipantId} Row={e.RowIndex} CQ={e.ContactQuality}");

                svc.OnLowBattery += (s, e) =>
                    Console.WriteLine($"[ANALITIKA] LowBattery      Participant={e.ParticipantId} Row={e.RowIndex} Bat={e.Battery}%");

                svc.OnTimeSkew += (s, e) =>
                    Console.WriteLine($"[ANALITIKA] TimeSkew        Participant={e.ParticipantId} Row={e.RowIndex} Skew={e.SkewMs:F0}ms");

                svc.OnStressSpike += (s, e) =>
                    Console.WriteLine($"[ANALITIKA] StressSpike {e.Direction} Δ={e.Delta:F2} | Participant={e.ParticipantId} Row={e.RowIndex} ({e.ValueBefore:F2}→{e.ValueAfter:F2})");

                svc.OnRelaxationDrop += (s, e) =>
                    Console.WriteLine($"[ANALITIKA] RelaxationDrop Δ={e.Delta:F2} | Participant={e.ParticipantId} Row={e.RowIndex} ({e.ValueBefore:F2}→{e.ValueAfter:F2})");

                using (ServiceHost host = new ServiceHost(svc))
                {
                    host.Open();
                    Console.WriteLine("[Service] EEG WCF Service started. Press ENTER to stop.");
                    Console.ReadLine();
                    host.Close();
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"[Greska] {ex.Message}");
            }
            
        }
    }
}
