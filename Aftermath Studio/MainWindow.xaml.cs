using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IrrKlang;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using NAudio.Wave;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aftermath_Studio
{
    /// <summary>
    /// Studio Application made by Dumitrescu Aldo and Mosteanu Bogdan from Grigore Moisil Highschool
    /// Enjoy
    /// </summary>
    public partial class MainWindow : Window
    {

        /// </summary>
        System.Windows.Media.Effects.BlurEffect BigBlur = new System.Windows.Media.Effects.BlurEffect(); // a nice blurry effect
        static ISoundSource track1;
        static ISoundSource track2;
        static ISoundSource track3;
        static ISoundSource track4;
        static bool isplaying1 = false, isplaying2 = false, isplaying3 = false, isplaying4 = false;
        static Stopwatch syncer = new Stopwatch(); // we use this to sync the two threads :)
        //BassEngine engine = BassEngine.Instance;
        //ISpectrumPlayer vistest;
        static Microsoft.Win32.OpenFileDialog DLG = new Microsoft.Win32.OpenFileDialog();
        static Microsoft.Win32.SaveFileDialog CLG = new Microsoft.Win32.SaveFileDialog();
        static Microsoft.Win32.OpenFileDialog SLG = new Microsoft.Win32.OpenFileDialog();
        static double bpm = 120; //default bpm
        static double samples_bpm = 120; //this is a default samplepack for 120 bpm
        static double latency = 1710; //default is around 120bpm
        static int nrp = 0;
        static int cpl;
        static int plat;
        static int ep = 0;
        static int mb = 0, md = 0, ml = 0, mv = 0; /// <summary>
        /// number of sounds on each track
        /// mb - number of sounds on bass track
        /// md - number of sounds on drums track
        /// ml - number of sounds on lead track
        /// mv - number of sounds on voice track
        /// </summary>
        static bool code;
        static bool rendered = false; //i can't even remember what this did but i'd rather not remove it
        static bool render_path = false; //yes
        static bool packloaded = false; // no samplepack is loaded, so we prevent crashes when it actually is
        static float spd; //speed at which the sounds will play
        static bool pa = false;
        static bool rec = false;
        static bool recording = false; //it's false for now
        static bool repeat = false; /// <summary>
                                    /// for now this has no use
                                    /// </summary>
        static Thread audiot;
        static Thread visualizer;
        static Thread renderer;
        static float tvolume = 0.4f; //asta e degeaba
        

        //engine, open dialogue, save dialogue and latency declared here (old comment. a lot is declared here now...)
        void docolor()
            //this is ran each time the theme is changed(this includes the app startup)
        {
            
            byte r = 0, g = 0, b = 0;
            if (App.gr >= 20) r = Convert.ToByte(App.gr - 20);
            if (App.gg >= 20) g = Convert.ToByte(App.gg - 20);
            if (App.gb >= 20) b = Convert.ToByte(App.gb - 20);
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, App.gr, App.gg, App.gb));
            SolidColorBrush brush2 = new SolidColorBrush(Color.FromArgb(255, r, g, b));
            MainGrid.Background = brush;
            Play_Frame.Background = brush;
            Stop_Frame.Background = brush;
            Record_Frame.Background = brush;
            bpm_box.Background = brush;
            Timer_Box.Background = brush;
            sample_box.Background = brush;
            Visualizer.Fill = brush2;
            VSQ1.Fill = brush;
            VSQ2.Fill = brush;
            VSQ3.Fill = brush;
            VSQ4.Fill = brush;
            //vistest.GetFFTData(fftDataBuffer); //some kind of test. can't remember
            //i do remember that it went horribly wrong
        }
        public MainWindow()
        {        
                          
            InitializeComponent();
            Record_Btn.Visibility = Visibility.Collapsed;
            Record_Frame.Visibility = Visibility.Collapsed;
            int waveInDevices = WaveIn.DeviceCount;
            String [] devices = new string[waveInDevices];
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                devices[waveInDevice] +="Device "+ waveInDevice + ": "+ deviceInfo.ProductName+"\n";
            }
            App.rdevices = devices;

            waveInDevices = WaveOut.DeviceCount;
            devices = new string[waveInDevices];
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveInDevice);
                devices[waveInDevice] += "Device " + waveInDevice + ": " + deviceInfo.ProductName + "\n";
            }

            App.pdevices = devices;


            if (chkfile(AppDomain.CurrentDomain.BaseDirectory + "/rdevice.ini"))
            {
                //we're reading settings
                //the reason it's a local file is because if the user manages to break the program, 
                //the change will be easily reversible by simply deleting the ".ini" file
                StreamReader rin = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/rdevice.ini");
                string Reader;
                Reader = rin.ReadLine();
                App.selecteddevice = int.Parse(Reader);
                Reader = rin.ReadLine();
                App.pselecteddevice = int.Parse(Reader);
                rin.Close();
                
                App.aengine0 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
                App.aengine1 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
                App.aengine2 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
                App.aengine3 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));

                
                //MessageBox.Show(App.getid(App.pselecteddevice));
            }
            else
            {
                App.selecteddevice = 0;
                App.pselecteddevice = 0;
                StreamWriter rout = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/rdevice.ini");
                rout.WriteLine("0");
                rout.WriteLine("0");
                rout.Close();
                App.aengine0 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
                App.aengine1 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
                App.aengine2 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
                App.aengine3 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
            }

            
            if (App.arguments.Length != 0) // this means somebody opened an asp file with the app
            {

                //MessageBox.Show(App.arguments[0]);
                string[] tempstring = App.arguments[0].Split('\\');
                string filename = tempstring[tempstring.Length - 1];  
                string extension = Path.GetExtension(filename);
                string result = filename.Substring(0, filename.Length - extension.Length);
                
                if(extension!=".asp")
                {
                    MessageBox.Show("Opened file is not of .asp type!", "Error"); //i doubt somebody would try to open anything other than an asp file but we must expect everything
                }
                else
                {
                    try //we try reading just in case the file is corrupted
                    {                       
                        StreamReader fin = new StreamReader(App.arguments[0]);
                        int j, i = -1;
                        string Reader;
                        Reader = "cucu";
                        Reader = fin.ReadLine();
                        if (Reader != null) bpm = double.Parse(Reader);
                        updatelat();
                        bpm_box.Text = bpm.ToString();
                        while (Reader != null)
                        {
                            Reader = null;
                            Reader = fin.ReadLine();
                            if (Reader != null)
                            {
                                i++;
                                int temp = 0;
                                int contor = 0;

                                for (j = 0; j <= Reader.Length - 1; j++)
                                    if (Reader[j] != ' ')
                                    {
                                        temp = temp * 10 + Reader[j] - '0';
                                    }
                                    else
                                    {
                                        App.a[i, contor] = temp;
                                        contor++;
                                        temp = 0;
                                    }
                                nrp = i;
                            }
                            refreshscreen();
                            refreshscreen2();
                        }
                        fin.Close();
                        App.location = App.arguments[0];
                    }
                    catch
                    {
                        MessageBox.Show("File is invalid!", "Error");
                    }
                    Studio_Main_Window.Title = "Aftermath Studio - " + result;
                }
                

            }
            Render_Grid.Visibility = Visibility.Collapsed;
            BigBlur.Radius = 0;
            MainBlur.Effect = BigBlur;
            updatelat();
            //App.samples = Directory.GetCurrentDirectory() + @"/asamples/"; was enabled for testing purposes
            App.buffer = 20;
            refreshscreen2();
            /*
            if (chkfile(App.samples + "Samplepack.db"))
            {
                StreamReader samplereader = new StreamReader(App.samples + "Samplepack.db");
                samples_bpm = int.Parse(samplereader.ReadLine());
                preload(); ///caution !
            } 
            Was enabled for testing purposes   
            */
            App.aengine0.SoundVolume = tvolume;
            App.aengine1.SoundVolume = tvolume;
            App.aengine2.SoundVolume = tvolume;
            App.aengine3.SoundVolume = tvolume;
            Bass_Box.Items.Add("0");
            Bass_Box.SelectedIndex = 0;
            Drums_Box.Items.Add("0");
            Drums_Box.SelectedIndex = 0;
            Lead_Box.Items.Add("0");
            Lead_Box.SelectedIndex = 0;
            Voice_Box.Items.Add("0");
            Voice_Box.SelectedIndex = 0;

            /// temporary!!  

            if (chkfile(AppDomain.CurrentDomain.BaseDirectory + "/theme.ini"))
            {
                //we're reading settings
                //the reason it's a local file is because if the user manages to break the program, 
                //the change will be easily reversible by simply deleting the ".ini" file
                StreamReader tin = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/theme.ini");
                string Reader;
                Reader = tin.ReadLine();
                int.TryParse(Reader, out App.color);
                if (App.color == 0)
                {
                    App.gr = 54;
                    App.gg = 158;
                    App.gb = 116;
                    docolor();
                }
                else if (App.color == 1)
                {

                    App.gr = 103;
                    App.gg = 125;
                    App.gb = 162;
                    docolor();
                }
                else if (App.color == 2)
                {
                    App.gr = 100;
                    App.gg = 100;
                    App.gb = 100;
                    docolor();
                }
                else if (App.color == 3)
                {
                    App.gr = 226;
                    App.gg = 203;
                    App.gb = 56;
                    docolor();
                }
                else if (App.color == 4)
                {
                    App.gr = 96;
                    App.gg = 190;
                    App.gb = 255;
                    docolor();
                }
                else if (App.color == 5)
                {
                    App.gr = 222;
                    App.gg = 127;
                    App.gb = 192;
                    docolor();
                }
                else if (App.color == 6)
                {
                    App.gr = 146;
                    App.gg = 103;
                    App.gb = 197;
                    docolor();
                }
                Reader = tin.ReadLine();
                if (Reader == "NoResize") Studio_Main_Window.ResizeMode = ResizeMode.CanMinimize;
                else Studio_Main_Window.ResizeMode = ResizeMode.CanResize;
                Reader = tin.ReadLine();
                if (Reader == "0") App.visualizer_enable = false;
                else App.visualizer_enable = true;
                tin.Close();
            }
            else
            {
                App.color = 2;
                App.gr = 100;
                App.gg = 100;
                App.gb = 100;
                App.visualizer_enable = true;
                docolor();
                StreamWriter tout = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/theme.ini");
                tout.WriteLine("2");
                tout.WriteLine("NoResize");
                tout.WriteLine("1");
                tout.Close();
            }
        }

        public void updatelat()
        {
            
            latency = Convert.ToInt32((240 / bpm) * 1000 - App.buffer);
            bpm_box.Text = bpm.ToString();
            spd = Convert.ToSingle(bpm / samples_bpm);
            plat = Convert.ToInt32(latency / 16);
           // MessageBox.Show(latency.ToString());
        }
        private void Quit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Activity.Text == "Playing")
            {
                Add_Pattern_Btn.IsEnabled = true;
                Del_Pattern_Btn.IsEnabled = true;
                Inc_Pat.IsEnabled = true;
                Dec_Pat.IsEnabled = true;
                Current_Pat.IsEnabled = true;
                code = false;
                
                App.aengine0.StopAllSounds();
                App.aengine1.StopAllSounds();
                App.aengine2.StopAllSounds();
                App.aengine3.StopAllSounds();

            }
            Studio_Main_Window.Close();
        }

        private void Studio_Main_Window_Closed(object sender, EventArgs e)
        {
            if (Activity.Text == "Playing")
            {
                Add_Pattern_Btn.IsEnabled = true;
                Del_Pattern_Btn.IsEnabled = true;
                Inc_Pat.IsEnabled = true;
                Dec_Pat.IsEnabled = true;
                Current_Pat.IsEnabled = true;
                code = false;
                App.aengine0.StopAllSounds();
                App.aengine1.StopAllSounds();
                App.aengine2.StopAllSounds();
                App.aengine3.StopAllSounds();
            }
            Application.Current.Shutdown();
        }
        void preload()
        {
            //Loading_label.Visibility = System.Windows.Visibility.Visible;
            //we preload all the sounds from the sample pack in order for it to work ok on slow hard drives
            //on laptop hard drives it's laggy without this
            mb = 1;
            ml = 1;
            mv = 1;
            md = 1;
            bool codep = true;
            while (codep == true)
            {
                codep = false;
                if (chkfile(App.samples + "bass/" + mb.ToString() + ".wav"))
                {
                    //App.aengine0.Play2D(App.samples + "bass/" + mb.ToString() + ".wav", false, false, StreamMode.Streaming, true).Volume = 0 ; // play the sound                 
                    mb++;
                    codep = true;
                }
                if (chkfile(App.samples + "drums/" + md.ToString() + ".wav"))
                {
                    //App.aengine1.Play2D(App.samples + "drums/" + md.ToString() + ".wav",false,false, StreamMode.Streaming,true).Volume = 0 ; // play the sound                 
                    md++;
                    codep = true;
                }
                if (chkfile(App.samples + "lead/" + ml.ToString() + ".wav"))
                {
                    // App.aengine2.Play2D(App.samples + "lead/" + ml.ToString() + ".wav", false, false, StreamMode.Streaming, true).Volume = 0; // play the sound                  
                    ml++;
                    codep = true;
                }

                if (chkfile(App.samples + "voice/" + mv.ToString() + ".wav"))
                {
                    //App.aengine3.Play2D(App.samples + "voice/" + mv.ToString() + ".wav", false, false, StreamMode.Streaming, true).Volume = 0; // play the sound                  
                    mv++;
                    codep = true;
                }
            }
            List<string> data1 = new List<string>(); // list of bass sounds
            List<string> data2 = new List<string>(); // list of drums sounds
            List<string> data3 = new List<string>(); // list of lead sounds
            List<string> data4 = new List<string>(); // list of voice sounds
            mb--;
            md--;
            ml--;
            mv--;
            int max = mb;
            if (max < md) max = md;
            if (max < ml) max = ml;
            if (max < mv) max = mv;
            for (int i = 0; i <= max; i++)
            {
                if (i <= mb)
                {
                    data1.Add(i.ToString());
                    App.aengine0.AddSoundSourceFromFile(App.samples + "bass/" + i + ".wav", StreamMode.NoStreaming, true);
                }

                if (i <= md)
                {
                    data2.Add(i.ToString());
                    App.aengine1.AddSoundSourceFromFile(App.samples + "drums/" + i + ".wav", StreamMode.NoStreaming, true);
                }
                if (i <= ml)
                {
                    data3.Add(i.ToString());
                    App.aengine2.AddSoundSourceFromFile(App.samples + "lead/" + i + ".wav", StreamMode.NoStreaming, true);
                }
                if (i <= mv)
                {
                    data4.Add(i.ToString());
                    App.aengine3.AddSoundSourceFromFile(App.samples + "voice/" + i + ".wav", StreamMode.NoStreaming, true);
                }
            }
            App.aengine0.RemoveAllSoundSources();
            App.aengine1.RemoveAllSoundSources();
            App.aengine2.RemoveAllSoundSources();
            App.aengine3.RemoveAllSoundSources();
            if (packloaded == false)
            {
                Bass_Box.Items.RemoveAt(0);
                Drums_Box.Items.RemoveAt(0);
                Lead_Box.Items.RemoveAt(0);
                Voice_Box.Items.RemoveAt(0);
            }
            packloaded = true; /// now we're actually loading a sample pack.       
            Bass_Box.ItemsSource = data1;
            Drums_Box.ItemsSource = data2;
            Lead_Box.ItemsSource = data3;
            Voice_Box.ItemsSource = data4;
            B_samples.Content = mb.ToString();
            D_samples.Content = md.ToString();
            L_samples.Content = ml.ToString();
            V_samples.Content = mv.ToString();
            //Loading_label.Visibility = System.Windows.Visibility.Collapsed;
            Bass_Box.SelectedIndex = App.a[cpl, 0];
            Drums_Box.SelectedIndex = App.a[cpl, 1];
            Lead_Box.SelectedIndex = App.a[cpl, 2];
            Voice_Box.SelectedIndex = App.a[cpl, 3];
            return;
        }
        private void Open_Project_Btn_Click(object sender, RoutedEventArgs e)
        {
            DLG.Filter = "Project files|*.asp";
            DLG.FileOk += DLG_FileOk;
            if(DLG.ShowDialog()==false) return;                       
        }
        private void DLG_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
            try  //we try reading from file.
            {
                StreamReader fin = new StreamReader(DLG.FileName);
                int j, i = -1;
                string Reader;
                Reader = "cucu";
                Reader = fin.ReadLine();
                if (Reader != null) bpm = double.Parse(Reader);
                updatelat();
                bpm_box.Text = bpm.ToString();
                while (Reader != null)
                {
                    Reader = null;
                    Reader = fin.ReadLine();
                    if (Reader != null)
                    {
                        i++;
                        int temp = 0;
                        int contor = 0;

                        for (j = 0; j <= Reader.Length - 1; j++)
                            if (Reader[j] != ' ')
                            {
                                temp = temp * 10 + Reader[j] - '0';
                            }
                            else
                            {
                                App.a[i, contor] = temp;
                                contor++;
                                temp = 0;
                            }
                        nrp = i;
                    }
                    refreshscreen();
                    refreshscreen2();
                }
                fin.Close();
                string extension = Path.GetExtension(DLG.SafeFileName);
                string result = DLG.SafeFileName.Substring(0, DLG.SafeFileName.Length - extension.Length);
                Studio_Main_Window.Title = "Aftermath Studio - " + result;
                App.location = DLG.FileName;
            }
            catch //damn...it failed
            {
                MessageBox.Show("File is invalid!", "Error");          
            }
        }
        public bool chkfile(string locator)
        {
            return File.Exists(locator);
        }
        private void Timer_Box_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (Timer_Box.SelectedText != "")
            {
                e.Handled = true;
                Timer_Box.Select(0, 0);
            }
        }
        void drawitnow()
        {
            //we drawwwwwwwwwwwww....jk. we just resize squares....fun stuff....
            Thread.Sleep(2);
            VSQ1.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ1.Height = 0; }));
            VSQ2.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ2.Height = 0; }));
            VSQ3.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ3.Height = 0; }));
            VSQ4.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ4.Height = 0; }));
            while (pa == true)
            {
                if (App.visualizer_enable == true)
                {
                    if (isplaying1)
                    {
                        try
                        { 
                            VSQ1.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                if (track1 != null)
                                {
                                    if (Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10) >= track1.SampleData.Length ||  Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)<=0)
                                    {
                                        syncer.Reset();
                                        syncer.Restart();
                                        VSQ1.Height = 0;
                                    } //you dun' goofd!
                                    else VSQ1.Height = (track1.SampleData[Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)] * int.Parse(Math.Round(Visualizer.ActualHeight).ToString())) / 255;
                                }
                            }));
                        }
                        catch
                        {
                            //boom
                        }
                    }
                    else
                    {
                        VSQ1.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ1.Height = 0; }));
                    }
                    if (isplaying2)
                    {
                        VSQ2.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            if (track2 != null)
                            {
                                if (Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10) >= track2.SampleData.Length || Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)<=0)
                                {
                                    syncer.Reset();
                                    syncer.Restart();
                                    VSQ2.Height = 0;
                                } //you dun' goofd!
                                else VSQ2.Height = (track2.SampleData[Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)] * int.Parse(Math.Round(Visualizer.ActualHeight).ToString())) / 255;
                            }
                                

                        }));
                    }
                    else
                    {
                        VSQ2.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ2.Height = 0; }));
                    }
                    if (isplaying3)
                    {
                        VSQ3.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            
                            if (track3 != null)
                            {
                                if (Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10) >= track3.SampleData.Length || Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)<=0)
                                {
                                    syncer.Reset();
                                    syncer.Restart();
                                    VSQ3.Height = 0;
                                } //you dun' goofd!
                                else VSQ3.Height = (track3.SampleData[Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)] * int.Parse(Math.Round(Visualizer.ActualHeight).ToString())) / 255;
                            }
                        }));

                    }
                    else
                    {
                        VSQ3.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ3.Height = 0; }));
                    }
                    if (isplaying4)
                    {
                        VSQ4.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            if (track4 != null)
                            {
                                if (Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10) >= track4.SampleData.Length || Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)<=0)
                                {
                                    syncer.Reset();
                                    syncer.Restart();
                                    VSQ4.Height = 0;
                                } //you dun' goofd!
                                else VSQ4.Height = (track4.SampleData[Convert.ToInt64(syncer.Elapsed.TotalMilliseconds + 10)] * int.Parse(Math.Round(Visualizer.ActualHeight).ToString())) / 255;
                            }
                        }));
                    }
                    else
                    {
                        VSQ4.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ4.Height = 0; }));
                    }
                }
                else
                {
                    VSQ1.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ1.Height = 0; }));
                    VSQ2.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ2.Height = 0; }));
                    VSQ3.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ3.Height = 0; }));
                    VSQ4.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ4.Height = 0; }));
                }
                Thread.Sleep(48);
                
            }
            VSQ1.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ1.Height = 0; }));
            VSQ2.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ2.Height = 0; }));
            VSQ3.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ3.Height = 0; }));
            VSQ4.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { VSQ4.Height = 0; }));
            return;
        }
        void playitnow()
        {
            pa = true;
            Play_Btn.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Play_Btn.Source = new BitmapImage(new Uri(@"\Play_BTN_A_Hover.png", UriKind.RelativeOrAbsolute)); }));
            Current_Pat.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { cpl = int.Parse(Current_Pat.Text) - 1; }));
            Activity.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Activity.Text = "Playing"; }));

            while (cpl <= nrp && code == true)
            {

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start(); //this syncs the tracks 
                syncer.Start(); //this will sync the play thread to the visualiser render thread. fun stuff...
                bool c1 = true, c2 = true, c3 = true, c4 = true;
                if (App.a[cpl, 0] != 0 && chkfile(App.samples + "bass/" + App.a[cpl, 0].ToString() + ".wav")) //check if the sound is 0
                {
                    track1 = App.aengine0.AddSoundSourceFromFile(App.samples + "bass/" + App.a[cpl, 0].ToString() + ".wav", StreamMode.NoStreaming, true);
                    //track1.ForcedStreamingThreshold = 90000000;
                    App.aengine0.Play2D(App.samples + "bass/" + App.a[cpl, 0].ToString() + ".wav").PlaybackSpeed = spd; // play the sound
                    if (GetWavFileDuration(App.samples + "bass/" + App.a[cpl, 0].ToString() + ".wav").TotalMilliseconds > latency + plat) //check if sound is longer than one beat so it doesn't get cut off short
                        c1 = false;
                    isplaying1 = true;

                }

                if (App.a[cpl, 1] != 0 && chkfile(App.samples + "drums/" + App.a[cpl, 1].ToString() + ".wav")) //check if the sound is 0 
                {

                    track2 = App.aengine1.AddSoundSourceFromFile(App.samples + "drums/" + App.a[cpl, 1].ToString() + ".wav", StreamMode.NoStreaming, true);
                    //track2.ForcedStreamingThreshold = 90000000;
                    if(recording==false) App.aengine1.Play2D(App.samples + "drums/" + App.a[cpl, 1].ToString() + ".wav").PlaybackSpeed = spd; // play the sound
                    if (GetWavFileDuration(App.samples + "drums/" + App.a[cpl, 1].ToString() + ".wav").TotalMilliseconds > latency + plat) //check if sound is longer than one beat so it doesn't get cut off short
                        c2 = false;
                    isplaying2 = true;
                }

                if (App.a[cpl, 2] != 0 && chkfile(App.samples + "lead/" + App.a[cpl, 2].ToString() + ".wav")) //check if the sound is 0
                {

                    track3 = App.aengine2.AddSoundSourceFromFile(App.samples + "lead/" + App.a[cpl, 2].ToString() + ".wav", StreamMode.NoStreaming, true);
                    //track3.ForcedStreamingThreshold = 90000000;
                    if (recording == false) App.aengine2.Play2D(App.samples + "lead/" + App.a[cpl, 2].ToString() + ".wav").PlaybackSpeed = spd; // play the sound
                    if (GetWavFileDuration(App.samples + "lead/" + App.a[cpl, 2].ToString() + ".wav").TotalMilliseconds > latency + plat) //check if sound is longer than one beat so it doesn't get cut off short
                        c3 = false;
                    isplaying3 = true;
                }

                if (App.a[cpl, 3] != 0 && chkfile(App.samples + "voice/" + App.a[cpl, 3].ToString() + ".wav")) //check if the sound is 0
                {
                    track4 = App.aengine3.AddSoundSourceFromFile(App.samples + "voice/" + App.a[cpl, 3].ToString() + ".wav", StreamMode.NoStreaming, true);
                    //track4.ForcedStreamingThreshold = 90000000;                            
                    if (recording == false) App.aengine3.Play2D(App.samples + "voice/" + App.a[cpl, 3].ToString() + ".wav").PlaybackSpeed = spd; // play the sound
                    if (GetWavFileDuration(App.samples + "voice/" + App.a[cpl, 3].ToString() + ".wav").TotalMilliseconds > latency + plat) //check if sound is longer than one beat so it doesn't get cut off short                    
                        c4 = false;
                    isplaying4 = true;
                }

                //Timer_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Timer_Box.Text = (cpl + 1).ToString() + " .  1"; }));
                Current_Pat.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Current_Pat.Text = (cpl + 1).ToString(); }));
                Bass_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Bass_Box.Text = App.a[cpl, 0].ToString(); }));
                Drums_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Drums_Box.Text = App.a[cpl, 1].ToString(); }));
                Lead_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Lead_Box.Text = App.a[cpl, 2].ToString(); }));
                Voice_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Voice_Box.Text = App.a[cpl, 3].ToString(); }));
                Bass_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Bass_Box.IsEnabled = false; }));
                Drums_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Drums_Box.IsEnabled = false; }));
                Lead_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Lead_Box.IsEnabled = false; }));
                Voice_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Voice_Box.IsEnabled = false; }));
                ep = cpl;
                Thread.Sleep(1);
                stopwatch.Stop();

                //MessageBox.Show(stopwatch.Elapsed.TotalMilliseconds.ToString());
                plat = Convert.ToInt32((latency - Convert.ToInt64(stopwatch.Elapsed.TotalMilliseconds)) / 16);
                if (plat < 0) plat = Convert.ToInt32(latency) / 16;
                if (code == true)
                {
                    int i = 1, j = 1, k = 1;
                    while (i <= 4 && j <= 4 && k <= 4 && code == true)
                    {
                        j++;
                        if (j >= 5)
                        {

                            j = 1;
                            Timer_Box.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() => { Timer_Box.Text = (cpl + 1).ToString() + " .  " + i.ToString(); }));
                            i++;
                        }

                        Thread.Sleep(plat);
                    }
                }
                if (repeat == false) cpl++;
                if (c1)
                {
                    App.aengine0.RemoveAllSoundSources();
                    isplaying1 = false;
                }
                if (c2)
                {
                    App.aengine1.RemoveAllSoundSources();
                    isplaying2 = false;
                }
                if (c3)
                {
                    App.aengine2.RemoveAllSoundSources();
                    isplaying3 = false;
                }
                if (c4)
                {
                    App.aengine3.RemoveAllSoundSources();
                    isplaying4 = false;
                }
                syncer.Stop();
            }
            pa = false;
            Play_Btn.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Play_Btn.Source = new BitmapImage(new Uri(@"\Play_BTN.png", UriKind.RelativeOrAbsolute)); }));
            Add_Pattern_Btn.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Add_Pattern_Btn.IsEnabled = true; }));
            Del_Pattern_Btn.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Del_Pattern_Btn.IsEnabled = true; }));
            Inc_Pat.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Inc_Pat.IsEnabled = true; }));
            Dec_Pat.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Dec_Pat.IsEnabled = true; }));
            Current_Pat.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Current_Pat.IsEnabled = true; }));
            Bass_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Bass_Box.IsEnabled = true; }));
            Drums_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Drums_Box.IsEnabled = true; }));
            Lead_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Lead_Box.IsEnabled = true; }));
            Voice_Box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Voice_Box.IsEnabled = true; }));
            Samples_Btn.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Samples_Btn.IsEnabled = true; }));
            cpl = 0;
            syncer.Stop();
            Activity.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Activity.Text = "Stopped"; }));
            return;
        }
        public  void Concatenate(string outputFile, IEnumerable<string> sourceFiles)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;
            try
            {
                foreach (string sourceFile in sourceFiles) 
                {
                    using (WaveFileReader reader = new WaveFileReader(sourceFile))
                    {
                        if (waveFileWriter == null) //initialising a writer
                        {   
                            waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);
                        }
                        else
                        {
                            if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                            {
                                throw new InvalidOperationException("The wave files have different format!");
                            }
                        }
                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.Write(buffer, 0, read);
                        }
                    }
                }
            }
            finally
            {
                if (waveFileWriter != null)
                {
                    waveFileWriter.Dispose(); //now we're finally done
                }
            }

        }

        private void RLG_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            render_path = true;
        }
        public void ExecuteCommand(string command)
        {
            ProcessStartInfo ProcessInfo;
            Process Process;       
            ProcessInfo = new ProcessStartInfo("ffmpeg",command);              
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process = Process.Start(ProcessInfo);                                                      
            while(Process.HasExited==false) {  Thread.Sleep(10); }  //we must wait for each process to end so they won't overlap                      
        }
        void rendernow()
        {
            Microsoft.Win32.SaveFileDialog RLG = new Microsoft.Win32.SaveFileDialog();
            string location = "";
            RLG.Filter = "WAVE files|*.wav";
            render_path = false;
            RLG.FileOk += RLG_FileOk;
            //if(RLG.ShowDialog()==false) return;
            RLG.ShowDialog();
            if (render_path) location = RLG.FileName;
            else return;
            render_path = false;
            Exp_label.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_label.Visibility = Visibility.Visible; }));
            Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = 0; }));
            List<string> Soundsb = new List<string>();
            List<string> Soundsd = new List<string>();
            List<string> Soundsl = new List<string>();
            List<string> Soundsv = new List<string>();
            int i;
            for (i = 0; i <= nrp; i++)
            {
                if (chkfile(App.samples + "bass/" + App.a[i, 0] + ".wav") == false || chkfile(App.samples + "drums/" + App.a[i, 1] + ".wav") == false || chkfile(App.samples + "lead/" + App.a[i, 2] + ".wav") == false || chkfile(App.samples + "voice/" + App.a[i, 3] + ".wav") == false)
                {
                    MessageBox.Show("Sample Pack integrity check failed!\nCheck if the correct sample pack is selected!", "Error");
                    Exp_label.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_label.Visibility = Visibility.Collapsed; }));
                    Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = 0; }));
                    BigBlur.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { BigBlur.Radius = 0; }));
                    MainBlur.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { MainBlur.IsEnabled = true; }));
                    Render_Grid.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Render_Grid.Visibility = Visibility.Collapsed; }));
                    return;
                }
                if (i == 0) Soundsb.Add(App.samples + "bass/" + App.a[i, 0] + ".wav");
                else if (GetWavFileDuration(App.samples + "bass/" + App.a[i - 1, 0].ToString() + ".wav").TotalMilliseconds < latency + plat)
                    Soundsb.Add(App.samples + "bass/" + App.a[i, 0] + ".wav");
                if (i == 0) Soundsd.Add(App.samples + "drums/" + App.a[i, 1] + ".wav");
                else if (GetWavFileDuration(App.samples + "drums/" + App.a[i - 1, 1].ToString() + ".wav").TotalMilliseconds < latency + plat)
                    Soundsd.Add(App.samples + "drums/" + App.a[i, 1] + ".wav");
                if (i == 0) Soundsl.Add(App.samples + "lead/" + App.a[i, 2] + ".wav");
                else if (GetWavFileDuration(App.samples + "lead/" + App.a[i - 1, 2].ToString() + ".wav").TotalMilliseconds < latency + plat)
                    Soundsl.Add(App.samples + "lead/" + App.a[i, 2] + ".wav");
                if (i == 0) Soundsv.Add(App.samples + "voice/" + App.a[i, 3] + ".wav");
                else if (GetWavFileDuration(App.samples + "voice/" + App.a[i - 1, 3].ToString() + ".wav").TotalMilliseconds < latency + plat)
                    Soundsv.Add(App.samples + "voice/" + App.a[i, 3] + ".wav");
                //Concatenate(location, new List<string>() { App.samples + "bass/" + App.a[i,0] + ".wav", App.samples + "drums/" + App.a[i,1] + ".wav", App.samples + "lead/" + App.a[i,2] + ".wav", App.samples + "voice/" + App.a[i,3] + ".wav" });
                if (nrp != 0) Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = (i * 70) / nrp; }));
                else Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = 0; }));
            }
            //MessageBox.Show(Soundsl.Count.ToString());
            Concatenate(location + "bass.wav", Soundsb);
            File.SetAttributes(location + "bass.wav", File.GetAttributes(location + "bass.wav") | FileAttributes.Hidden);
            Concatenate(location + "drums.wav", Soundsd);
            File.SetAttributes(location + "drums.wav", File.GetAttributes(location + "drums.wav") | FileAttributes.Hidden);
            Concatenate(location + "lead.wav", Soundsl);
            File.SetAttributes(location + "lead.wav", File.GetAttributes(location + "lead.wav") | FileAttributes.Hidden);
            Concatenate(location + "voice.wav", Soundsv);
            File.SetAttributes(location + "voice.wav", File.GetAttributes(location + "voice.wav") | FileAttributes.Hidden);
            //WAVFile.MergeAudioFiles(mAudioFilenames, mDestFilename, mTempDir);
            string strCmdText;
            strCmdText = "-y -i " + '\u0022' + location + "bass.wav" + '\u0022' + " -i " + '\u0022' + location + "drums.wav" + '\u0022' + " -filter_complex amerge -ac 1 " + '\u0022' + location + "1.wav" + '\u0022';
            ExecuteCommand(strCmdText);
            Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = 85; }));
            File.SetAttributes(location + "1.wav", File.GetAttributes(location + "1.wav") | FileAttributes.Hidden);
            strCmdText = "-y -i " + '\u0022' + location + "lead.wav" + '\u0022' + " -i " + '\u0022' + location + "voice.wav" + '\u0022' + " -filter_complex amerge -ac 1 " + '\u0022' + location + "2.wav" + '\u0022';
            ExecuteCommand(strCmdText);
            Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = 95; }));
            File.SetAttributes(location + "2.wav", File.GetAttributes(location + "2.wav") | FileAttributes.Hidden);
            if (bpm == samples_bpm)
            {
                strCmdText = "-y -i " + '\u0022' + location + "1.wav" + '\u0022' + " -i " + '\u0022' + location + "2.wav" + '\u0022' + " -filter_complex amerge -ac 1 " + '\u0022' + location + '\u0022';
                ExecuteCommand(strCmdText);
            }
            else
            {
                strCmdText = "-y -i " + '\u0022' + location + "1.wav" + '\u0022' + " -i " + '\u0022' + location + "2.wav" + '\u0022' + " -filter_complex amerge -ac 1 " + '\u0022' + location + "output.wav" + '\u0022';
                ExecuteCommand(strCmdText);
                File.SetAttributes(location + "output.wav", File.GetAttributes(location + "output.wav") | FileAttributes.Hidden);
                strCmdText = "-y -i " + '\u0022' + location + "output.wav" + '\u0022' + " -filter:a " + '\u0022' + "asetrate=48000*" + (bpm / samples_bpm).ToString() + '\u0022' + " -vn " + '\u0022' + location + '\u0022';
                ExecuteCommand(strCmdText);
            }

            File.Delete(location + "bass.wav");
            File.Delete(location + "drums.wav");
            File.Delete(location + "lead.wav");
            File.Delete(location + "voice.wav");
            File.Delete(location + "1.wav");
            File.Delete(location + "2.wav");
            File.Delete(location + "output.wav");
            Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = 100; }));
            Exp_label.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_label.Visibility = Visibility.Collapsed; }));
            Exp_Bar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Exp_Bar.Value = 0; }));
            MessageBox.Show("Exporting finished!", "Done");
            rendered = true;
            BigBlur.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { BigBlur.Radius = 0; }));
            MainBlur.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { MainBlur.IsEnabled = true; }));
            Render_Grid.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Render_Grid.Visibility = Visibility.Collapsed; }));

            return;
        }


        public static TimeSpan GetWavFileDuration(string fileName)
        {
            WaveFileReader wf = new WaveFileReader(fileName);
            return wf.TotalTime;
        }
        void playbtn()
        {
            if (Activity.Text == "Stopped")
            {
                if (chkfile(App.samples + "Samplepack.db"))
                {
                    Add_Pattern_Btn.IsEnabled = false;
                    Del_Pattern_Btn.IsEnabled = false;
                    Inc_Pat.IsEnabled = false;
                    Dec_Pat.IsEnabled = false;
                    Current_Pat.IsEnabled = false;
                    Samples_Btn.IsEnabled = false;
                    code = true;
                    pa = true;

                    audiot = new Thread(playitnow, 0);
                    audiot.Start();
                    visualizer = new Thread(drawitnow, 0);
                    visualizer.Start();
                }
                else
                {
                    stoprec();
                    MessageBox.Show("No sample pack is currently selected!\nPlease pick a sample pack using the ''Samples'' button.", "Error");
                }
                    
            }
        }
        private void Play_Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            playbtn();
        }
        private void Stop_Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Activity.Text == "Playing")
            {

                Add_Pattern_Btn.IsEnabled = true;
                Del_Pattern_Btn.IsEnabled = true;
                Inc_Pat.IsEnabled = true;
                Dec_Pat.IsEnabled = true;
                Current_Pat.IsEnabled = true;
                code = false;
                App.aengine0.StopAllSounds();
                App.aengine1.StopAllSounds();
                App.aengine2.StopAllSounds();
                App.aengine3.StopAllSounds();
                track1 = null;
                track2 = null;
                track3 = null;
                track4 = null;
                stoprec();
            }
            else
            {
                cpl = 0;
                ep = 0;
                refreshscreen();
                Current_Pat.Text = (cpl + 1).ToString();
                Timer_Box.Text = (cpl + 1).ToString() + " .  1";
            }
        }
        void refreshscreen()
        {
            Current_Pat.Text = (ep + 1).ToString();
            Bass_Box.Text = App.a[ep, 0].ToString();
            Drums_Box.Text = App.a[ep, 1].ToString();
            Lead_Box.Text = App.a[ep, 2].ToString();
            Voice_Box.Text = App.a[ep, 3].ToString();
        }
        void refreshscreen2()
        {
            Max_Pat.Text = "/" + (nrp + 1).ToString();
        }
        private void Inc_Pat_Click(object sender, RoutedEventArgs e)
        {
            if (ep < nrp)
            {
                ep++;
                refreshscreen();
            }

        }
        private void Dec_Pat_Click(object sender, RoutedEventArgs e)
        {
            if (ep != 0)
            {
                ep--;
                refreshscreen();
            }

        }
        private void Current_Pat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int tnum;
                int.TryParse(Current_Pat.Text, out tnum);
                if (tnum > 0 && tnum <= nrp + 1)
                {
                    ep = tnum - 1;
                    refreshscreen();
                }
                else
                {
                    ep = 0;
                    refreshscreen();
                }
            }
        }
        private void Add_Pattern_Btn_Click(object sender, RoutedEventArgs e)
        {
            nrp++;
            App.a[nrp, 0] = 0;
            App.a[nrp, 1] = 0;
            App.a[nrp, 2] = 0;
            App.a[nrp, 3] = 0;
            refreshscreen2();

        }


        private void Del_Pattern_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (nrp > 0)
            {
                nrp--;
                if (ep >= nrp)
                {
                    ep = nrp;
                }
                refreshscreen();
                refreshscreen2();
            }
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            Window1 Aboutw = new Window1();
            Aboutw.ShowDialog();
        }

        private void New_Project_Btn_Click(object sender, RoutedEventArgs e)
        {
            nrp = 0;
            for (int i = 0; i <= 3; i++)
                App.a[0, i] = 0;
            App.location = "";
            ep = 0;
            refreshscreen();
            refreshscreen2();
            App.aengine0.RemoveAllSoundSources();
            App.aengine1.RemoveAllSoundSources();
            App.aengine2.RemoveAllSoundSources();
            App.aengine3.RemoveAllSoundSources();
            Studio_Main_Window.Title = "Aftermath Studio";
            if(chkfile(App.samples + "Samplepack.db"))
            {
                bpm = samples_bpm;
                updatelat();
            }
        }
        private void bpm_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double val;
                
                double.TryParse(bpm_box.Text, out val);

                if (val <= samples_bpm - 41)
                {
                    MessageBox.Show("BPM value is too low for this sample pack!", "Error");
                    bpm = samples_bpm;
                    bpm_box.Text = samples_bpm.ToString();

                }
                else if (val >= samples_bpm + 6)
                {
                    MessageBox.Show("BPM value is too high for this sample pack!", "Error");
                    bpm = samples_bpm;
                    bpm_box.Text = samples_bpm.ToString();
                }
                else bpm = val;
                updatelat();
            }
        }
        void saveall()
        {
            if (App.location == "")
            { return; }
            StreamWriter fout = new StreamWriter(App.location);
            int i, j;
            string writer;
            fout.WriteLine(bpm.ToString());
            for (i = 0; i <= nrp; i++)
            {
                writer = "";
                for (j = 0; j <= 4; j++)
                {
                    if (j <= 3) writer += App.a[i, j].ToString() + " ";
                    else writer += App.recorder[i] + " ";
                }
                    
                fout.WriteLine(writer);
            }
            fout.Close();
            MessageBox.Show("Saving complete!", "Save");
        }
        private void Save_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (App.location == "") MessageBox.Show("No project file open!\nTry ''Save As...''", "Error");
            else saveall();
        }

        private void Save_As_Btn_Click(object sender, RoutedEventArgs e)
        {
            CLG.Filter = "Project files|*.asp";
            CLG.FileOk += CLG_FileOk;
            CLG.ShowDialog();
            string extension = System.IO.Path.GetExtension(CLG.SafeFileName);
            string result = CLG.SafeFileName.Substring(0, CLG.SafeFileName.Length - extension.Length);
            Studio_Main_Window.Title = "Aftermath Studio - " + result;
            saveall();
        }

        private void CLG_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.location = CLG.FileName;
            StreamWriter fout = new StreamWriter(App.location);
            fout.Close();
        }
        private void Samples_Btn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog samplebrowser = new System.Windows.Forms.FolderBrowserDialog();
            samplebrowser.ShowDialog();         
            if (samplebrowser.SelectedPath != "")
            {
                string sampleinfo = "";
                App.samples = samplebrowser.SelectedPath + '/';
                if (chkfile(App.samples + "Samplepack.db"))
                {
                    StreamReader samplereader = new StreamReader(App.samples + "Samplepack.db");
                    samples_bpm = double.Parse(samplereader.ReadLine());
                    sample_box.Text = samples_bpm.ToString();
                    updatelat();
                    if (Studio_Main_Window.Title == "Aftermath Studio")
                    {
                        bpm_box.Text = samples_bpm.ToString();
                        sample_box.Text = samples_bpm.ToString();
                        bpm = samples_bpm;
                        updatelat();
                    }
                    string reader = " ";
                    while (reader != null)
                    {
                        reader = samplereader.ReadLine();
                        if (reader != null)
                        {
                            sampleinfo = sampleinfo + reader + "\n";
                        }
                    }
                    if (sampleinfo != "") MessageBox.Show(sampleinfo, "Sample Pack Information");
                    samplereader.Close();
                }
                else
                {
                    MessageBox.Show("Invalid sample pack folder!", "Error");
                    return;
                }
                //textBox1.Text = fd.SelectedFolder;
                //preload();
                App.aengine0.RemoveAllSoundSources();
                App.aengine1.RemoveAllSoundSources();
                App.aengine2.RemoveAllSoundSources();
                App.aengine3.RemoveAllSoundSources();
                preload();
            }
            else
            {
                return;
            }
        }

        private void SLG_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void Play_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (pa == true) Play_Btn.Source = new BitmapImage(new Uri(@"\Play_BTN_A_Hover.png", UriKind.RelativeOrAbsolute));
            else Play_Btn.Source = new BitmapImage(new Uri(@"\Play_BTN_Hover.png", UriKind.RelativeOrAbsolute));
        }
        private void Play_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (pa == true) Play_Btn.Source = new BitmapImage(new Uri(@"\Play_BTN_A.png", UriKind.RelativeOrAbsolute));
            else Play_Btn.Source = new BitmapImage(new Uri(@"\Play_BTN.png", UriKind.RelativeOrAbsolute));
        }
        private void Stop_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            Stop_Btn.Source = new BitmapImage(new Uri(@"\Stop_BTN_Hover.png", UriKind.RelativeOrAbsolute));
        }
        private void Stop_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            Stop_Btn.Source = new BitmapImage(new Uri(@"\Stop_BTN.png", UriKind.RelativeOrAbsolute));
        }
        private void bpm_box_LostFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, App.gr, App.gg, App.gb));
            bpm_box.Background = brush;
            bpm_box.Text = bpm.ToString();
        }
        private void bpm_box_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0, App.gr, App.gg, App.gb));
            bpm_box.Background = brush;
        }
        private void Drums_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            Drums_Box.Text = App.a[ep, 1].ToString();
        }

        private void Lead_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            Lead_Box.Text = App.a[ep, 2].ToString();
        }

        private void Voice_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            Voice_Box.Text = App.a[ep, 3].ToString();
        }
        void losefocus()
        {
            Keyboard.ClearFocus();
            Keyboard.Focus(LoseIt);
        }
        private void Frame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            losefocus();
        }
        private void Bass_Box_DropDownClosed(object sender, EventArgs e)
        {
            App.a[ep, 0] = int.Parse(Bass_Box.SelectedIndex.ToString());
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            losefocus();
        }

        private void Drums_Box_DropDownClosed(object sender, EventArgs e)
        {
            App.a[ep, 1] = int.Parse(Drums_Box.SelectedIndex.ToString());
        }

        private void Lead_Box_DropDownClosed(object sender, EventArgs e)
        {
            App.a[ep, 2] = int.Parse(Lead_Box.SelectedIndex.ToString());
        }

        private void Voice_Box_DropDownClosed(object sender, EventArgs e)
        {
            App.a[ep, 3] = int.Parse(Voice_Box.SelectedIndex.ToString());
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (packloaded == false)
            {
                MessageBox.Show("No sample pack loaded!", "Error");
                return;
            }
            if(chkfile(AppDomain.CurrentDomain.BaseDirectory + @"/"+"ffmpeg.exe")==false)
            {
                MessageBox.Show("Ffmpeg is missing!\nCannot export track.", "Error");
                return;
            }
            BigBlur.Radius = 10;
            MainBlur.IsEnabled = false;         
            Render_Grid.Visibility = Visibility.Visible;
            Exp_label.Visibility = Visibility.Collapsed;
        }

        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            BigBlur.Radius = 0;
            MainBlur.IsEnabled = true;
            Render_Grid.Visibility = Visibility.Collapsed;
        }

        private void Export_Btn_Click(object sender, RoutedEventArgs e)
        {
            renderer = new Thread(rendernow, 0);
            renderer.Start();         
        }

        private void Record_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            
            if (rec == true) Record_Btn.Source = new BitmapImage(new Uri(@"\Record_BTN_A_Hover.png", UriKind.RelativeOrAbsolute));
            else Record_Btn.Source = new BitmapImage(new Uri(@"\Record_BTN_Hover.png", UriKind.RelativeOrAbsolute));
        }

        private void Record_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (rec == true) Record_Btn.Source = new BitmapImage(new Uri(@"\Record_BTN_A.png", UriKind.RelativeOrAbsolute));
            else Record_Btn.Source = new BitmapImage(new Uri(@"\Record_BTN.png", UriKind.RelativeOrAbsolute));
        }
        void stoprec()
        {
            rec = false;
            Record_Btn.Source = new BitmapImage(new Uri(@"\Record_BTN.png", UriKind.RelativeOrAbsolute));
        }
        private void Record_Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (rec== false)
            {
                rec = true;
                Record_Btn.Source = new BitmapImage(new Uri(@"\Record_BTN_A.png", UriKind.RelativeOrAbsolute));
                if (pa == false)
                {
                    playbtn();
                    

                }

            }
            else
            {
                stoprec();

            }
        }

        private void Play_Btn_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            
        }

        private void Activity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
           
            if (pa==false && chkfile(App.samples + "Samplepack.db"))
            {
                stoprec();
            }
        }

        private void Menu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //losefocus();
            //i'd rather not use this anymore....
        }

        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            if (chkfile(AppDomain.CurrentDomain.BaseDirectory + "/User_Manual.pdf") == false) MessageBox.Show("Help file was deleted!\nPlease reinstall the application.", "Error");
            else System.Diagnostics.Process.Start(@"User_Manual.pdf");
        }

        private void Studio_Main_Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            VSQ1.Height = 0;
            VSQ2.Height = 0;
            VSQ3.Height = 0;
            VSQ4.Height = 0;
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            losefocus();
        }

        private void Timer_Box_GotFocus(object sender, RoutedEventArgs e)
        {
            losefocus();
        }

        private void Preferences_Btn_Click(object sender, RoutedEventArgs e)
        {
            Preferences_Window Prefs = new Preferences_Window();
            Prefs.ShowDialog();
        }
    }
}
