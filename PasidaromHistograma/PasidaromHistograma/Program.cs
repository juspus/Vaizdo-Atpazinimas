using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace AntraPaskaitaHistograma
{
    class Program
    {
        static void Main(string[] args)
        {
            Mat img = new Mat();
            Mat video = new Mat();
            Mat gray = new Mat();
            Mat blue = new Mat();
            Mat green = new Mat();
            Mat red = new Mat();

            VideoCapture cap = new VideoCapture(0);

            img = Cv2.ImRead(@"C:\Users\Laptop\Desktop\aa.jpg", LoadMode.Color);
            gray = img.CvtColor(ColorConversion.RgbToGray);

            Mat[] bgr = Cv2.Split(img);
            bgr[0].CopyTo(blue);
            bgr[1].CopyTo(green);
            bgr[2].CopyTo(red);

            float[] histvaluer = new float[256];
            float[] histvalueg = new float[256];
            float[] histvalueb = new float[256];
            float[] histvalue = new float[256];

            MatOfByte matr = new MatOfByte(red);
            var idxr = matr.GetIndexer();
            MatOfByte matg = new MatOfByte(green);
            var idxg = matg.GetIndexer();
            MatOfByte matb = new MatOfByte(blue);
            var idxb = matb.GetIndexer();
            for (int row = 0; row < gray.Rows; row++)
            {
                for (int col = 0; col < gray.Cols;  col++)
                {
                    histvaluer[idxr[row, col]]++;
                    histvalueg[idxg[row, col]]++;
                    histvalueb[idxb[row, col]]++;
                }
            }

            

            float maxValueR = histvaluer.Max();
            float maxValueG = histvalueg.Max();
            float maxValueB = histvalueb.Max();
            Mat histarea = new Mat(new Size(256, 110), MatType.CV_8UC3, Scalar.White); //padauginam matrica is 255 nes tada matysim kaip atrod

            for (int i = 0; i<256; i++)
            {
                Console.WriteLine(histvalue[i].ToString());

                //Normalizavimas
                histvaluer[i] = histvaluer[i] / maxValueR * 100;
                histvalueg[i] = histvalueg[i] / maxValueG * 100;
                histvalueb[i] = histvalueb[i] / maxValueB * 100;
            }

            /*while (true)
            {
                cap.Read(img);
                //show.Image = img;
                bgr = Cv2.Split(img);
                bgr[0].CopyTo(blue);
                bgr[1].CopyTo(green);
                bgr[2].CopyTo(red);

                matr = new MatOfByte(red);
                idxr = matr.GetIndexer();
                matg = new MatOfByte(green);
                idxg = matg.GetIndexer();
                matb = new MatOfByte(blue);
                idxb = matb.GetIndexer();

                for (int row = 0; row < gray.Rows; row++)
                {
                    for (int col = 0; col < gray.Cols; col++)
                    {
                        histvaluer[idxr[row, col]]++;
                        histvalueg[idxg[row, col]]++;
                        histvalueb[idxb[row, col]]++;
                    }
                }

                maxValueR = histvaluer.Max();
                maxValueG = histvalueg.Max();
                maxValueB = histvalueb.Max();
            }*/

            for (int i=0; i<255; i++)
            {
                Cv2.Line(histarea, new Point(i, 100 - (int)histvaluer[i]), new Point(i + 1, 100 - (int)histvaluer[i + 1]), Scalar.Red, 1);
                Cv2.Line(histarea, new Point(i, 100 - (int)histvalueg[i]), new Point(i + 1, 100 - (int)histvalueg[i + 1]), Scalar.Green, 1);
                Cv2.Line(histarea, new Point(i, 100 - (int)histvalueb[i]), new Point(i + 1, 100 - (int)histvalueb[i + 1]), Scalar.Blue, 1);
            }

            Window nicole = new Window("nicole", WindowMode.FreeRatio);
            nicole.Image = img;

            Window histoWind = new Window("histo", WindowMode.FreeRatio);
            histoWind.Image = histarea;

            Cv2.WaitKey();

            nicole.Close();
        }
    }
}
