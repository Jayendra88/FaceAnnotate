using faceAnnotate_api;
using faceAnnotate_api.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace faceAnnotate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string[] fileNames;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetInputPath(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPG files|*.jpg";
            dlg.ShowDialog();
            fileNames = dlg.FileNames;

            if (fileNames == null || fileNames.Length == 0) return;

            StartAnnotateBtn.IsEnabled = true;
            BrowseImagesBtn.IsEnabled = false;

        }

        private void StartAnnotate(object sender, RoutedEventArgs e)
        {
            if (fileNames == null || fileNames.Length == 0) return;

            API faceApi = new API();
            List<Person> persons = faceApi.getFileLocations(fileNames);

        }

        private void Save(object sender, RoutedEventArgs e)
        {
            //save to xl file
        }

    }
}
