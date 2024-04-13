using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AIOLlama
{
    /// <summary>
    /// Interaction logic for SpeechRecognition.xaml
    /// </summary>
    public partial class SpeechRecognition : Window
    {
        public SpeechRecognition()
        {
            InitializeComponent();
        }

        bool IsActivated;
        bool IsListening;
        int activator_index;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        StringBuilder text_builder = new StringBuilder();


        private async void WebView2_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await WebFrame.EnsureCoreWebView2Async();

                StringBuilder path_builder = new StringBuilder();

                path_builder.Append(@"file:///");

                #if DEBUG
                path_builder.Append("C:\\Users\\teodo\\source\\repos\\AIOLlama\\AIOLlama\\SpeechInterface\\index.html");
                #endif

                #if (!DEBUG)
                WebFrame.Source = new Uri(path_builder.ToString());
                #endif


                WebFrame.Source = new Uri(path_builder.ToString());
                WebFrame.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;
                WebFrame.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                
            }
            catch { }
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            string recv = e.TryGetWebMessageAsString();
            string recv_lower = recv.ToLower();
            System.Diagnostics.Debug.WriteLine("[ Browser ]: " + recv);

            if (IsActivated == true)
            {

                if (IsListening == true)
                {
                    if (stopwatch.ElapsedMilliseconds < 20000)
                    {
                        System.Diagnostics.Debug.WriteLine("[ Browser ]: " + recv);

                        if (recv.IndexOf("[ Final ]: ") == 0)
                        {
                            Ollama.OllamaWriteOperation(recv);
                        }
                    }
                    else
                    {
                        IsListening = false;
                        stopwatch.Reset();
                    }
                }
                else
                {
                    if (recv_lower.Contains("listen") == true)
                    {
                        System.Diagnostics.Debug.WriteLine("[ Listening ]");
                        activator_index = recv_lower.IndexOf("listen");
                        IsListening = true;
                        stopwatch.Start();
                    }
                }
            }

            if (recv == "[ Activated ]")
            {
                System.Diagnostics.Debug.WriteLine(recv);
                IsActivated = true;
            }
        }

        private void WebFrame_Initialized(object? sender, EventArgs e)
        {
            WebFrame.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;
        }

        private void CoreWebView2_PermissionRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2PermissionRequestedEventArgs e)
        {
            if (e.PermissionKind == Microsoft.Web.WebView2.Core.CoreWebView2PermissionKind.Microphone)
            {
                e.State = Microsoft.Web.WebView2.Core.CoreWebView2PermissionState.Allow;
                e.Handled = true;
            }
        }

        public async void CloseSpeechRecognition()
        {
            await WebFrame.CoreWebView2.ExecuteScriptAsync("stop()");
            this.Close();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WebFrame?.Dispose();
        }
    }
}
