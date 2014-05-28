using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxand;
using System.Runtime.InteropServices;
using faceAnnotate_api.model;
using System.Drawing;

namespace faceAnnotate_api
{
    public class FeatureTracker
    {

        // WinAPI procedure to release HBITMAP handles returned by FSDKCam.GrabFrame
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);


        public FeatureTracker()
        {

            FSDK.ActivateLibrary("aTNQlrb9dEiYDYz/SpNbtASWMYKUtKGfBvyP6BVgKLhfOYCCj1GYS5CnrKUoFkhLAQrbT+JW8veY+NOYXXRV+vV8bNLKiBySt/f2tp0+ATVuQcrm/emDS8KXZN75oO3iQv1rWVpF4mP6eFEIROuqU8ZvW9QyfL8/D9Mf/E6urkc=");
            FSDK.InitializeLibrary();
            FSDK.SetFaceDetectionParameters(true, true, 336);

        }

        public static List<ImagePoint> GetFeaturePoints(Image faceImage)
        {

            List<ImagePoint> faceFeatureList = new List<ImagePoint>();
            try
            {
                FSDK.CImage image = new FSDK.CImage(faceImage);
                FSDK.TFacePosition facePosition = image.DetectFace();
                if (0 == facePosition.w)
                    return null;
                else
                {
                    int left = facePosition.xc - (int)(facePosition.w * 0.6f);
                    int top = facePosition.yc - (int)(facePosition.w * 0.5f);
                    //gr.DrawRectangle(Pens.LightGreen, left, top, (int)(facePosition.w * 1.2), (int)(facePosition.w * 1.2));

                    FSDK.TPoint[] facialFeatures = image.DetectFacialFeaturesInRegion(ref facePosition);
                    //int i = 0;
                    foreach (FSDK.TPoint point in facialFeatures)
                    {
                        faceFeatureList.Add(new ImagePoint(point.x, point.y));
                        //gr.DrawEllipse((++i > 2) ? Pens.LightGreen : Pens.Blue, point.x, point.y, 3, 3);
                    }

                    return faceFeatureList;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }     
    }
}
