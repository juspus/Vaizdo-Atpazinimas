using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace ZiuretiViduje // Haar'o funkcija
{
    class Program
    {
        static void MakePositiveFile()
        {
            Mat img = new Mat();
            string[] files =
                Directory.GetFiles(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\Positive");// "*.jpg");

            StreamWriter xmlfile = new StreamWriter(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\dat.vec", true); 

            foreach (string file in files)
            {
                img = Cv2.ImRead(file);
                int w = img.Width;
                int h = img.Height;
                Console.WriteLine(file);
                xmlfile.WriteLine(file + " 1 0 0 " + w.ToString() + " " + h.ToString());
                xmlfile.Flush();
            }
            xmlfile.Close();
            Console.ReadKey();
        }

        static void MakeNegativesFile()
        {
            Mat img = new Mat();
            string[] files =
                Directory.GetFiles(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\Negative");// "*.jpg");

            StreamWriter xmlfile = new StreamWriter(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\neg.txt", true);

            foreach (string file in files)
            {
                Console.WriteLine(file);
                xmlfile.WriteLine(file);
                xmlfile.Flush();
            }
            xmlfile.Close();
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            int mode = 2;

            switch (mode)
            {
                case 0:
                    Mat frame = new Mat();
                    Mat gray = new Mat();
                    VideoCapture cap = new VideoCapture(0);

                    Window show = new Window("frame", WindowMode.FreeRatio);

                    CascadeClassifier face = new CascadeClassifier(
                        @"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\ZiuretiViduje\haarcascade_frontalface_default.xml");
                    CascadeClassifier smile = new CascadeClassifier(
                        @"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\ZiuretiViduje\haarcascade_smile.xml");
                    CascadeClassifier eye = new CascadeClassifier(
                        @"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\ZiuretiViduje\haarcascade_eye.xml");

                    while (true)
                    {
                        cap.Read(frame);

                        if (frame.Cols > 0)
                        {
                            Cv2.CvtColor(frame, gray, ColorConversion.RgbToGray);
                            var facerect = face.DetectMultiScale(gray, 1.1, 3, HaarDetectionType.Zero, new Size(30, 30),
                                new Size(200, 200));

                            foreach (Rect box in facerect)
                            {
                                Cv2.Rectangle(frame, box, Scalar.Red, 2);
                                var smilerect = smile.DetectMultiScale(gray[box], 1.1, 3, HaarDetectionType.Zero,
                                    new Size(20, 20),
                                    new Size(200, 200));
                                var eyerect = eye.DetectMultiScale(gray[box], 1.1, 3, HaarDetectionType.Zero,
                                    new Size(10, 10),
                                    new Size(200, 200));

                                foreach (Rect smileBox in smilerect)
                                {
                                    Cv2.Rectangle(frame,
                                        new Rect(box.X + smileBox.X, box.Y + smileBox.Y, smileBox.Width,
                                            smileBox.Height), Scalar.Green, 2);
                                }

                                foreach (Rect eyeBox in eyerect)
                                {
                                    Cv2.Rectangle(frame,
                                        new Rect(box.X + eyeBox.X, box.Y + eyeBox.Y, eyeBox.Width, eyeBox.Height),
                                        Scalar.Blue, 2);
                                }
                            }

                            show.Image = frame;
                        }

                        if (Cv2.WaitKey(10) == 'q')
                        {
                            break;
                        }
                    }

                    cap.Release();
                    break;
                case 1:
                    MakePositiveFile();
                    MakeNegativesFile();
                    break;
                case 2:
                    Mat zenklaiRGB = Cv2.ImRead(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\ZiuretiViduje\ZiuretiViduje\kontroller-rvp340-siemens_i12965.jpg", LoadMode.Color);
                    Mat zenklaiGray = new Mat();
                    Cv2.CvtColor(zenklaiRGB, zenklaiGray, ColorConversion.RgbToGray);
                    CascadeClassifier stop = new CascadeClassifier(@"C:\Users\Laptop\Downloads\opencv\build\x64\vc14\bin\cascade\cascade.xml");
                    var stopBoxes = stop.DetectMultiScale(zenklaiGray, 1.1, 3, HaarDetectionType.Zero, new Size(30, 30),
                        new Size(200, 200));

                    foreach (Rect box in stopBoxes)
                    {
                        Cv2.Rectangle(zenklaiRGB, box, Scalar.Blue, 2);
                    }
                    Window sss = new Window("sss", WindowMode.FreeRatio);
                    sss.Image = zenklaiRGB;

                    Cv2.WaitKey();
                    break;
                default:
                    break;
            }
        }
    }
}
