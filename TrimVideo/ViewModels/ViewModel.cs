using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrimVideo.Commands;

namespace TrimVideo.ViewModels
{
    internal class ViewModel : NotifyPropertyChangedBase
    {
        private string _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
            set { _UpdateField(ref _filePath, value); }
        }

        private TimeSpan _videoProgress;
        public TimeSpan VideoProgress
        {
            get { return _videoProgress; }
            set { _UpdateField(ref _videoProgress, value); }
        }

        private TimeSpan _videoLowerBound;
        public TimeSpan VideoLowerBound
        {
            get { return _videoLowerBound; }
            set { _UpdateField(ref _videoLowerBound, value); _RaisePropertyChanged(nameof(ClipLength)); }
        }

        private TimeSpan _videoUpperBound;
        public TimeSpan VideoUpperBound
        {
            get { return _videoUpperBound; }
            set { _UpdateField(ref _videoUpperBound, value); _RaisePropertyChanged(nameof(ClipLength)); }
        }

        private TimeSpan _videoLength;
        public TimeSpan VideoLength
        {
            get { return _videoLength; }
            set { _UpdateField(ref _videoLength, value, _ => _InitializeTimestamps()); }
        }

        public TimeSpan ClipLength { get => VideoUpperBound - VideoLowerBound; }

        private bool _isPlaying = true;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _UpdateField(ref _isPlaying, value); }
        }

        private bool _isMuted;
        public bool IsMuted
        {
            get { return _isMuted; }
            set { _UpdateField(ref _isMuted, value); }
        }

        public ICommand SelectFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand SkipFileCommand { get; }

        public ICommand TogglePlaybackCommand { get; }
        public ICommand ToggleMutedCommand { get; }

        private readonly IEnumerable<string> _otherArguments = Enumerable.Empty<string>();

        private bool _isActivated = true;
        public bool IsActivated
        {
            get { return _isActivated; }
            set { _UpdateField(ref _isActivated, value); }
        }

        public ViewModel ()
        {
            SelectFileCommand = new DelegateCommand(_SelectFile);
            SaveFileCommand = new DelegateCommand(_SaveFile);
            TogglePlaybackCommand = new DelegateCommand(_TogglePlayback);
            ToggleMutedCommand = new DelegateCommand(_ToggleMuted);
            SkipFileCommand = new DelegateCommand(_SkipFile);

            var args = Environment.GetCommandLineArgs().Skip(1);
            do
            {
                if (args.Any())
                {
                    if (Directory.Exists(args.First()))
                    {
                        args = args.Concat(Directory.GetFiles(args.First())).Skip(1);
                    }

                    if (File.Exists(args.First())) FilePath = args.First();

                    if (args.Count() > 1)
                    {
                        _otherArguments = args.Skip(1).Where(x => File.Exists(x));
                    }
                }
            } 
            while (string.IsNullOrEmpty(FilePath) && _otherArguments?.FirstOrDefault() != null);

            if (string.IsNullOrEmpty(FilePath))
            {
                IsActivated = false;
            }
        }

        private void _InitializeTimestamps ()
        {
            VideoUpperBound = VideoLength * 2 / 3;
            VideoLowerBound = VideoLength * 1 / 3;
            VideoProgress = VideoLowerBound;
        }

        private void _SelectFile()
        {
            // Todo
        }

        private void _SkipFile()
        {
            if (_otherArguments.Any())
            {
                NewInstance(_otherArguments);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void _SaveFile()
        {
            IsPlaying = false;

            Process process = new ()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $" -i \"{FilePath}\" -ss {VideoLowerBound} -to {VideoUpperBound} -c copy \"{Path.Combine(Path.GetDirectoryName(FilePath) ?? "", "_" + Path.GetFileNameWithoutExtension(FilePath) + "_Trim" + Path.GetExtension(FilePath))}\" -y",
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            process.Start();
            process.WaitForExit();

            if (_otherArguments.Any())
            {
                NewInstance(_otherArguments);
            }
        }

        public static void NewInstance(IEnumerable<string> args)
        {
            Process newInstance = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.ProcessPath,
                    Arguments = string.Join(' ', args.Select(x => $"\"{x}\"")),
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            newInstance.Start();
            Environment.Exit(0);
        }

        private void _TogglePlayback()
        {
            IsPlaying = !IsPlaying;
        }

        private void _ToggleMuted()
        {
            IsMuted = !IsMuted;
        }
    }
}
