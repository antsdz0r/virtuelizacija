using Common;
using System;
using System.IO;
using System.ServiceModel;

namespace Service
{
    public class EegService : IEegService
    {
        private EegMeta _currentMeta;
        private int _lastRowIndex = -1;
        private StreamWriter _sessionWriter;
        private StreamWriter _rejectsWriter;
        private string _sessionPath;


        public AckResponse StartSession(EegMeta meta)
        {
            if (meta == null || string.IsNullOrWhiteSpace(meta.ParticipantId))
                throw new FaultException<ValidationFault>(new ValidationFault("ParticipantId je obavezan.", -1));

            throw new NotImplementedException();
        }

        public AckResponse PushSample(EegSample sample)
        {
            // 1. Null check
            if (sample == null)
                throw new FaultException<DataFormatFault>(new DataFormatFault("Sample je null.", -1));

            // 2. Monotoni RowIndex
            if (sample.RowIndex <= _lastRowIndex)
                throw new FaultException<ValidationFault>(new ValidationFault($"RowIndex nije monoton: primljen {sample.RowIndex}, prethodni {_lastRowIndex}.", sample.RowIndex));
            _lastRowIndex = sample.RowIndex;

            // 3. Timestamp parsiranje
            if (sample.Timestamp == DateTime.MinValue)
                throw new FaultException<DataFormatFault>(new DataFormatFault("Timestamp nije validan.", sample.RowIndex));

            // 4. Realni opsezi
            if (sample.Attention < 0 || sample.Engagement < 0 || sample.Stress < 0)
                throw new FaultException<ValidationFault>(new ValidationFault("Negativna metrika.", sample.RowIndex));
            if (double.IsNaN(sample.AF3) || double.IsInfinity(sample.AF3))
                throw new FaultException<DataFormatFault>(new DataFormatFault("Neispravan kanal AF3.", sample.RowIndex));

            throw new NotImplementedException();
        }

        public AckResponse EndSession()
        {
            // TODO:
            // - Zatvoriti streamove
            // - Pokrenuti OnTransferCompleted

            throw new NotImplementedException();
        }
    }
}