using System;

namespace Miller.Msfs.ForeFlightRelay
{
    public class TrafficStateEventArgs : EventArgs
    {
        public TrafficStateEventArgs(TrafficState trafficState)
        {
            TrafficState = trafficState;
        }

        public TrafficState TrafficState { get; set;}
    }
}
