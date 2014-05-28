using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faceAnnotate_api.model
{
    public class ImagePoint
    {
        public ImagePoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
