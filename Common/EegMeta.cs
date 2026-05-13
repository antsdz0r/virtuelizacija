using System.Runtime.Serialization;

namespace Common
{
    /// <summary>
    /// Metadata header sent once at the beginning of each EEG session.
    /// ParticipantId is extracted from the CSV filename (subject_X_results.csv → X).
    /// </summary>
    [DataContract]
    public class EegMeta
    {
        public EegMeta(string participantId, string fileName, int totalRows, string schemaVersion = "1.0")
        {
            ParticipantId  = participantId;
            FileName       = fileName;
            TotalRows      = totalRows;
            SchemaVersion  = schemaVersion;
        }

        [DataMember] public string ParticipantId  { get; set; }
        [DataMember] public string FileName       { get; set; }
        [DataMember] public int    TotalRows      { get; set; }
        [DataMember] public string SchemaVersion  { get; set; }

        public override string ToString() =>
            $"[EegMeta] Participant={ParticipantId}, File={FileName}, Rows={TotalRows}";
    }
}
