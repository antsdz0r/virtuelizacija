using Common;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.ServiceModel;

namespace Service
{
    public class EegService : IEegService, IDisposable
    {
        private EegMeta _currentMeta;
        private int _lastRowIndex = -1;
        private int _receivedCount = 0;
        private StreamWriter _sessionWriter;
        private StreamWriter _rejectsWriter;
        private bool _disposed = false;

        //  StartSession 
        public AckResponse StartSession(EegMeta meta)
        {
            if (meta == null || string.IsNullOrWhiteSpace(meta.ParticipantId))
                throw new FaultException<ValidationFault>(new ValidationFault("ParticipantId je obavezan.", -1));

            _currentMeta = meta;
            _lastRowIndex = -1;
            _receivedCount = 0;

            // Mora da otvori fajlove inace PushSample puca (_sessionWriter == null)
            string dataPath = ConfigurationManager.AppSettings["DataPath"] ?? "Data";
            string sessionDir = Path.Combine(dataPath, meta.ParticipantId, DateTime.Now.ToString("yyyy-MM-dd"));
            Directory.CreateDirectory(sessionDir);

            string sessionFile = Path.Combine(sessionDir, "session.csv");
            string rejectsFile = Path.Combine(sessionDir, "rejects.csv");

            bool sessionExists = File.Exists(sessionFile);
            _sessionWriter = new StreamWriter(new FileStream(sessionFile, FileMode.Append, FileAccess.Write, FileShare.Read));
            if (!sessionExists)
                _sessionWriter.WriteLine("Timestamp,AF3,T7,Pz,T8,AF4,Attention,Engagement,Excitement,Interest,Relaxation,Stress,Battery,ContactQuality,SlideIndex,SetIndex,RowIndex");

            bool rejectsExists = File.Exists(rejectsFile);
            _rejectsWriter = new StreamWriter(new FileStream(rejectsFile, FileMode.Append, FileAccess.Write, FileShare.Read));
            if (!rejectsExists)
                _rejectsWriter.WriteLine("Time,Reason,RawRow");

            Console.WriteLine($"[Server] >>> ZAPOCET PRENOS za Participant={meta.ParticipantId}, File={meta.FileName}, OcekivanoRedova={meta.TotalRows}");

            return new AckResponse { IsAck = true, Message = "Sesija otvorena.", Status = SessionStatus.InProgress };
        }

        // PushSample 
        public AckResponse PushSample(EegSample sample)
        {
            // 1. Null check
            if (sample == null)
                throw new FaultException<DataFormatFault>(new DataFormatFault("Sample je null.", -1));

            // 2. Monotoni RowIndex
            if (sample.RowIndex <= _lastRowIndex)
            {
                LogReject(sample, $"RowIndex nije monoton (primljen {sample.RowIndex}, prethodni {_lastRowIndex})");
                throw new FaultException<ValidationFault>(new ValidationFault(
                    $"RowIndex nije monoton: primljen {sample.RowIndex}, prethodni {_lastRowIndex}.", sample.RowIndex));
            }
                _lastRowIndex = sample.RowIndex;
            

            // 3. Timestamp
            if (sample.Timestamp == DateTime.MinValue)
            {
                LogReject(sample, "Timestamp nije validan");
                throw new FaultException<DataFormatFault>(new DataFormatFault("Timestamp nije validan.", sample.RowIndex));
            }

            // 4. Nenegativne metrike
            if (sample.Attention < 0 || sample.Engagement < 0 || sample.Excitement < 0 ||
                sample.Interest < 0 || sample.Relaxation < 0 || sample.Stress < 0)
            {
                LogReject(sample, "Negativna metrika");
                throw new FaultException<ValidationFault>(new ValidationFault("Negativna metrika.", sample.RowIndex));
            }

            // 4b. NaN/Infinity za sve EEG kanale
            double[] channels = { sample.AF3, sample.T7, sample.Pz, sample.T8, sample.AF4 };
            string[] chNames = { "AF3", "T7", "Pz", "T8", "AF4" };
            for (int i = 0; i < channels.Length; i++)
            {
                if (double.IsNaN(channels[i]) || double.IsInfinity(channels[i]))
                {
                    LogReject(sample, $"Neispravan kanal {chNames[i]}");
                    throw new FaultException<DataFormatFault>(new DataFormatFault($"Neispravan kanal {chNames[i]}.", sample.RowIndex));
                }
            }

            // Upis u session.csv
            _sessionWriter?.WriteLine(
                $"{sample.Timestamp:O},{sample.AF3.ToString(CultureInfo.InvariantCulture)},{sample.T7.ToString(CultureInfo.InvariantCulture)},{sample.Pz.ToString(CultureInfo.InvariantCulture)},{sample.T8.ToString(CultureInfo.InvariantCulture)},{sample.AF4.ToString(CultureInfo.InvariantCulture)}," +
                $"{sample.Attention.ToString(CultureInfo.InvariantCulture)},{sample.Engagement.ToString(CultureInfo.InvariantCulture)},{sample.Excitement.ToString(CultureInfo.InvariantCulture)},{sample.Interest.ToString(CultureInfo.InvariantCulture)}," +
                $"{sample.Relaxation.ToString(CultureInfo.InvariantCulture)},{sample.Stress.ToString(CultureInfo.InvariantCulture)},{sample.Battery},{sample.ContactQuality}," +
                $"{sample.SlideIndex},{sample.SetIndex},{sample.RowIndex}");

            _receivedCount++;

            Console.WriteLine($"[Server] prenos u toku... Participant={_currentMeta?.ParticipantId} Row={sample.RowIndex}");

            return new AckResponse { IsAck = true, Message = $"Red {sample.RowIndex} primljen.", Status = SessionStatus.InProgress };
        }

        private void LogReject(EegSample sample, string reason)
        {
            try
            {
                string raw = sample == null ? "" :
                    $"{sample.Timestamp:O};AF3={sample.AF3};Stress={sample.Stress};Bat={sample.Battery};CQ={sample.ContactQuality};Row={sample.RowIndex}";
                _rejectsWriter?.WriteLine($"{DateTime.Now:O},\"{reason}\",\"{raw}\"");
                _rejectsWriter?.Flush();
            }
            catch { /* logovanje ne sme da obori servis */ }
        }

        // EndSession
        public AckResponse EndSession()
        {
            Console.WriteLine($"[Server] <<< ZAVRSEN PRENOS za Participant={_currentMeta?.ParticipantId}, primljeno: {_receivedCount} redova.");

            _sessionWriter?.Flush();
            _sessionWriter?.Dispose();
            _sessionWriter = null;

            _rejectsWriter?.Flush();
            _rejectsWriter?.Dispose();
            _rejectsWriter = null;

            return new AckResponse { IsAck = true, Message = "Sesija završena.", Status = SessionStatus.Completed };
        }

        // ?? IDisposable 
        ~EegService() { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _sessionWriter?.Dispose();
                    _rejectsWriter?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}