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

namespace AIOLlama
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int on_off;
        int maximised_or_normalised;

        SpeechRecognition speechRecogniser = new SpeechRecognition();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SpeechRecognitionOnOff(object sender, RoutedEventArgs e)
        {
            on_off++;

            switch (on_off)
            {
                case 1:
                    Ollama.InitializeOllama(String.Empty);


                    speechRecogniser.Visibility = Visibility.Hidden;
                    speechRecogniser.Show();
                    break;
                case 2:
                    speechRecogniser.CloseSpeechRecognition();
                    speechRecogniser = new SpeechRecognition();
                    on_off = 0;
                    Ollama.ShutdownOllama();
                    break;
            }
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            this?.DragMove();
        }

        private void MinimiseWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximiseOrNormaliseWindow(object sender, RoutedEventArgs e)
        {
            maximised_or_normalised++;

            switch (maximised_or_normalised)
            {
                case 1:
                    MaximiseOrNormaliseWindowButton.Content = "\xEF2F";
                    this.WindowState = WindowState.Maximized;
                    break;
                case 2:
                    MaximiseOrNormaliseWindowButton.Content = "\xEF2E";
                    this.WindowState = WindowState.Normal;
                    maximised_or_normalised = 0;
                    break;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            speechRecogniser.Close();
            Ollama.ShutdownOllama();
            this.Close();
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                InputTextbox.Width = 700;
            }
            else if(this.WindowState == WindowState.Maximized)
            {
                InputTextbox.Width = this.ActualWidth - SpeechRecognitionButton.ActualWidth - 60;
            }
        }
    }
}