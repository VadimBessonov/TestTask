using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using TestTask;

namespace WPFTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void SelectFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "File"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                FilePath.Text = filename;
            }

        }
        private void GetStats(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(FilePath.Text))
            {
                MessageBox.Show("File doesnt exist", "File error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            IList<LetterStats> letterStats;
            switch (SelectedStat.SelectedIndex)
            {
                case 0:
                    letterStats = TestTask.Program.SingleLetterStats(FilePath.Text);
                    break;
                case 1:
                    letterStats = TestTask.Program.DoubleLetterStats(FilePath.Text);
                    break;
                default:
                    MessageBox.Show($"unknownTabIndex - {SelectedStat.TabIndex}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new Exception("UnknownTabIndex");
            }
            StatList.Items.Clear();
            for (int i = 0; i < letterStats.Count; i++)
            {
                StatList.Items.Add($"{letterStats[i].Letter} - {letterStats[i].Count}\n");
            }
        }

    }
}
