using System;
using System.Runtime.Serialization;

namespace Common
{
    /// <summary>
    /// Represents one row from an EEG CSV file.
    /// Expected CSV columns (in order):
    ///   Timestamp, AF3, T7, Pz, T8, AF4,
    ///   Attention, Engagement, Excitement, Interest, Relaxation, Stress,
    ///   Battery, ContactQuality, SlideIndex, SetIndex
    /// </summary>
    [DataContract]
    public class EegSample
    {
        // ── EEG Channels ────────────────────────────────────────────────────
        [DataMember] public DateTime Timestamp    { get; set; }
        [DataMember] public double   AF3          { get; set; }
        [DataMember] public double   T7           { get; set; }
        [DataMember] public double   Pz           { get; set; }
        [DataMember] public double   T8           { get; set; }
        [DataMember] public double   AF4          { get; set; }

        // ── Cognitive Metrics ───────────────────────────────────────────────
        [DataMember] public double   Attention    { get; set; }
        [DataMember] public double   Engagement   { get; set; }
        [DataMember] public double   Excitement   { get; set; }
        [DataMember] public double   Interest     { get; set; }
        [DataMember] public double   Relaxation   { get; set; }
        [DataMember] public double   Stress       { get; set; }

        // ── Metadata ────────────────────────────────────────────────────────
        [DataMember] public int      Battery        { get; set; }
        [DataMember] public int      ContactQuality { get; set; }
        [DataMember] public int      SlideIndex     { get; set; }
        [DataMember] public int      SetIndex       { get; set; }

        // ── Sequence ────────────────────────────────────────────────────────
        /// <summary>0-based row index within the current session (used for monotonicity check).</summary>
        [DataMember] public int      RowIndex       { get; set; }

        public override string ToString() =>
            $"[Row {RowIndex}] {Timestamp:O} | Stress={Stress:F2} Attention={Attention:F2} Battery={Battery}%";
    }
}
