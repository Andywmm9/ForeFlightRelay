using System;
using System.Windows;
using System.Windows.Interop;

namespace Miller.Msfs.ForeFlightRelay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var viewModel = new ViewModel();

            viewModel.PropertyChanged += (sender, e) => {
                if (e.PropertyName == nameof(ViewModel.IsConnected))
                {
                    if (!viewModel.IsConnected) System.Windows.Application.Current.Shutdown();
                }
            };

            DataContext = viewModel;

            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            GetHWinSource().AddHook(WndProc);
            ((ViewModel)DataContext).SetWindowHandle(GetHWinSource().Handle);
        }

        protected HwndSource GetHWinSource()
        {
            return PresentationSource.FromVisual(this) as HwndSource;
        }

        private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
        {
            ((ViewModel)DataContext).ReceiveSimConnectMessage();

            return IntPtr.Zero;
        }
    }
}
