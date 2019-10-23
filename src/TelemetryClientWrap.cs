using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace finitelogic.Common.Telemetry
{
    public class TelemetryClientWrap : ITelemetryClient
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly string APPINSIGHTS_INSTRUMENTATIONKEY = "APPINSIGHTS_INSTRUMENTATIONKEY";
        public TelemetryClientWrap(IConfiguration configuration)
        {
            IConfiguration config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _telemetryClient = new TelemetryClient();
            var appinsights = config[APPINSIGHTS_INSTRUMENTATIONKEY];
            if (appinsights != null)
                 _telemetryClient.InstrumentationKey = appinsights;
        }


        // returns a metric for tracking up to 4 dimensions
        // see https://github.com/microsoft/ApplicationInsights-SDK-Labs/blob/master/Metrics/Tests/Microsoft.ApplicationInsights.Metrics.Core.Test/Examples.cs
        // for examples of how to aggregate metrics.
        // note: Configuration was not implemented so aggregation cannot occur, so no +1 -1 etc, and we can add if necessary
        public Metric GetMetric(string metricName, params string[] dimensions)
        {

            if (dimensions == null)
            {
                return _telemetryClient.GetMetric(metricName);
            }

            if (dimensions?.Length > 4)
            {
                throw new InvalidOperationException($"Dimensions on a Metric is limited to 4 - {metricName}");
            }
#pragma warning disable
            switch (dimensions.Length)
            {
                case 1:
                    return _telemetryClient.GetMetric(metricName, dimensions[0]);
                    break;
                case 2:
                    return _telemetryClient.GetMetric(metricName, dimensions[0], dimensions[1]);
                    break;
                case 3:
                    return _telemetryClient.GetMetric(metricName, dimensions[0], dimensions[1], dimensions[2]);
                    break;
                case 4:
                    return _telemetryClient.GetMetric(metricName, dimensions[0], dimensions[1], dimensions[2], dimensions[3]);
                    break;
                default:
                    throw new InvalidOperationException($"Unable to Process Dimensions {metricName}");
            }
#pragma warning restore
        }
        public void TrackException(Exception ex, IDictionary<string, string> telemetryProperties = null)
        {
            _telemetryClient.TrackException(ex, telemetryProperties);
        }

        public void TrackEvent(string eventName, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> metrics = null)
        {
            _telemetryClient.TrackEvent(eventName, telemetryProperties, metrics);
        }

        public IDictionary<string, string> TelemetryProperties(params string[] keysAndValues)
        {
            if (keysAndValues == null)
            {
                return null;
            }

            Dictionary<string, string> properties = new Dictionary<string, string>();
            for (int i = 0; i < keysAndValues.Length - 1; i++)
            {

                properties.Add(keysAndValues[i], keysAndValues[i + 1]);
            }

            return properties;

        }
    }
}
