using Miller.Msfs.ForeFlightRelay.Packets;
using System;
using System.ComponentModel;

namespace Miller.Msfs.ForeFlightRelay
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SimulatorConnection _simulatorConnection;
        private ForeFlightPositionNetworkRelay _foreFlightPositionNetworkRelay;
        private string _connectionButtonText;

        public Command ConnectToggleCommand { get; private set; }
        public string ConnectionButtonText
        {
            get { return _connectionButtonText; }
            set
            {
                _connectionButtonText = value;
                NotifyPropertyChanged(nameof(ConnectionButtonText));
            }
        }

        public ViewModel()
        {
            _simulatorConnection = new SimulatorConnection();
            _simulatorConnection.PositionReceived += OnPositionReceived;
            ConnectToggleCommand = new Command(x => { ToggleConnect(); });
            ConnectionButtonText = "Connect";
            _foreFlightPositionNetworkRelay = new ForeFlightPositionNetworkRelay();
        }

        public void ReceiveSimConnectMessage()
        {
            _simulatorConnection.ReceiveMessage();
        }

        private void ToggleConnect()
        {
            try
            {
                if (!_simulatorConnection.IsConnected)
                {
                    _simulatorConnection.Connect();
                    ConnectionButtonText = "Disconnect";
                }
                else
                {
                    _simulatorConnection.Disconnect();
                    ConnectionButtonText = "Connect";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnPositionReceived(object sender, PositionUpdatedEventArgs eventArgs)
        {
            var foreFlightPositionPacket = new ForeFlightPositionPacket();

            foreFlightPositionPacket.Altitude = eventArgs.Position.Altitude * 0.4048;  // ft to meters
            foreFlightPositionPacket.Latitude = eventArgs.Position.Latitude;
            foreFlightPositionPacket.Longitude = eventArgs.Position.Longitude;

            _foreFlightPositionNetworkRelay.Send(foreFlightPositionPacket);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
