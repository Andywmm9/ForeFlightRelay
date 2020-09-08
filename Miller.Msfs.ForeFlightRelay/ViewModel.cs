using Miller.Msfs.ForeFlightRelay.NetworkRelays;
using Miller.Msfs.ForeFlightRelay.Packets;
using System;
using System.ComponentModel;

namespace Miller.Msfs.ForeFlightRelay
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ISimulatorConnection _simulatorConnection;
        private ForeFlightAircraftStateNetworkRelay _foreFlightPositionNetworkRelay;
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
            _simulatorConnection.SimulatorDataReceived += OnPositionReceived;
            ConnectToggleCommand = new Command(x => { ToggleConnect(); });
            ConnectionButtonText = "Connect";
            _foreFlightPositionNetworkRelay = new ForeFlightAircraftStateNetworkRelay();
        }

        public void ReceiveSimConnectMessage()
        {
            _simulatorConnection.ReceiveMessage();
        }

        public void SetWindowHandle(IntPtr hWnd)
        {
            _simulatorConnection.SetWindowHandle(hWnd);
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
            var foreFlightPositionPacket = new ForeFlightAircraftStatePacket
            {
                Altitude = eventArgs.AircraftState.Altitude,
                Latitude = eventArgs.AircraftState.Latitude,
                Longitude = eventArgs.AircraftState.Longitude,
                Track = eventArgs.AircraftState.Heading,
                Groundspeed = eventArgs.AircraftState.Groundspeed
            };

            _foreFlightPositionNetworkRelay.Send(foreFlightPositionPacket);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
