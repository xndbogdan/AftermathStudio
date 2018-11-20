using IrrKlang;
using System.Windows;

namespace Aftermath_Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] arguments;
        public static int n;
        public static string location = "";
        public static string samples;
        public static int[,] a = new int[401, 4];
        public static int buffer; // latency for cpu
        public static int color;
        public static byte gr, gg, gb;
        public static int[] recorder = new int[401];
        public static string[] rdevices;
        public static string[] pdevices;
        public static ISoundEngine aengine0;
        public static ISoundEngine aengine1;
        public static ISoundEngine aengine2;
        public static ISoundEngine aengine3;
        public static IAudioRecorder rengine0;
        public static ISoundDeviceList pdeviceList = new ISoundDeviceList(SoundDeviceListType.PlaybackDevice, SoundOutputDriver.AutoDetect);
        public static int selecteddevice;
        public static int pselecteddevice;
        private void Application_Startup_1(object sender, StartupEventArgs e)
        {
            arguments = e.Args;
        } 

        public static bool visualizer_enable;
        public static string getid(int a)
        {
            string deviceID = App.pdeviceList.getDeviceID(a+1);
            return deviceID;
        }
    }
}
