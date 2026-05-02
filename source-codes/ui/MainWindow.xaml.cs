using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;

namespace ui
{
    public partial class MainWindow : Window
    {
        // Oprava CS8618: Inicializace timerů
        private DispatcherTimer _clockTimer = new DispatcherTimer();
        private DispatcherTimer _windowCheckTimer = new DispatcherTimer();
        private GlobalSystemMediaTransportControlsSessionManager? _sessionManager;
        private List<WindowInfo> _windowInfoList = new List<WindowInfo>();

        public class WindowInfo
        {
            public string Title { get; set; } = "";
            public IntPtr Hwnd { get; set; }
            public BitmapSource? Icon { get; set; }
            public DateTime ProcessStartTime { get; set; }
        }

        #region Win32 API
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private const int SW_RESTORE = 9;
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;

        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;
        private const uint WS_EX_TOOLWINDOW = 0x00000080;
        private const uint WS_EX_APPWINDOW = 0x00040000;
        private const uint WS_VISIBLE = 0x10000000;
        private const uint WS_MINIMIZE = 0x20000000;

        private const uint GW_OWNER = 4;

        private const int GCLP_HICON = -14;
        private const int GCLP_HICONSM = -34;
        private const uint WM_GETICON = 0x007F;
        private const uint ICON_SMALL = 0;
        private const uint ICON_BIG = 1;
        private const uint ICON_SMALL2 = 2;

        private const int DWMWA_CLOAKED = 14;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            VerifyLogoLoaded();
            PositionAtBottom();
            
            // Inicializace hodin
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();

            // Inicializace kontroly oken
            _windowCheckTimer.Interval = TimeSpan.FromSeconds(2);
            _windowCheckTimer.Tick += (s, e) => UpdateOpenWindows();
            _windowCheckTimer.Start();
            UpdateOpenWindows();

            UpdateMediaInfo();
        }

        private void VerifyLogoLoaded()
        {
            if (MenuLogo.Source == null)
            {
                System.Diagnostics.Debug.WriteLine("Logo failed to load - using fallback");
                MenuButton.Content = "NOS";
            }
        }

