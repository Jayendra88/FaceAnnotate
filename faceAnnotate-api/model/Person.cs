using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace faceAnnotate_api.model
{
    public class Person
    {
        private String fileLocation;

        private String fileName;

        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private List<ImagePoint> importantImagePointList;

        public List<ImagePoint> ImportantImagePointList
        {
            get { return importantImagePointList; }
            set { importantImagePointList = value; }
        }

        public String FileLocation
        {
            get { return fileLocation; }
            set 
            { 
                fileLocation = value;
                image = System.Drawing.Image.FromFile(FileLocation); 
            }
        }
        private Image image;
        
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        private Image annotatedFace;

        public Image AnnotatedFace
        {
            get { return annotatedFace; }
            set { annotatedFace = value; }
        }

    }
}
