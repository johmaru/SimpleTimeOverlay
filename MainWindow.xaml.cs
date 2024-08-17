using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TimeOverlay;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static string TimeZoneID { get; set; } = "Tokyo Standard Time";
    public MainWindow()
    {
        InitializeComponent();
        
        this.Loaded += MainWindow_OnLoaded;

        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        timer.Tick += Timer_Tick;
        timer.Start();
    }
    
    private void Timer_Tick(object sender, EventArgs e)
    {
        TimeZoneInfo tokyoTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
        DateTime tokyoTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tokyoTimeZoneInfo);
        ClockTextBlock.Text = tokyoTime.ToString("hh:mm:ss");
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        PresentationSource source = PresentationSource.FromVisual(this);
        if (source != null)
        {
            Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
            double dpiScaleX = transformToDevice.M11;
            double dpiScaleY = transformToDevice.M22;

            var screenBounds = new Rect(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight
            );

            double actualWidth = this.ActualWidth * dpiScaleX - 50;
            double actualHeight = this.ActualHeight * dpiScaleY;

            this.Left = (screenBounds.Right - actualWidth) / dpiScaleX;
            this.Top = (screenBounds.Bottom - actualHeight) / dpiScaleY;
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        DragMove();
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseRightButtonDown(e);
        Console.WriteLine("Right click");
        ContextMenu contextMenu = new ContextMenu();
        foreach (var tzi in System.TimeZoneInfo.GetSystemTimeZones())
        {
            MenuItem menuItem = new MenuItem
            {
                Header = tzi.DisplayName,
            };
            menuItem.Click += (sender, args) =>
            {
                TimeZoneID = tzi.Id;
                Timer_Tick(this, null);
            };
            contextMenu.Items.Add(menuItem);
        }
        contextMenu.IsOpen = true;
    }
}