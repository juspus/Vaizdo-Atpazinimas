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

            Mat[] bgr;

            MatOfByte matr;
            MatOfByte matg;
            MatOfByte matb;

            float[] histvaluer = new float[256];
            float[] histvalueg = new float[256];
            float[] histvalueb = new float[256];

            float maxValueR;
            float maxValueG;
            float maxValueB;

            Mat histarea = new Mat(new Size(256, 110), MatType.CV_8UC3, Scalar.White); //padauginam matrica is 255 nes tada matysim kaip atrod

            Window nicole = new Window("nicole", WindowMode.FreeRatio);
            Window histoWind = new Window("histo", WindowMode.FreeRatio);
            VideoCapture cap = new VideoCapture(0);

            while (true)
            {
                histarea = new Mat(new Size(256, 110), MatType.CV_8UC3, Scalar.White);

                cap.Read(img);
                nicole.Image = img;
                bgr = Cv2.Split(img);

                matr = new MatOfByte(bgr[2]);
                var idxr = matr.GetIndexer();
                matg = new MatOfByte(bgr[1]);
                var idxg = matg.GetIndexer();
                matb = new MatOfByte(bgr[0]);
                var idxb = matb.GetIndexer();

                for (int row = 0; row < img.Rows; row++)
                {
                    for (int col = 0; col < img.Cols; col++)
                    {
                        histvaluer[idxr[row, col]]++;
                        histvalueg[idxg[row, col]]++;
                        histvalueb[idxb[row, col]]++;
                    }
                }

                maxValueR = histvaluer.Max();
                maxValueG = histvalueg.Max();
                maxValueB = histvalueb.Max();

                for (int i = 0; i < 256; i++)
                {
                //Normalizavimas
                    histvaluer[i] = histvaluer[i] / maxValueR * 100;
                    histvalueg[i] = histvalueg[i] / maxValueG * 100;
                    histvalueb[i] = histvalueb[i] / maxValueB * 100;
                }

                for (int i = 0; i < 255; i++)
                {
                    Cv2.Line(histarea, new Point(i, 100 - (int)histvaluer[i]), new Point(i + 1, 100 - (int)histvaluer[i + 1]), Scalar.Red, 1);
                    Cv2.Line(histarea, new Point(i, 100 - (int)histvalueg[i]), new Point(i + 1, 100 - (int)histvalueg[i + 1]), Scalar.Green, 1);
                    Cv2.Line(histarea, new Point(i, 100 - (int)histvalueb[i]), new Point(i + 1, 100 - (int)histvalueb[i + 1]), Scalar.Blue, 1);
                }

                histoWind.Image = histarea;

                GC.Collect(); //Cleaning previous objects to protect from memory leak

                if (Cv2.WaitKey(10) == 'q') { break; }
        }

            cap.Release();
            Cv2.WaitKey();

            nicole.Close();
        }
    }
}
