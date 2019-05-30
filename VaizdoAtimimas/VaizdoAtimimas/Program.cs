using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace VaizdoAtimimas
{
    class Program
    {
        static void hu_momentai() { }
        static void Main(string[] args)
        {
            Mat background = new Mat();
            Mat foreground = new Mat();

            VideoCapture cap = new VideoCapture(0);
            Window show = new Window("rezultatas", WindowMode.FreeRatio);

            while (true)
            {
                cap.Read(background);
                if (background.Cols > 0)
                {
                    //Cv2.CvtColor(background, background, ColorConversion.RgbToGray);
                    show.Image = background;
                }

                if (Cv2.WaitKey(10) == 'q')
                {
                    break;
                }
            }

            Mat subs = new Mat();

            int skaitliukas = 0;
            while (true)
            {
                cap.Read(foreground);
                if (foreground.Cols > 0)
                {
                    //Cv2.CvtColor(foreground, foreground, ColorConversion.RgbToGray);
                    if (skaitliukas > 40)
                    {
                        skaitliukas = 0;
                        foreground.CopyTo(background);
                    }
                    else
                    {
                        skaitliukas++;
                    }

                    Cv2.Subtract(background, foreground, subs);
                    Cv2.Threshold(subs, subs, 40, 225, ThresholdType.Binary);
                    show.Image = subs;
                }
                if (Cv2.WaitKey(10) == 'q')
                {
                    break;
                }
            }
            cap.Release();
        }
    }
}
