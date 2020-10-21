using System;

namespace Miller.Msfs.ForeFlightRelay
{
    public class AircraftStateEventArgs : EventArgs
    {
        public AircraftStateEventArgs(AircraftState aircraftState)
        {
            AircraftState = aircraftState;
        }

        public AircraftState AircraftState { get; set;}
    }
}
