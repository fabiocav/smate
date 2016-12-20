using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs.Script.LanguageService
{
    internal class ProcessManager
    {
        private readonly string _fileName;
        private readonly string _arguments;
        private readonly string _workingDirectory;
        private TaskCompletionSource<object> _taskCompletionSource;
        private StreamWriter _processWriter;
        private readonly ISubject<string> _outputSubject = new Subject<string>();

        public ProcessManager(string fileName)
            : this(fileName, null, null)
        {
        }

        public ProcessManager(string fileName, string arguments, string workingDirectory)
        {
            _fileName = fileName;
            _arguments = arguments;
            _workingDirectory = workingDirectory;
        }

        public void Start()
        {
            _taskCompletionSource = new TaskCompletionSource<object>();

            var startInfo = new ProcessStartInfo
            {
                FileName = _fileName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                ErrorDialog = false,
                WorkingDirectory = _workingDirectory,
                Arguments = _arguments
            };

            var process = new Process { StartInfo = startInfo };
            process.ErrorDataReceived += ProcessDataReceived;
            process.OutputDataReceived += ProcessDataReceived;
            process.EnableRaisingEvents = true;

            process.Exited += (s, e) =>
            {
                _taskCompletionSource.SetResult(process.ExitCode == 0);
                process.Close();
            };

            process.Start();

            _processWriter = process.StandardInput;

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }

        private async Task StartReading(StreamReader standardOutput)
        {
            string output = await standardOutput.ReadLineAsync();
            _outputSubject.OnNext(output);
        }

        private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                _outputSubject.OnNext(e.Data);
            }
        }

        public async Task Write(string inputData)
        {
            await _processWriter?.WriteLineAsync(inputData);
        }

        public IObservable<string> Output => _outputSubject.AsObservable();
    }
}
