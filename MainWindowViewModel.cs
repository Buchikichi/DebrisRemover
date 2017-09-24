using DebrisRemoverApp.Properties;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DebrisRemoverApp
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void AddFile(string name)
        {
            var info = new FileInfo()
            {
                Name = name
            };

            FileList.Insert(0, info);
        }

        #region Properties
        public string ScanPath
        {
            get => Settings.Default.ScanPath;
            set
            {
                Settings.Default.ScanPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanPath)));
            }
        }

        public ObservableCollection<FileInfo> FileList { get; } = new ObservableCollection<FileInfo>();
        #endregion

        public class FileInfo
        {
            public string Name { get; set; }
        }
    }
}
