using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace vaizduojam
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("po pitaka");

            Mat img = Cv2.ImRead(@"C:\Users\Laptop\Desktop\1482848234_gaidys-3-x.jpg", LoadMode.Color);

            Mat atskiras_img = new Mat();
            img.CopyTo(atskiras_img);

            Mat[] bgr = Cv2.Split(atskiras_img);

            Mat tmp = new Mat();
            bgr[0].CopyTo(tmp);
            bgr[1].CopyTo(bgr[0]);
            tmp.CopyTo(bgr[1]);

            Cv2.Merge(bgr, atskiras_img);

            Window show = new Window("originalas", WindowMode.FreeRatio);
            show.Image = img;

            Window show2 = new Window("perdirbtas", WindowMode.FreeRatio);
            show2.Image = atskiras_img;

            VideoCapture cap = new VideoCapture(0);

            while(true)
            {
                cap.Read(img);
                show.Image = atskiras_img;
                img.CopyTo(atskiras_img);
                bgr = Cv2.Split(atskiras_img);

                Mat tmp1 = new Mat();
                bgr[0].CopyTo(tmp1);
                bgr[2].CopyTo(bgr[0]);
                tmp1.CopyTo(bgr[2]);

                Cv2.Merge(bgr, atskiras_img);

                if (Cv2.WaitKey(10) == 'q') { break; }
            }

            cap.Release();
            show.Close();

            Cv2.WaitKey();
        }
    }
}
