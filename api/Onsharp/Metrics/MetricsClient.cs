namespace Onsharp.Metrics
{
    internal class MetricsClient : IMetrics
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        
        internal MetricsClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        internal void Connect()
        {
            
        }
    }
}