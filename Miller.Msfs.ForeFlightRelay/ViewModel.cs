using Miller.Msfs.ForeFlightRelay.Packets;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Miller.Msfs.ForeFlightRelay
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ISimulatorConnection _simulatorConnection;
        private NetworkRelay _foreFlightPositionNetworkRelay;
        private DispatcherTimer _autoConnectTimer;
        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyPropertyChanged(nameof(IsConnected));
            }
        }

        public ViewModel()
        {
            _simulatorConnection = new MsfsSimulatorConnection();
            _simulatorConnection.SimulatorDataReceived += OnPositionReceived;
            _simulatorConnection.SimulatorConnectionLost += OnConnectionLost;
            _foreFlightPositionNetworkRelay = new NetworkRelay();
            _autoConnectTimer = new DispatcherTimer();
            _autoConnectTimer.Tick += OnTryAutoConnect;
            _autoConnectTimer.Interval = new TimeSpan(0, 0, 5);
            _autoConnectTimer.Start();
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
                    IsConnected = true;
                }
                else
                {
                    _simulatorConnection.Disconnect();
                    IsConnected = false;
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

        private void OnConnectionLost(object sender, EventArgs e)
        {
            IsConnected = false;
        }

        private void OnTryAutoConnect(object sender, EventArgs e)
        {
            if (_simulatorConnection.IsConnected)
                return;

            ToggleConnect();
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
