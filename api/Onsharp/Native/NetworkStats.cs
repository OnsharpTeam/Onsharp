namespace Onsharp.Native
{
   
    /// <summary>
    /// Containing information about the Onset networking.
    /// </summary>
    public struct NetworkStats
    {
        public int TotalPacketLoss { get; }

        public int LastSecondPacketLoss { get; }

        public int MessagesInResendBuffer { get; }

        public int BytesInResendBuffer { get; }

        public int BytesSend { get; }

        public int BytesReceived { get; }

        public int BytesResend { get; }

        public int TotalBytesSend { get; }

        public int TotalBytesReceived { get; }

        public bool IsLimitedByCongestionControl { get; }

        public bool IsLimitedByOutgoingBandwidthLimit { get; }

        internal NetworkStats(int totalPacketLoss, int lastSecondPacketLoss, int messagesInResendBuffer,
            int bytesInResendBuffer, int bytesSend, int bytesReceived, int bytesResend, int totalBytesSend,
            int totalBytesReceived, bool isLimitedByCongestionControl, bool isLimitedByOutgoingBandwidthLimit)
        {
            TotalPacketLoss = totalPacketLoss;
            LastSecondPacketLoss = lastSecondPacketLoss;
            MessagesInResendBuffer = messagesInResendBuffer;
            BytesInResendBuffer = bytesInResendBuffer;
            BytesSend = bytesSend;
            BytesReceived = bytesReceived;
            BytesResend = bytesResend;
            TotalBytesSend = totalBytesSend;
            TotalBytesReceived = totalBytesReceived;
            IsLimitedByCongestionControl = isLimitedByCongestionControl;
            IsLimitedByOutgoingBandwidthLimit = isLimitedByOutgoingBandwidthLimit;
        }
    }
}