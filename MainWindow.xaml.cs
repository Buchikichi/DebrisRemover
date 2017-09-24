using DebrisRemoverApp.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DebrisRemoverApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string[] exclusives = {
            ".jpg", ".jpeg", ".png", "gif", "tiff", "bmp", "psd",
            "pdf", "avi", "flv", ".mov", ".mp4", ".m4v",
            ".wav",
            ".htm", ".html",
        };
        private MainWindowViewModel model = new MainWindowViewModel();

        #region Events
        private void ChooseDirectory()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                DefaultDirectory = model.ScanPath,
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }
            model.ScanPath = dialog.FileName;
        }

        private bool Excluded(string name)
        {
            bool match = false;
            var lower = name.ToLower();

            foreach (var ex in exclusives)
            {
                if (lower.Contains(ex))
                {
                    match = true;
                    break;
                }
            }
            return match;
        }

        private List<string> ListTarget()
        {
            List<string> list = new List<string>();
            var dirList = Directory.EnumerateDirectories(model.ScanPath, "*", SearchOption.AllDirectories);

            foreach (var dir in dirList)
            {
                var filelist = Directory.EnumerateFiles(dir);

                foreach (var filename in filelist)
                {
                    if (Excluded(filename))
                    {
                        continue;
                    }
                    list.Add(filename);
                }
            }
            return list;
        }

        private bool IsDeletionTarget(string name)
        {
            bool match = false;
            var lower = name.ToLower();

            foreach (var target in Settings.Default.DeletionTarget)
            {
                if (lower.Contains(target.ToLower()))
                {
                    match = true;
                    break;
                }
            }
            return match;
        }

        private void RemoveDebris()
        {
            var targetList = ListTarget();

            model.AddFile("*Begin*");
            foreach (var target in targetList)
            {
                if (IsDeletionTarget(target))
                {
                    File.Delete(target);
                    continue;
                }
                model.AddFile(target);
            }
            model.AddFile("*End*");
        }
        #endregion

        #region Initialize
        private void SetupEvent()
        {
            ChooseDirectoryButton.Click += (sender, e) => ChooseDirectory();
            RemoveButton.Click += (sender, e) => RemoveDebris();
            ExitButton.Click += (sender, e) => Close();
            Closed += (sender, e) => Settings.Default.Save();
        }

        public MainWindow()
        {
            InitializeComponent();
            SetupEvent();
            DataContext = model;
        }
        #endregion
    }
}
