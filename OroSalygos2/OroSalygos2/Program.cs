using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OroSalygos2
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(@"C:\Users\Laptop\Desktop\asd2");
            Window show = new Window("",WindowMode.FreeRatio);
            Window show2 = new Window("asds", WindowMode.FreeRatio);
            Mat[] splitMat = new Mat[3];
            Mat image = new Mat();
            Mat imageTrue = new Mat();
            foreach (var file in files)
            {
                image = Cv2.ImRead(file);
                image.CopyTo(imageTrue);
                Cv2.CvtColor(image, image, ColorConversion.RgbToHsv);
                Cv2.Split(image, out splitMat);
                //Cv2.CvtColor(splitMat[1], image, ColorConversion.RgbToGray);

                Cv2.BitwiseNot(splitMat[1], image);
                Cv2.Threshold(image, image, 210, 255, ThresholdType.Binary);
                

                int total = image.Cols * image.Rows;
                int black = Cv2.CountNonZero(image);
                float debesuotumas =  (float)black / (float)total;
                string oroSalygos = "";
                if (debesuotumas != 0 && debesuotumas < 0.01)
                    oroSalygos = "Giedra su mazais debesimis";
                else if (0.01 < debesuotumas && debesuotumas < 0.45)
                    oroSalygos = "Lengvas Debesuotumas";
                else if (0.45 < debesuotumas && debesuotumas < 0.8)
                    oroSalygos = "Debesuota";
                else if (0.8 < debesuotumas)
                    oroSalygos = "Labai Debesuota";
                else
                    oroSalygos = "Giedra";

                    Console.WriteLine(debesuotumas);
                Point vieta = new Point(100, 100);
                Cv2.PutText(imageTrue, oroSalygos, vieta, FontFace.Italic, 3, Scalar.Red, 5);
                show.Image = image;
                show2.Image = imageTrue;
                Cv2.WaitKey();

            }
        }
    }
}
