using System;

namespace Miller.Msfs.ForeFlightRelay
{
    public class AHRSDataEventArgs : EventArgs
    {
        public AHRSData AHRSData { get; set;}

        public AHRSDataEventArgs(AHRSData ahrsData)
        {
            AHRSData = ahrsData;
        }
    }
}
