using EYB.FileManager;
using MahApps.Metro.Controls;
using SteamKeychain.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SteamKeychain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string JSON_FILENAME = "SteamKeys.json";
        private string _localFolder = AppDomain.CurrentDomain.BaseDirectory;

        public static MainWindow AppContext { get; private set; }

        public SteamKeyRepositoryViewModel Repository { get; set; }
        public Command CreateBackup { get; }
        public Command CopyToClipboard { get; }
        public Command OpenFolderPath { get; }

        public MainWindow()
        {
            AppContext = this;
            CreateBackup = new Command(_CreateBackup);
            CopyToClipboard = new Command(_CopyToClipboard);
            OpenFolderPath = new Command(_OpenFolderPath);
            Repository = new SteamKeyRepositoryViewModel(new JsonFile(SaveLocation.Absolute, Path.Combine(_localFolder, JSON_FILENAME)));

            _CreateBackup();

            this.InitializeComponent();
        }

        private void _CreateBackup()
        {
            var path = Path.Combine(_localFolder, JSON_FILENAME);
            if (File.Exists(path))
            {
                File.Copy(path, path + ".bak", true);
            }
        }

        private void _CopyToClipboard()
        {
            Clipboard.SetText(Repository.GeneratePlainText(false));
        }

        private void _OpenFolderPath()
        {
            Process.Start(_localFolder);
        }
    }
}
