using faceAnnotate_api;
using faceAnnotate_api.model;
using System;
using System.Collections.Generic;
using System.Data;
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
        public static string outputPath;

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

            BrowseOutputLocationBtn.IsEnabled = true;
            BrowseImagesBtn.IsEnabled = false;

        }

        private void GetOutputPath(object sender, RoutedEventArgs e)
        {
            Gat.Controls.OpenDialogView openDialog = new Gat.Controls.OpenDialogView();
            Gat.Controls.OpenDialogViewModel vm = (Gat.Controls.OpenDialogViewModel)openDialog.DataContext;
            vm.IsDirectoryChooser = true;
            bool? result = vm.Show();

            if (result != true) return;
            outputPath = vm.SelectedFilePath;

            StartAnnotateBtn.IsEnabled = true;
            BrowseOutputLocationBtn.IsEnabled = false;
        }
        private void StartAnnotate(object sender, RoutedEventArgs e)
        {
            if (fileNames == null || fileNames.Length == 0) return;

            API faceApi = new API();
            List<Person> persons = faceApi.getFileLocations(fileNames);

            //anotate images
            // show annotated images in the ui

            foreach (var person in persons)
            {
                //person.Image = System.Drawing.Image //{ Source = new BitmapImage() { UriSource = new Uri(person.FileLocation, UriKind.Absolute) } };
                faceApi.annotateFace(person);
            }

            StartAnnotateBtn.IsEnabled = false;
            ExportXL.IsEnabled = true;
        }


        private void ExportToXL(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Annotated Faces"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Microsoft XL Document (.xlsx)|*.xlsx"; // Filter files by extension 
            //dlg.

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                DataSet ds = XLFileWriter.CreateSampleData();

                try
                {
                    XLFileWriter.CreateExcelDocument(ds, filename);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Couldn't create Excel file.\r\nException: " + ex.Message);
                    return;
                }
            }

            

            ExportXL.IsEnabled = false;
            BrowseImagesBtn.IsEnabled = true;
        }

    }
}
