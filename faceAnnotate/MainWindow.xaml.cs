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
        List<Person> persons;
        API faceApi;

        public MainWindow()
        {
            InitializeComponent();
            faceApi = new API();
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

            //BrowseOutputLocationBtn.IsEnabled = true;
            StartAnnotateBtn.IsEnabled = true;
            BrowseImagesBtn.IsEnabled = false;

        }

        //private void GetOutputPath(object sender, RoutedEventArgs e)
        //{
        //    Gat.Controls.OpenDialogView openDialog = new Gat.Controls.OpenDialogView();
        //    Gat.Controls.OpenDialogViewModel vm = (Gat.Controls.OpenDialogViewModel)openDialog.DataContext;
        //    vm.IsDirectoryChooser = true;
        //    bool? result = vm.Show();

        //    if (result != true) return;
        //    outputPath = vm.SelectedFilePath;

        //    StartAnnotateBtn.IsEnabled = true;
        //    BrowseOutputLocationBtn.IsEnabled = false;
        //}

        private void StartAnnotate(object sender, RoutedEventArgs e)
        {
            if (fileNames == null || fileNames.Length == 0) return;
            
            persons = faceApi.getFileLocations(fileNames);

            foreach (var person in persons)
            {
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
            Nullable<bool> result = dlg.ShowDialog();

            //persons = getSamplePersons();


            if (result == true)
            {
                string filePath = dlg.FileName;
                if (persons == null|| persons.Count == 0) return;
                faceApi.ExportToXL(persons, filePath); // export to xl file
            }

            ExportXL.IsEnabled = false;
            BrowseImagesBtn.IsEnabled = true;
        }

        //private List<Person> getSamplePersons()
        //{
        //    List<Person> list = new List<Person>();
        //    list.Add(new Person() { ImportantImagePointList = getFeaturePoints(10), FileLocation = @"C:\Users\jayendra\Desktop\img\1.jpg" });
        //    list.Add(new Person() { ImportantImagePointList = getFeaturePoints(8), FileLocation = @"C:\Users\jayendra\Desktop\img\1.jpg" });
        //    list.Add(new Person() { ImportantImagePointList = getFeaturePoints(15), FileLocation = @"C:\Users\jayendra\Desktop\img\1.jpg" });
        //    list.Add(new Person() { ImportantImagePointList = getFeaturePoints(12), FileLocation = @"C:\Users\jayendra\Desktop\img\1.jpg" });
        //    list.Add(new Person() { ImportantImagePointList = getFeaturePoints(9), FileLocation = @"C:\Users\jayendra\Desktop\img\1.jpg" });
        //    list.Add(new Person() { ImportantImagePointList = getFeaturePoints(10), FileLocation = @"C:\Users\jayendra\Desktop\img\1.jpg" });
        //    return list;
        //}

        //private List<ImagePoint> getFeaturePoints(int p)
        //{
        //    List<ImagePoint> points = new List<ImagePoint>();

        //    for (int i = 0; i < p; i++) 
        //    {
        //        points.Add(new ImagePoint(i,p-1));
        //    }

        //        return points;
        //}

    }
}
