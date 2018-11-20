using IrrKlang;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Aftermath_Studio
{
    /// <summary>
    /// Interaction logic for Preferences_Window.xaml
    /// </summary>
    public partial class Preferences_Window : Window
    {
        void docolor() 
            //this will run whenever the theme is changed. this includes this window's startup
        {
            byte r=0, g=0, b=0;
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, App.gr, App.gg, App.gb));
            if(App.gr>=20) r = Convert.ToByte(App.gr - 20);
            if (App.gg >= 20) g = Convert.ToByte(App.gg - 20);
            if (App.gb >= 20) b = Convert.ToByte(App.gb - 20);                       
            SolidColorBrush brusha = new SolidColorBrush(Color.FromArgb(255, r, g, b));
            Grid_Color.Background = brush;
            Main_Back.Background = brusha;
            Audio_S.Background = brusha;
            Appearance_S.Background = brusha;
            Audio_B.Background = brusha;
            Appearance_B.Background = brusha;
        }
        
        public Preferences_Window()
        {
            InitializeComponent();
            Buffer_box.Text = App.buffer.ToString();
            comboBox.SelectedIndex = App.color;
            docolor();
            MainWindow MW = Application.Current.Windows.OfType<MainWindow>().First<MainWindow>();
            if(MW.ResizeMode == ResizeMode.CanResize) Resize_Box.IsChecked=true;
            if (App.visualizer_enable == true) vis_checkbox.IsChecked = true;
            for (int i = 0; i < App.rdevices.Length; i++)
            {
                ComboBoxItem temp = new System.Windows.Controls.ComboBoxItem();
                temp.Content = App.rdevices[i];
                devicebox.Items.Add(temp);
            }
            devicebox.SelectedIndex = App.selecteddevice;

            for (int i = 0; i < App.pdevices.Length; i++)
            {
                ComboBoxItem temp = new System.Windows.Controls.ComboBoxItem();
                temp.Content = App.pdevices[i];
                pdevicebox.Items.Add(temp);
            }
            pdevicebox.SelectedIndex = App.pselecteddevice;
        }

        private void Audio_B_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Appearance_B_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Interface_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            devicebox.Visibility = Visibility.Collapsed;
            Device_label.Visibility = Visibility.Collapsed;
            pdevicebox.Visibility = Visibility.Collapsed;
            PDevice_label.Visibility = Visibility.Collapsed;
            Audio_S.Visibility = Visibility.Collapsed;
            Appearance_S.Visibility = Visibility.Visible;
            Buffer_box.Visibility = Visibility.Collapsed;
            Buffer_label.Visibility = Visibility.Collapsed;
            comboBox.Visibility = Visibility.Visible;
            Theme_Label.Visibility = Visibility.Visible;
            Resize_Box.Visibility = Visibility.Visible;
            Resize_Label.Visibility = Visibility.Visible;
            vis_checkbox.Visibility = Visibility.Visible;
            vis_label.Visibility = Visibility.Visible;
        }

        private void Audio_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pdevicebox.Visibility = Visibility.Visible;
            PDevice_label.Visibility = Visibility.Visible;
            devicebox.Visibility = Visibility.Visible;
            Device_label.Visibility = Visibility.Visible;
            Audio_S.Visibility = Visibility.Visible;
            Appearance_S.Visibility = Visibility.Collapsed;
            Buffer_box.Visibility = Visibility.Visible;
            Buffer_label.Visibility = Visibility.Visible;
            comboBox.Visibility = Visibility.Collapsed;
            Theme_Label.Visibility = Visibility.Collapsed;
            Resize_Box.Visibility = Visibility.Collapsed;
            Resize_Label.Visibility = Visibility.Collapsed;
            vis_checkbox.Visibility = Visibility.Collapsed;
            vis_label.Visibility = Visibility.Collapsed;
        }

        private void Buffer_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int val;        
                int.TryParse(Buffer_box.Text, out val);
                Buffer_box.Text = val.ToString();
                App.buffer = val;                           
                MainWindow MW = Application.Current.Windows.OfType<MainWindow>().First<MainWindow>();
                MW.updatelat();
                
               
            }
        }
        void writetofile()
        {
            if ((File.GetAttributes(AppDomain.CurrentDomain.BaseDirectory + "/theme.ini") & FileAttributes.ReadOnly) > 0)
            {
                MessageBox.Show("Preferences cannot be saved!\nPlease copy Aftermath Studio to a hard drive.", "Error");
                return;
                // The file is read-only (or write-protected)
            }
            try
            {
                StreamWriter tout = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/theme.ini");
                tout.WriteLine(App.color);
                if (Resize_Box.IsChecked == true) tout.WriteLine("Resize");
                else tout.WriteLine("NoResize");
                if (App.visualizer_enable == true) tout.WriteLine("1");
                else tout.WriteLine("0");
                tout.Close();
            }
            catch
            {
                MessageBox.Show("Preferences cannot be saved!\nPlease copy Aftermath Studio to a hard drive.", "Error");
                return;
                // The file is read-only (or write-protected)
            }

        }
        void savecolor()
        {
            byte r = 0, g = 0, b = 0;
            if (App.gr >= 20) r = Convert.ToByte(App.gr - 20);
            if (App.gg >= 20) g = Convert.ToByte(App.gg - 20);
            if (App.gb >= 20) b = Convert.ToByte(App.gb - 20);
            SolidColorBrush brush2 = new SolidColorBrush(Color.FromArgb(255, r, g, b));
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, App.gr, App.gg, App.gb));
            //MessageBox.Show(App.gr.ToString() + " " + App.gg.ToString() + " " + App.gb.ToString());
            MainWindow MW = Application.Current.Windows.OfType<MainWindow>().First<MainWindow>();
            MW.MainGrid.Background = brush;
            MW.Play_Frame.Background = brush;
            MW.Stop_Frame.Background = brush;
            MW.Record_Frame.Background = brush;
            MW.bpm_box.Background = brush;
            MW.Timer_Box.Background = brush;
            MW.sample_box.Background = brush;
            MW.Visualizer.Fill = brush2;
            MW.VSQ1.Fill = brush;
            MW.VSQ2.Fill = brush;
            MW.VSQ3.Fill = brush;
            MW.VSQ4.Fill = brush;

            /*
            if (Application.Current.Windows.OfType<Window1>().Count<Window1>() != 0)
            {
                Window1 Helper = Application.Current.Windows.OfType<Window1>().First<Window1>();
                Helper.Grid_Color.Background = brush;
            }
            */
        }           
        private void Resize_Box_Click(object sender, RoutedEventArgs e)
        {
            if(Resize_Box.IsChecked==true)
            {
                MainWindow MW = Application.Current.Windows.OfType<MainWindow>().First<MainWindow>();
                MW.ResizeMode = ResizeMode.CanResize;
                
            }
            else
            {
                MainWindow MW = Application.Current.Windows.OfType<MainWindow>().First<MainWindow>();
                MW.ResizeMode = ResizeMode.CanMinimize;
                MW.WindowState = WindowState.Normal;
                MW.Width = 541.5;
                MW.Height = 208;
                PrefW.Focus();
            }
        }

        private void PrefW_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            writetofile(); //treaba solida
        }

        private void Orange_Selected(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 0;
            App.color = comboBox.SelectedIndex;
            App.gr = 54;
            App.gg = 158;
            App.gb = 116;
            savecolor();
            docolor();
        }

        private void Blue_Selected(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 1;
            App.color = comboBox.SelectedIndex;
            App.gr = 103;
            App.gg = 125;
            App.gb = 162;
            savecolor();
            docolor();
        }

        private void Black_Selected(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 2;
            App.gr = 100;
            App.gg = 100;
            App.gb = 100;
            App.color = comboBox.SelectedIndex;
            savecolor();
            docolor();
        }

        private void Yellow_Selected(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 3;
            App.gr = 226;
            App.gg = 203;
            App.gb = 56;
            App.color = comboBox.SelectedIndex;
            savecolor();
            docolor();
        }

        private void Red_Selected(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 4;
            App.gr = 96;
            App.gg = 190;
            App.gb = 255;
            App.color = comboBox.SelectedIndex;
            savecolor();
            docolor();
        }

        private void Purple_Selected(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 5;
            App.gr = 222;
            App.gg = 127;
            App.gb = 192;
            App.color = comboBox.SelectedIndex;
            savecolor();
            docolor();
        }

        private void Purple2_Selected(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 6;
            App.gr = 146;
            App.gg = 103;
            App.gb = 197;
            App.color = comboBox.SelectedIndex;
            savecolor();
            docolor();
        }

        private void vis_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (vis_checkbox.IsChecked == true)
            {
                App.visualizer_enable = true;
            }
            else
            {
                App.visualizer_enable = false;
            }
        }

        private void devicebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            App.selecteddevice = devicebox.SelectedIndex;
            StreamWriter rout = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/rdevice.ini");
            rout.WriteLine(devicebox.SelectedIndex);
            rout.WriteLine(pdevicebox.SelectedIndex);
            rout.Close();

        }

        private void pdevicebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.pselecteddevice = pdevicebox.SelectedIndex;
            StreamWriter rout = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/rdevice.ini");
            rout.WriteLine(devicebox.SelectedIndex);
            rout.WriteLine(pdevicebox.SelectedIndex);
            rout.Close();
            App.aengine0 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
            App.aengine1 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
            App.aengine2 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
            App.aengine3 = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, App.getid(App.pselecteddevice));
        }
    }
}
