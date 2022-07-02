using System;

namespace ForeFlightRelay.Wpf
{
    public class AircraftStateEventArgs : EventArgs
    {
        public AircraftStateEventArgs(AircraftState aircraftState)
        {
            AircraftState = aircraftState;
        }

        public AircraftState AircraftState { get; set; }
    }
}
