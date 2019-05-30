using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace SestasLaborasHough
{
    class Program
    {
        static void Main(string[] args)
        {
            Mat orig_img = Cv2.ImRead(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\SestasLaborasHough\769c0a5f4ecb29f951f85503a5c6d323.jpg", LoadMode.Color);
            //orig_img = matematika_tiesei(orig_img, out Mat edges);
            orig_img = matematika_apskritimui(orig_img, out Mat edges);
            cameraWork();
            Window original_show = new Window("color", WindowMode.FreeRatio);
            original_show.Image = orig_img;

            Window edges_show = new Window("edges", WindowMode.FreeRatio);
            edges_show.Image = edges;

            Cv2.WaitKey();
        }

        static Mat matematika_apskritimui(Mat color_img, out Mat edges)
        {
            edges = new Mat();
            Mat gray = new Mat();
            Cv2.CvtColor(color_img, gray, ColorConversion.RgbToGray);
            Cv2.Canny(gray, edges, 100, 100);

            CvCircleSegment[] circles = Cv2.HoughCircles(gray, HoughCirclesMethod.Gradient, 5, 600, 100, 100);

            foreach (CvCircleSegment circle in circles)
            {
                //if(circle.Radius< 110 && circle.Radius>100)
                Cv2.Circle(color_img, (int)circle.Center.X, (int)circle.Center.Y, (int) circle.Radius, Scalar.Blue, -1);
            }

            return color_img;
        }

        static Mat matematika_tiesei(Mat color_img, out Mat edges)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(color_img, gray, ColorConversion.RgbToGray);

            edges = new Mat();
            Cv2.Canny(gray, edges, 50, 50);
            //Cv2.Threshold(gray, edges, 100, 255, ThresholdType.BinaryInv);

            CvLineSegmentPolar[] lines = Cv2.HoughLines(edges, 1, Cv.PI / 360, 200, 0, 0); //houghline p greitesnis

            foreach (CvLineSegmentPolar line in lines)
            {
                double a = Math.Cos(line.Theta);
                double b = Math.Sin(line.Theta);
                double x0 = a * line.Rho;
                double y0 = b * line.Rho;
                Point first = new Point(x0 + (1000 * (-b)), y0 + 1000 * a);
                Point second = new Point(x0 - (1000 * (-b)), y0 - 1000 * a);
                //Cv2.Line(color_img, first, second, Scalar.Red, 1);

               // if ((line.Theta >= 0) && (line.Theta < 0.5))
                    Cv2.Line(color_img, first, second, Scalar.Green, 5);

                //if ((line.Theta > Cv.PI / 180 * 89) && (line.Theta < Cv.PI / 180 * 91))
                    Cv2.Line(color_img, first, second, Scalar.Green, 5);
            }

            return color_img;
        }

        static void cameraWork()
        {
            Mat img = new Mat();
            VideoCapture cap = new VideoCapture(0);
            Window videoWindow = new Window("video", WindowMode.FreeRatio);
            Window videoThreshWindow = new Window("threshvideo", WindowMode.FreeRatio);
            Mat videoImg = new Mat();
            while (true)
            {
                cap.Read(img);
               // videoImg = matematika_apskritimui(img, out Mat edges);
                videoImg = matematika_apskritimui(img, out Mat edges);
                videoWindow.Image = videoImg;
                videoThreshWindow.Image = edges;

                if (Cv2.WaitKey(10) == 'q') { break; }
            }

            cap.Release();
            videoWindow.Close();
            videoThreshWindow.Close();

            Cv2.WaitKey();
        }
    }
}
