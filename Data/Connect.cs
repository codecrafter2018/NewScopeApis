
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Extensions.Configuration;
using System;

namespace Ultratechapis.Data
{
    public class Connect
    {
        private readonly ServiceClient _serviceClient;
        public TimeSpan ConnectionDuration { get; }

        public Connect(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Dataverse");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _serviceClient = new ServiceClient(connectionString);
            stopwatch.Stop();

            ConnectionDuration = stopwatch.Elapsed;

            if (!_serviceClient.IsReady)
            {
                throw new Exception("Failed to connect to Dataverse.");
            }
        }

        public ServiceClient Client => _serviceClient;
    }
}