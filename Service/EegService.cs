using System;
using Common;

namespace Service
{
    public class EegService : IEegService
    {
        public AckResponse StartSession(EegMeta meta)
        {
            // TODO:
            // - Kreirati session folder
            // - Validirati meta podatke
            // - Pokrenuti događaj OnTransferStarted

            throw new NotImplementedException();
        }

        public AckResponse PushSample(EegSample sample)
        {
            // TODO:
            // - Validacija podataka
            // - Upis u fajl
            // - Analitika i warning eventi

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