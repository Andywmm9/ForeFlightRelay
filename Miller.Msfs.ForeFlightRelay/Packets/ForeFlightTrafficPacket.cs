using System;
using System.Text;

namespace Miller.Msfs.ForeFlightRelay.Packets
{
    public class ForeFlightTrafficPacket : IPacket
    {
        public string SimulatorName { get; set; }
        public int ICAOAddress { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        /// <summary>
        /// Altitude in meters MSL.
        /// </summary>
        public double Altitude { get; set; }
        /// <summary>
        /// Vertical speed in feet per minute.
        /// </summary>
        public double VerticalSpeed { get; set; }
        public bool IsAirborne { get; set; }
        /// <summary>
        /// Heading in true degrees.
        /// </summary>
        public double Heading { get; set; }
        /// <summary>
        /// Velocity in knots.
        /// </summary>
        public double Velocity { get; set; }
        public string Callsign { get; set; }

        public string Encode()
        {
            var sb = new StringBuilder();

            sb.Append("XTRAFFIC");
            sb.Append(SimulatorName);
            sb.Append(",");
            sb.Append(ICAOAddress);
            sb.Append(",");
            sb.Append(Latitude.ToString("F4"));
            sb.Append(",");
            sb.Append(Longitude.ToString("F4"));
            sb.Append(",");
            sb.Append(Altitude.ToString("F1"));
            sb.Append(",");
            sb.Append(VerticalSpeed.ToString("F1"));
            sb.Append(",");
            sb.Append(IsAirborne ? "1" : "0");
            sb.Append(",");
            sb.Append(Heading.ToString("F1"));
            sb.Append(",");
            sb.Append(Velocity.ToString("F1"));
            sb.Append(",");
            sb.Append(Callsign);

            return sb.ToString();
        }
    }
}
