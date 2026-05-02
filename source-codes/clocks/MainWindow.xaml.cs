using System.Windows;
using System.Windows.Threading;

namespace ClocksApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private DispatcherTimer _stopwatchTimer;
        private DispatcherTimer _timerTimer;
        private TimeSpan _stopwatchTime;
        private TimeSpan _timerTime;
        private bool _stopwatchRunning;
        private bool _timerRunning;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            _stopwatchTimer = new DispatcherTimer();
            _stopwatchTimer.Interval = TimeSpan.FromMilliseconds(10);
            _stopwatchTimer.Tick += StopwatchTimer_Tick;
            _timerTimer = new DispatcherTimer();
            _timerTimer.Interval = TimeSpan.FromSeconds(1);
            _timerTimer.Tick += TimerTimer_Tick;
            UpdateClock();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            ClockDisplay.Text = now.ToString("HH:mm:ss");
            DateDisplay.Text = now.ToString("dddd, d. MMMM yyyy");
        }

        private void ClockTab_Click(object sender, RoutedEventArgs e)
        {
            ClockPanel.Visibility = Visibility.Visible;
            StopwatchPanel.Visibility = Visibility.Collapsed;
            TimerPanel.Visibility = Visibility.Collapsed;
        }

        private void StopwatchTab_Click(object sender, RoutedEventArgs e)
        {
            ClockPanel.Visibility = Visibility.Collapsed;
            StopwatchPanel.Visibility = Visibility.Visible;
            TimerPanel.Visibility = Visibility.Collapsed;
        }

        private void TimerTab_Click(object sender, RoutedEventArgs e)
        {
            ClockPanel.Visibility = Visibility.Collapsed;
            StopwatchPanel.Visibility = Visibility.Collapsed;
            TimerPanel.Visibility = Visibility.Visible;
        }

        private void StopwatchTimer_Tick(object? sender, EventArgs e)
        {
            _stopwatchTime = _stopwatchTime.Add(TimeSpan.FromMilliseconds(10));
            StopwatchDisplay.Text = _stopwatchTime.ToString(@"mm\:ss\.ff");
        }

        private void StopwatchStart_Click(object sender, RoutedEventArgs e)
        {
            _stopwatchRunning = true;
            _stopwatchTimer.Start();
            StopwatchStartBtn.IsEnabled = false;
            StopwatchStopBtn.IsEnabled = true;
        }

        private void StopwatchStop_Click(object sender, RoutedEventArgs e)
        {
            _stopwatchRunning = false;
            _stopwatchTimer.Stop();
            StopwatchStartBtn.IsEnabled = true;
            StopwatchStopBtn.IsEnabled = false;
        }

        private void StopwatchReset_Click(object sender, RoutedEventArgs e)
        {
            _stopwatchRunning = false;
            _stopwatchTimer.Stop();
            _stopwatchTime = TimeSpan.Zero;
            StopwatchDisplay.Text = "00:00:00";
            StopwatchStartBtn.IsEnabled = true;
            StopwatchStopBtn.IsEnabled = false;
        }

        private void TimerTimer_Tick(object? sender, EventArgs e)
        {
            _timerTime = _timerTime.Subtract(TimeSpan.FromSeconds(1));
            if (_timerTime <= TimeSpan.Zero)
            {
                _timerTime = TimeSpan.Zero;
                _timerTimer.Stop();
                _timerRunning = false;
                TimerDisplay.Text = "00:00:00";
                TimerStartBtn.IsEnabled = true;
                TimerStopBtn.IsEnabled = false;
                System.Media.SystemSounds.Exclamation.Play();
            }
            else
            {
                TimerDisplay.Text = _timerTime.ToString(@"mm\:ss");
            }
        }

        private void TimerSet_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TimerInput.Text, out int minutes) && minutes > 0)
            {
                _timerTime = TimeSpan.FromMinutes(minutes);
                TimerDisplay.Text = _timerTime.ToString(@"mm\:ss");
            }
        }

        private void TimerStart_Click(object sender, RoutedEventArgs e)
        {
            if (_timerTime > TimeSpan.Zero)
            {
                _timerRunning = true;
                _timerTimer.Start();
                TimerStartBtn.IsEnabled = false;
                TimerStopBtn.IsEnabled = true;
            }
        }

        private void TimerStop_Click(object sender, RoutedEventArgs e)
        {
            _timerRunning = false;
            _timerTimer.Stop();
            TimerStartBtn.IsEnabled = true;
            TimerStopBtn.IsEnabled = false;
        }

        private void TimerReset_Click(object sender, RoutedEventArgs e)
        {
            _timerRunning = false;
            _timerTimer.Stop();
            _timerTime = TimeSpan.Zero;
            TimerDisplay.Text = "00:00:00";
            TimerInput.Text = "";
            TimerStartBtn.IsEnabled = true;
            TimerStopBtn.IsEnabled = false;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
