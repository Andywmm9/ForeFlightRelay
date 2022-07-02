using ForeFlightRelay.Wpf.Packets;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace ForeFlightRelay.Wpf
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string _simulatorName = "Microsoft Flight Simulator";
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
            _simulatorConnection.AircraftStateDataReceived += OnPositionReceived;
            _simulatorConnection.AHRSDataReceived += OnAHRSDataReceived;
            _simulatorConnection.TrafficDataReceived += OnTrafficReceived;
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

        private void OnPositionReceived(object sender, AircraftStateEventArgs eventArgs)
        {
            var foreFlightPositionPacket = new ForeFlightAircraftStatePacket
            {
                SimulatorName = _simulatorName,
                Altitude = eventArgs.AircraftState.Altitude,
                Latitude = eventArgs.AircraftState.Latitude,
                Longitude = eventArgs.AircraftState.Longitude,
                Track = eventArgs.AircraftState.Heading,
                Groundspeed = eventArgs.AircraftState.Groundspeed
            };

            _foreFlightPositionNetworkRelay.Send(foreFlightPositionPacket);
        }

        private void OnAHRSDataReceived(object sender, AHRSDataEventArgs eventArgs)
        {
            var foreFlightAHRSPacket = new ForeFlightAHRSPacket
            {
                SimulatorName = _simulatorName,
                TrueHeading = eventArgs.AHRSData.TrueHeading,
                Pitch = -eventArgs.AHRSData.Pitch,
                Roll = -eventArgs.AHRSData.Roll
            };

            _foreFlightPositionNetworkRelay.Send(foreFlightAHRSPacket);
        }

        private void OnTrafficReceived(object sender, TrafficStateEventArgs eventArgs)
        {
            var foreFlightTrafficPacket = new ForeFlightTrafficPacket
            {
                SimulatorName = _simulatorName,
                Latitude = eventArgs.TrafficState.Latitude,
                Longitude = eventArgs.TrafficState.Longitude,
                IsAirborne = !eventArgs.TrafficState.IsOnGround,
                Velocity = eventArgs.TrafficState.Groundspeed,
                Altitude = eventArgs.TrafficState.Altitude,
                Callsign = eventArgs.TrafficState.Callsign,
                ICAOAddress = eventArgs.TrafficState.ICAOAddress,
                Heading = eventArgs.TrafficState.Heading
            };

            _foreFlightPositionNetworkRelay.Send(foreFlightTrafficPacket);
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            IsConnected = false;
            System.Windows.Application.Current.Shutdown();
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
