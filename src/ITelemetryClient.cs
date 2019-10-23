using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;

namespace finitelogic.Common.Telemetry
{
    public interface ITelemetryClient
    {
        IDictionary<string, string> TelemetryProperties(params string[] keysAndValues);
        Metric GetMetric(string metricName, params string[] dimensions);
        void TrackEvent(string eventName, IDictionary<string, string> telemetryProperties = null, IDictionary<string, double> metrics = null);
        void TrackException(Exception ex, IDictionary<string, string> telemetryProperties = null);
    }
}