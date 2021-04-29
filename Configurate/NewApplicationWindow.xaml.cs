using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Configurate.Tools;
using System.ComponentModel;

using Microsoft.Win32;

namespace Configurate
{
    /// <summary>
    /// Lógica interna para NewApplicationWindow.xaml
    /// </summary>
    public partial class NewApplicationWindow : Window
    {
        public delegate void OnApplicationsChangedDelegate();
        public static OnApplicationsChangedDelegate OnApplicationsChanged;

        public NewApplicationWindow()
        {
            InitializeComponent();
        }

        public void OnWindowExit(object sender, CancelEventArgs e)
        {
            Application.Current.MainWindow.IsHitTestVisible = true;
        }

        private void SearchForApplicationPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "Application Path",
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                PathTextBox.Text = dialog.FileName;

                string extension = System.IO.Path.GetExtension(dialog.FileName).ToLower();

                switch (extension)
                {
                    case ".ini":
                        ParserTextBox.Text = "ini";
                        SaverTextBox.Text = "ini";
                        break;
                    case ".json":
                        ParserTextBox.Text = "json";
                        SaverTextBox.Text = "json";
                        break;
                    case ".xml":
                        ParserTextBox.Text = "xml";
                        SaverTextBox.Text = "xml";
                        break;
                    default:
                        ParserTextBox.Text = "Format not recognized... Try \"ini\", \"json\", \"xml\" or use a custom parser here!";
                        SaverTextBox.Text = "Format not recognized... Try \"ini\", \"json\", \"xml\" or use a custom saver here!";
                        break;
                }
            }
        }

        private void CreateNewApplication(object sender, RoutedEventArgs e)
        {
            string setupFile = $"{Defaults.CONFIGURATE}\\Setup.ini";

            if (!File.Exists(setupFile))
            {
                MessageBox.Show("Setup.ini file does not exist!", "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a name!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(PathTextBox.Text))
            {
                MessageBox.Show("Please enter a path!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(ParserTextBox.Text))
            {
                MessageBox.Show("Please enter a parser!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SaverTextBox.Text))
            {
                MessageBox.Show("Please enter a saver!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (StreamWriter sw = File.AppendText(setupFile))
            {
                sw.WriteLine("\n");
                sw.WriteLine($"[{NameTextBox.Text}]");
                sw.WriteLine("Active=true");
                sw.WriteLine($"Path={PathTextBox.Text}");
                sw.WriteLine("Description=Change in Setup.ini");
                sw.WriteLine("Developer=Change in Setup.ini");
                sw.WriteLine("Publisher=Change in Setup.ini");
                sw.WriteLine("Release Date=Change in Setup.ini");
                sw.WriteLine($"Parser={ParserTextBox.Text}");
                sw.WriteLine($"Saver={SaverTextBox.Text}");
            }

            MessageBox.Show("Successfully added new application: " + NameTextBox.Text, "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            OnApplicationsChanged?.Invoke();
            Close();
        }
    }
}
