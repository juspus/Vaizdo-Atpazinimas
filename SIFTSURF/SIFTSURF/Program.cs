using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace SIFTSURF
{
    class Program
    {
        static void Main(string[] args)
        {
           // SURF surf = new SURF(500, 4, 2, true);
            SIFT sift = new SIFT(1000);
            KeyPoint[] keiKeyPoints2 = { };
            KeyPoint[] keiKeyPoints1 = { };
            MatOfFloat desc1 = new MatOfFloat();
            MatOfFloat desc2 = new MatOfFloat();
            
            Mat logo_img =
                Cv2.ImRead(
                    @"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\SIFTSURF\SIFTSURF\arduino2.png",
                    LoadMode.GrayScale);

            Window videoWindow = new Window("Logo", WindowMode.FreeRatio);
           // surf.Run(logo_img, null, out keiKeyPoints1, desc1);
           sift.Run(logo_img, null, out keiKeyPoints1, desc1);
            BFMatcher matcher = new BFMatcher(NormType.L2);
            cameraWork(matcher, desc1, keiKeyPoints1, logo_img, sift);
        }

        static void cameraWork(BFMatcher matcher, MatOfFloat desc1, KeyPoint[] keiKeyPoints1, Mat logo_img, SIFT surf)
        {
            Mat img = new Mat();
            Mat view = new Mat();
            VideoCapture cap = new VideoCapture(0);
            Window videoWindow = new Window("video", WindowMode.FreeRatio);
            KeyPoint[] keiKeyPoints2 = { };
            MatOfFloat desc2 = new MatOfFloat();

            while (true)
            {
                cap.Read(img);

                if (img.Cols > 0)
                {
                    Cv2.CvtColor(img, img, ColorConversion.RgbToGray);
                    Cv2.FindHomography(logo_img, img, HomographyMethod.Zero, Double.PositiveInfinity, img);
                    /*surf.Run(img, null, out keiKeyPoints2, desc2);
                    DMatch[] matches = matcher.Match(desc1, desc2);
                    for (int i = 0; i < matches.Length; i++)
                    {
                        Console.WriteLine(matches[i].Distance);
                        if (matches[i].Distance < 5000)
                        {
                            Cv2.Circle(img, new Point((int)keiKeyPoints2[matches[i].TrainIdx].Pt.X,  (int) keiKeyPoints2[matches[i].TrainIdx].Pt.Y),6, Scalar.Red, 1);
                            //Cv2.DrawMatches(logo_img, keiKeyPoints1, img, keiKeyPoints2, matches, view);
                        }
                    }*/
                   // Cv2.DrawMatches(logo_img, keiKeyPoints1, img, keiKeyPoints2, matches, view);
                    videoWindow.Image = img;
                }

                if (Cv2.WaitKey(10) == 'q') { break; }
            }

            cap.Release();
            videoWindow.Close();

            Cv2.WaitKey();
        }
    }
}
