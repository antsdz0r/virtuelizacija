using System;

namespace Common
{
    // ── Transfer lifecycle ───────────────────────────────────────────────────

    public class TransferStartedEventArgs : EventArgs
    {
        public TransferStartedEventArgs(EegMeta meta) { Meta = meta; }
        public EegMeta Meta { get; }
    }

    public class SampleReceivedEventArgs : EventArgs
    {
        public SampleReceivedEventArgs(EegSample sample, string participantId)
        {
            Sample        = sample;
            ParticipantId = participantId;
        }
        public EegSample Sample        { get; }
        public string    ParticipantId { get; }
    }

    public class TransferCompletedEventArgs : EventArgs
    {
        public TransferCompletedEventArgs(string participantId, int totalReceived)
        {
            ParticipantId  = participantId;
            TotalReceived  = totalReceived;
        }
        public string ParticipantId { get; }
        public int    TotalReceived { get; }
    }

    // ── Warnings ─────────────────────────────────────────────────────────────

    public class WarningRaisedEventArgs : EventArgs
    {
        public WarningRaisedEventArgs(string participantId, string message, int rowIndex = -1)
        {
            ParticipantId = participantId;
            Message       = message;
            RowIndex      = rowIndex;
            RaisedAt      = DateTime.Now;
        }
        public string   ParticipantId { get; }
        public string   Message       { get; }
        public int      RowIndex      { get; }
        public DateTime RaisedAt      { get; }
    }

    // ── Analytics ────────────────────────────────────────────────────────────

    public class StressSpikeEventArgs : EventArgs
    {
        public StressSpikeEventArgs(string participantId, DateTime timestamp, int rowIndex,
                                    double valueBefore, double valueAfter, double delta)
        {
            ParticipantId = participantId;
            Timestamp     = timestamp;
            RowIndex      = rowIndex;
            ValueBefore   = valueBefore;
            ValueAfter    = valueAfter;
            Delta         = delta;
        }
        public string   ParticipantId { get; }
        public DateTime Timestamp     { get; }
        public int      RowIndex      { get; }
        public double   ValueBefore   { get; }
        public double   ValueAfter    { get; }
        public double   Delta         { get; }
        public string   Direction     => Delta > 0 ? "UP" : "DOWN";
    }

    public class RelaxationDropEventArgs : EventArgs
    {
        public RelaxationDropEventArgs(string participantId, DateTime timestamp, int rowIndex,
                                       double valueBefore, double valueAfter, double delta)
        {
            ParticipantId = participantId;
            Timestamp     = timestamp;
            RowIndex      = rowIndex;
            ValueBefore   = valueBefore;
            ValueAfter    = valueAfter;
            Delta         = delta;
        }
        public string   ParticipantId { get; }
        public DateTime Timestamp     { get; }
        public int      RowIndex      { get; }
        public double   ValueBefore   { get; }
        public double   ValueAfter    { get; }
        public double   Delta         { get; }
    }

    public class PoorContactEventArgs : EventArgs
    {
        public PoorContactEventArgs(string participantId, int contactQuality, int rowIndex)
        {
            ParticipantId  = participantId;
            ContactQuality = contactQuality;
            RowIndex       = rowIndex;
        }
        public string ParticipantId  { get; }
        public int    ContactQuality { get; }
        public int    RowIndex       { get; }
    }

    public class LowBatteryEventArgs : EventArgs
    {
        public LowBatteryEventArgs(string participantId, int battery, int rowIndex)
        {
            ParticipantId = participantId;
            Battery       = battery;
            RowIndex      = rowIndex;
        }
        public string ParticipantId { get; }
        public int    Battery       { get; }
        public int    RowIndex      { get; }
    }

    public class TimeSkewEventArgs : EventArgs
    {
        public TimeSkewEventArgs(string participantId, int rowIndex, double skewMs)
        {
            ParticipantId = participantId;
            RowIndex      = rowIndex;
            SkewMs        = skewMs;
        }
        public string ParticipantId { get; }
        public int    RowIndex      { get; }
        public double SkewMs        { get; }
    }
}
