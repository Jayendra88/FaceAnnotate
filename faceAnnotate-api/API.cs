using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using faceAnnotate_api.model;
using System.IO;
using System.Drawing;

namespace faceAnnotate_api
{
    public class API
    {
        public List<Person> getFileLocations(string imageFolderLocation) 
        {
            List<Person> List = new List<Person>();
            string[] filePaths = Directory.GetFiles(imageFolderLocation);
            foreach (var item in filePaths)
            {
                List.Add(new Person() { FileLocation = item });
            }
            return List;
        }

        public Person annotateFace(Person person) 
        {
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
    }
}
