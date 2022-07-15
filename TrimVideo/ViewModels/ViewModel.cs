using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TrimVideo.ViewModels
{
    internal class ViewModel : NotifyPropertyChangedBase
    {
        private string _filePath = @"C:\Users\Flynn\Downloads\Valorant 2022.07.03 - 13.48.06.07.DVR.mp4";
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
            set { _UpdateField(ref _videoLowerBound, value); }
        }

        private TimeSpan _videoUpperBound;
        public TimeSpan VideoUpperBound
        {
            get { return _videoUpperBound; }
            set { _UpdateField(ref _videoUpperBound, value); }
        }

        private TimeSpan _videoLength;
        public TimeSpan VideoLength
        {
            get { return _videoLength; }
            set { _UpdateField(ref _videoLength, value, _ => _InitializeTimestamps()); }
        }

        private bool _isPlaying = true;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _UpdateField(ref _isPlaying, value); }
        }

        public ICommand SelectFileCommand { get; }
        public ICommand OpenFilePathCommand { get; }
        public ICommand SaveFileCommand { get; }

        public ICommand TogglePlaybackCommand { get; }

        public ViewModel ()
        {
            SelectFileCommand = new DelegateCommand(_SelectFile);
            OpenFilePathCommand = new DelegateCommand(_OpenFilePath);
            SaveFileCommand = new DelegateCommand(_SaveFile);
            TogglePlaybackCommand = new DelegateCommand(_TogglePlayback);

            Environment.GetCommandLineArgs();
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

        private void _OpenFilePath()
        {
            // Todo
        }

        private void _SaveFile()
        {
            // Todo
        }

        private void _TogglePlayback()
        {
            IsPlaying = !IsPlaying;
        }
    }
}
