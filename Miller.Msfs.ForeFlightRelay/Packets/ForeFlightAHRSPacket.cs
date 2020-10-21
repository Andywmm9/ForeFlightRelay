using System.Text;

namespace Miller.Msfs.ForeFlightRelay.Packets
{
    public class ForeFlightAHRSPacket : IPacket
    {
        public string SimulatorName { get; set; }
        /// <summary>
        /// True heading in degrees.
        /// </summary>
        public double TrueHeading { get; set; }
        /// <summary>
        /// Pitch in degrees, up is positive.
        /// </summary>
        public double Pitch { get; set; }
        /// <summary>
        /// Roll in dergrees, right is positive.
        /// </summary>
        public double Roll { get; set; }

        public string Encode()
        {
            var sb = new StringBuilder();

            sb.Append("XATT");
            sb.Append(SimulatorName);
            sb.Append(",");
            sb.Append(TrueHeading.ToString("F1"));
            sb.Append(",");
            sb.Append(Pitch.ToString("F1"));
            sb.Append(",");
            sb.Append(Roll.ToString("F1"));

            return sb.ToString();
        }
    }
}
