using System.Text;

namespace Miller.Msfs.ForeFlightRelay.Packets
{
    public class ForeFlightPositionPacket
    {
        private const string _simulatorName = "Microsoft Flight Simulator";

        public double Longitude { get; set;}
        public double Latitude { get; set;}
        /// <summary>
        /// Altitude in meters MSL.
        /// </summary>
        public double Altitude { get; set;}
        /// <summary>
        /// Track-along-ground from true north, positive value.
        /// </summary>
        public double Track { get; set;}
        /// <summary>
        /// Groundspeed in meteres/sec.
        /// </summary>
        public double Groundspeed { get; set;}

        /// <summary>
        /// Returns a string format of the packet data to send over the network.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            var sb = new StringBuilder();

            sb.Append("XGPS");
            sb.Append(_simulatorName);
            sb.Append(",");
            sb.Append(Longitude.ToString("F2"));
            sb.Append(",");
            sb.Append(Latitude.ToString("F2"));
            sb.Append(",");
            sb.Append(Altitude.ToString("F1"));
            sb.Append(",");
            sb.Append(Track.ToString("F2"));
            sb.Append(",");
            sb.Append(Groundspeed.ToString("F1"));

            return sb.ToString();
        }
    }
}
