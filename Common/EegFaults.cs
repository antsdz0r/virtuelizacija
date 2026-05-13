using System.Runtime.Serialization;

namespace Common
{
    /// <summary>Thrown when a CSV row cannot be parsed (wrong column count, bad number format, etc.).</summary>
    [DataContract]
    public class DataFormatFault
    {
        public DataFormatFault(string message, int rowIndex = -1)
        {
            Message  = message;
            RowIndex = rowIndex;
        }

        [DataMember] public string Message  { get; set; }
        [DataMember] public int    RowIndex { get; set; }
    }

    /// <summary>Thrown when a sample fails business-rule validation (out-of-range values, non-monotonic RowIndex, etc.).</summary>
    [DataContract]
    public class ValidationFault
    {
        public ValidationFault(string message, int rowIndex = -1)
        {
            Message  = message;
            RowIndex = rowIndex;
        }

        [DataMember] public string Message  { get; set; }
        [DataMember] public int    RowIndex { get; set; }
    }
}