        private void MenuLogo_ImageFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Logo ImageFailed: {e.ErrorException?.Message}");
            MenuLogo.Source = null;
            MenuButton.Content = "NOS";
        }

        private void PositionAtBottom()
        {
            this.Left = 0;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height;
            this.Width = SystemParameters.PrimaryScreenWidth;
        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            var cz = new System.Globalization.CultureInfo("cs-CZ");
            
            ClockText.Text = now.ToString("HH:mm");
            DateText.Text = now.ToString("dd/MM");
            
            PopupClockText.Text = now.ToString("HH:mm");
            PopupDateText.Text = now.ToString("dddd, d. MMMM yyyy", cz);
            PopupDayOfYearText.Text = $"Den {now.DayOfYear} z {(DateTime.IsLeapYear(now.Year) ? 366 : 365)}";
        }

        private void UpdateOpenWindows()
        {
            _windowInfoList.Clear();
            IntPtr shellWindow = GetShellWindow();

            EnumWindows((hWnd, lParam) =>
            {
                // Skip our own window
                if (hWnd == new WindowInteropHelper(this).Handle)
                    return true;

                // Skip shell/desktop window
                if (hWnd == shellWindow)
                    return true;

                // Check if window is visible
                if (!IsWindowVisible(hWnd))
                    return true;

                // Get window text
                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                    return true;

                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(hWnd, sb, sb.Capacity);
                string title = sb.ToString().Trim();

                if (string.IsNullOrEmpty(title))
                    return true;

                // Use Windows taskbar logic: check window styles
                if (!ShouldShowOnTaskbar(hWnd))
                    return true;

                DateTime processStartTime = GetProcessStartTime(hWnd);

                _windowInfoList.Add(new WindowInfo {
                    Title = title,
                    Hwnd = hWnd,
                    Icon = GetWindowIcon(hWnd),
                    ProcessStartTime = processStartTime
                });

                return true;
            }, IntPtr.Zero);

            // Sort by process start time: oldest first (windows opened earlier appear first in list)
            var sortedList = _windowInfoList
                .OrderBy(w => w.ProcessStartTime)
                .ThenBy(w => w.Hwnd.ToInt64())
                .ToList();

            Dispatcher.Invoke(() =>
            {
                OpenWindowsList.ItemsSource = sortedList;
            });
        }

        private bool ShouldShowOnTaskbar(IntPtr hWnd)
        {
            // Get window styles
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            int style = GetWindowLong(hWnd, GWL_STYLE);
            IntPtr owner = GetWindow(hWnd, GW_OWNER);

            // Check if window is cloaked (hidden by DWM, used by system/UWP windows)
            if (IsWindowCloaked(hWnd))
                return false;

            // Check class name for known system windows
            if (IsSystemWindowClass(hWnd))
                return false;

            // Windows taskbar rules:
            bool isToolWindow = (exStyle & WS_EX_TOOLWINDOW) != 0;
            bool hasOwner = owner != IntPtr.Zero;
            bool isAppWindow = (exStyle & WS_EX_APPWINDOW) != 0;
            bool isVisible = (style & WS_VISIBLE) != 0;
            bool isMinimized = (style & WS_MINIMIZE) != 0;

            // Must be visible
            if (!isVisible)
                return false;

            // Tool windows never appear on taskbar
            if (isToolWindow)
                return false;

            // Windows with an owner (child windows) don't appear unless explicitly marked as app window
            if (hasOwner && !isAppWindow)
                return false;

            // Skip windows that are not normal application windows
            // Some additional style checks
            const uint WS_EX_NOACTIVATE = 0x08000000;
            
            bool noActivate = (exStyle & WS_EX_NOACTIVATE) != 0;
            if (noActivate)
                return false;

            return true;
        }

        private bool IsWindowCloaked(IntPtr hWnd)
        {
            try
            {
                int cloaked;
                int result = DwmGetWindowAttribute(hWnd, DWMWA_CLOAKED, out cloaked, sizeof(int));
                return result == 0 && cloaked != 0;
            }
            catch
            {
                return false;
            }
        }

        private bool IsSystemWindowClass(IntPtr hWnd)
        {
            try
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                string classStr = className.ToString().ToLowerInvariant();

                // Known system window classes that should not appear on taskbar
                string[] systemClasses = new[]
                {
                    "shell_traywnd",
                    "progman",
                    "workerw",
                    "traynotifywnd",
                    "syspager",
                    "msctfime ui",
                    "ime",
                    "buttontraywindow",
                    "tooltips_32",
                    "tooltips_ex32",
                    "xaml:window",
                    "windows.UI.Core.CoreWindow",
                    "ApplicationManager",
                    "LauncherTipWnd",
                    "LauncherTipWndClass",
                    "DUI",
                    "Button",
                    "Static",
                    "mscandui",
                    "SysShadow",
                    "NVIDIA_Share",
                    "NVIDIA_GeForce",
                    "CEF-"
                };

                foreach (var sysClass in systemClasses)
                {
                    if (classStr.Contains(sysClass.ToLowerInvariant()))
                        return true;
                }

                // Check for notification area windows
                if (classStr.StartsWith("notifyicon") || classStr.StartsWith("notification"))
                    return true;
            }
            catch { }

            return false;
        }

        private BitmapSource? GetWindowIcon(IntPtr hWnd)
        {
            try
            {
                // Try multiple methods to get the window icon, in order of preference
                IntPtr hIcon = GetWindowIconHandle(hWnd);
                
                if (hIcon != IntPtr.Zero)
                {
                    var source = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    source.Freeze();
                    DestroyIcon(hIcon);
                    return source;
                }

                // Fallback: try to get icon from executable
                return GetExeIcon(hWnd);
            }
            catch { return null; }
        }

        private IntPtr GetWindowIconHandle(IntPtr hWnd)
        {
            IntPtr hIcon = IntPtr.Zero;

            // Method 1: Get small icon from window class (16x16, best for taskbar)
            hIcon = GetClassLongPtr(hWnd, GCLP_HICONSM);
            if (hIcon != IntPtr.Zero)
                return hIcon;

            // Method 2: Get big icon from window class (32x32)
            hIcon = GetClassLongPtr(hWnd, GCLP_HICON);
            if (hIcon != IntPtr.Zero)
                return hIcon;

            // Method 3: Send WM_GETICON message (many modern apps respond to this)
            hIcon = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_SMALL, IntPtr.Zero);
            if (hIcon != IntPtr.Zero)
                return hIcon;

            hIcon = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_SMALL2, IntPtr.Zero);
            if (hIcon != IntPtr.Zero)
                return hIcon;

            hIcon = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_BIG, IntPtr.Zero);
            if (hIcon != IntPtr.Zero)
                return hIcon;

            return IntPtr.Zero;
        }

        private BitmapSource? GetExeIcon(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = System.Diagnostics.Process.GetProcessById((int)processId);
                string? exePath = process.MainModule?.FileName;
                if (string.IsNullOrEmpty(exePath))
                    return null;

                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr hSuccess = SHGetFileInfo(exePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                if (hSuccess == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
                    return null;

                var source = Imaging.CreateBitmapSourceFromHIcon(shinfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                DestroyIcon(shinfo.hIcon);
                return source;
            }
            catch { return null; }
        }

        private DateTime GetProcessStartTime(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = System.Diagnostics.Process.GetProcessById((int)processId);
                return process.StartTime;
            }
            catch
            {
                // Fallback: return minimum date if we cannot get process info
                return DateTime.MinValue;
            }
        }

        private async void UpdateMediaInfo()
        {
            try {
                _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                if (_sessionManager != null) {
                    _sessionManager.CurrentSessionChanged += (s, e) => UpdateMediaProperties();
                    UpdateMediaProperties();
                }
            } catch { UpdateNowPlaying("Media service unavailable"); }
        }

        private async void UpdateMediaProperties()
        {
            var session = _sessionManager?.GetCurrentSession();
            if (session != null) {
                var props = await session.TryGetMediaPropertiesAsync();
                Dispatcher.Invoke(() => UpdateNowPlaying($"{props.Title} - {props.Artist}"));
            } else {
                Dispatcher.Invoke(() => UpdateNowPlaying("No music playing"));
            }
        }

        public void UpdateNowPlaying(string songName)
        {
            // Oprava CS0103: Hledání prvku uvnitř Popupu
            if (this.FindName("MenuNowPlayingText") is TextBlock menuText)
            {
                menuText.Text = songName;
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e) => MenuPopup.IsOpen = !MenuPopup.IsOpen;

        private void DateTimeBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) => DateTimePopup.IsOpen = !DateTimePopup.IsOpen;

        private void OpenWindowItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement el && el.DataContext is WindowInfo info)
            {
                ShowWindow(info.Hwnd, SW_RESTORE);
                SetForegroundWindow(info.Hwnd);
            }
        }
    }
}