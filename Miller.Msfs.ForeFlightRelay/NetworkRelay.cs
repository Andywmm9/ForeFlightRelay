using Miller.Msfs.ForeFlightRelay.Packets;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Miller.Msfs.ForeFlightRelay
{
    public class NetworkRelay
    {
        private const int _port = 49002;
        private UdpClient _updClient = new UdpClient();

        public void Send(IPacket packet)
        {
            var endPoint = new IPEndPoint(IPAddress.Broadcast, _port);
            var encodedMessage = packet.Encode();
            var bytes = Encoding.ASCII.GetBytes(encodedMessage);

            _updClient.Send(bytes, bytes.Length, endPoint);
            Debug.WriteLine("ForeFlight Packet Send: {0}", new { encodedMessage });
        }
    }
}
