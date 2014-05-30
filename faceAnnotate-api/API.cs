using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using faceAnnotate_api.model;
using System.IO;
using System.Drawing;
using System.Data;

namespace faceAnnotate_api
{
    public class API
    {
        public List<Person> getFileLocations(string[] filePaths) 
        {
            List<Person> List = new List<Person>();
            //string[] filePaths = Directory.GetFiles(imageFolderLocation);
            foreach (var item in filePaths)
            {
                List.Add(new Person() { FileLocation = item });
            }
            return List;
        }

        public Person annotateFace(Person person) 
        {
            FeatureTracker ft = new FeatureTracker();
            List<ImagePoint> imagePointList = FeatureTracker.GetFeaturePoints(person.Image);
            // annotate and drwa and keep the new image in property person.annotatedFace
            //person.AnnotatedFace = 
            return person;
        }


        public void saveImage(Person person, string destinationPath) 
        {
            person.Image.Save(Path.Combine(destinationPath, person.FileName));
        }

        public void logToTextFile(Person person) 
        {
            //person.importantImagePointList is logged to a file 
        }

        public void ExportToXL(List<Person> persons, string filePath) 
        {

            DataSet ds = XLFileWriter.CreateSampleData(persons);

            try
            {
                XLFileWriter.CreateExcelDocument(ds, filePath);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Couldn't create Excel file.\r\nException: " + ex.Message);
                return;
            }
        }
    }
}
