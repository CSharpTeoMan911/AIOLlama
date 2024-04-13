using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AIOLlama
{
    class Ollama
    {
        private static bool OllamaRunning;
        private static Process process;

        public static void InitializeOllama(string model)
        {

            Thread thread = new Thread(() =>
            {
                StringBuilder arguments_builder = new StringBuilder();
                arguments_builder.Append("/k");
                arguments_builder.Append(" ollama run ");
                arguments_builder.Append(" llama2 ");

                process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments = arguments_builder.ToString();
                process.Start();

                OllamaRunning = true;
            });
            thread.Priority = ThreadPriority.AboveNormal;
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = false;
            thread.Start();
        }
        

        public static void ShutdownOllama()
        {
            foreach (Process p in Process.GetProcessesByName("ollama"))
                p.Kill();
            foreach (Process p in Process.GetProcessesByName("ollama app"))
                p.Kill();
        }

        public static void OllamaWriteOperation(string content)
        {
            Thread thread = new Thread(async() =>
            {
                var uri = new Uri("http://127.0.0.1:11434");
                var ollama = new OllamaApiClient(uri);

                // select a model which should be used for further operations ollama.
                ollama.SelectedModel = "llama2";
                var chat = ollama.Chat(stream => System.Diagnostics.Debug.Write(stream.Message?.Content ?? ""));
                await chat.Send(content);
            });
            thread.Priority = ThreadPriority.AboveNormal;
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = false;
            thread.Start();
        }

    }
}
