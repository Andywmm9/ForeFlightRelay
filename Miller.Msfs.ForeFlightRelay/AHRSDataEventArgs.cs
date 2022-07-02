using System;

namespace ForeFlightRelay.Wpf
{
    public class AHRSDataEventArgs : EventArgs
    {
        public AHRSData AHRSData { get; set; }

        public AHRSDataEventArgs(AHRSData ahrsData)
        {
            AHRSData = ahrsData;
        }
    }
}
