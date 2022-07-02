using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Interop;

namespace ForeFlightRelay.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            var viewModel = new ViewModel();

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
