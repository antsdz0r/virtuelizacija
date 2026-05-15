using System.ServiceModel;

namespace Common
{
    /// <summary>
    /// WCF Service Contract for streaming EEG data from client to server.
    ///
    /// Protocol:
    ///   1. Client calls StartSession(meta)   → server opens/creates CSV file
    ///   2. Client calls PushSample(sample)   → server validates + appends row
    ///   3. Client calls EndSession()         → server closes file, fires OnTransferCompleted
    /// </summary>
    [ServiceContract]
    public interface IEegService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        AckResponse StartSession(EegMeta meta);

        [OperationContract]
        [FaultContract(typeof(DataFormatFault))]
        [FaultContract(typeof(ValidationFault))]
        AckResponse PushSample(EegSample sample);

        [OperationContract]
        AckResponse EndSession();
    }
}
