using System;

namespace ForeFlightRelay.Wpf
{
    public class TrafficStateEventArgs : EventArgs
    {
        public TrafficStateEventArgs(TrafficState trafficState)
        {
            TrafficState = trafficState;
        }

        public TrafficState TrafficState { get; set; }
    }
}
