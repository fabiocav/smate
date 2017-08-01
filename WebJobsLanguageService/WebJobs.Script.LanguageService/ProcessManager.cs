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
    internal class ProcessManager : IDisposable
    {
        private readonly string _fileName;
        private readonly string _arguments;
        private readonly string _workingDirectory;
        private TaskCompletionSource<object> _taskCompletionSource;
        private StreamWriter _processWriter;
        private readonly ISubject<string> _outputSubject = new Subject<string>();
        private Process _process;
        private bool _disposed = false;

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

            _process = new Process { StartInfo = startInfo };
            _process.ErrorDataReceived += ProcessDataReceived;
            _process.OutputDataReceived += ProcessDataReceived;
            _process.EnableRaisingEvents = true;

            _process.Exited += (s, e) =>
            {
                _taskCompletionSource.SetResult(_process.ExitCode == 0);
                _process.Close();
            };

            _process.Start();

            _processWriter = _process.StandardInput;

            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
        }

        public void Close()
        {
            _process?.Dispose();
            _process = null;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Close();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
