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
            Mat boximg = new Mat();

            Rect box = new Rect(50, 50, 250, 300);

            bool histogramSave = false;

            float[] histTemplate = new float[256];

            float error = 9999999;

            Mat histarea = new Mat(new Size(256, 110), MatType.CV_8UC3, Scalar.White); //padauginam matrica is 255 nes tada matysim kaip atrod

            Window videoCap = new Window("Video", WindowMode.FreeRatio);
            Window histoWind = new Window("histo", WindowMode.FreeRatio);
            VideoCapture cap = new VideoCapture(0);
            

            while (true)
            {
                    histarea = new Mat(new Size(256, 110), MatType.CV_8UC3, Scalar.White);
                    cap.Read(img);

                    img[box].CopyTo(boximg);

                    using (var bgrHistogram = MakeBgrHistogram(histarea, boximg, out float[] histoMeasure))
                    {

                        if (Cv2.WaitKey(1) == 's')
                        {
                            histTemplate = histoMeasure;
                            //histoWind.Image = histarea;
                            Console.WriteLine("saugo");
                            histogramSave = true;
                        }

                    Cv2.Rectangle(img, box, Scalar.Red, 1);

                    

                    if (histogramSave)
                    {
                        error = GetDistance(histoMeasure, histTemplate);
                        getStatistics(histoMeasure, out float mean, out float variance, out float skness, out float kurtosis, out float energy, out float entropy);

                        if (error < 100) { Cv2.Rectangle(img, box, Scalar.Green, 1); }
                        else { Cv2.Rectangle(img, box, Scalar.Red, 1); }

                        Cv2.PutText(img, error.ToString(), new Point(50, 50), FontFace.HersheyPlain, 1, Scalar.Red);
                        Cv2.PutText(img, mean.ToString(), new Point(300, 50), FontFace.HersheyPlain, 1, Scalar.Red);
                        Cv2.PutText(img, variance.ToString(), new Point(300, 100), FontFace.HersheyPlain, 1, Scalar.Red);
                        Cv2.PutText(img, skness.ToString(), new Point(300, 150), FontFace.HersheyPlain, 1, Scalar.Red);
                        if(kurtosis < 999999999)
                            Cv2.PutText(img, kurtosis.ToString(), new Point(300, 200), FontFace.HersheyPlain, 1, Scalar.Red);
                        else
                            Cv2.PutText(img, "Unmappable", new Point(300, 200), FontFace.HersheyPlain, 1, Scalar.Red);
                        Cv2.PutText(img, energy.ToString(), new Point(300, 250), FontFace.HersheyPlain, 1, Scalar.Red);
                        Cv2.PutText(img, entropy.ToString(), new Point(300, 300), FontFace.HersheyPlain, 1, Scalar.Red);
                    }                   

                    videoCap.Image = img;                       

                    if (Cv2.WaitKey(10) == 'q') { break; }
                    }

                GC.Collect(); //Cleaning previous objects to protect from memory leak


            }
            cap.Release();
            //videoCap.Close();
            //histoWind.Close();

        }
        //vec1.size == vec2.size
        private static float GetDistance(float[] vec1, float[] vec2)
        {
            float error;
            float sum = 0;

            if (vec1.Length == vec2.Length)
            {
                for (int i =0; i < vec1.Length; i++)
                {
                    sum += ((vec1[i] - vec2[i]) * ((vec1[i] - vec2[i])));
                }
                error = (float)Math.Sqrt(sum);
            }
            else
            {
                Console.WriteLine("wtf");
                return -1;
            }

            return error;
        }

        private static float[] HistogramCalculations(Mat img)
        {
            MatOfByte mat = new MatOfByte(img);
            var idx = mat.GetIndexer();
            float[] histvalue = new float[256];

            for (int row = 0; row < img.Rows; row++)
            {
                for (int col = 0; col < img.Cols; col++)
                {
                    histvalue[idx[row, col]]++;
                }
            }
            return histvalue;
        }

        private static Mat MakeBgrHistogram(Mat outputImg, Mat inputImg, out float[] histoMeasure)
        {
            Mat[] bgr = Cv2.Split(inputImg);


            float[] histvaluer = HistogramCalculations(bgr[2]);
            float[] histvalueg = HistogramCalculations(bgr[1]);
            float[] histvalueb = HistogramCalculations(bgr[0]);

            float maxValueR = histvaluer.Max();
            float maxValueG = histvalueg.Max();
            float maxValueB = histvalueb.Max();

            for (int i = 0; i < 256; i++)
            {
                //Normalizavimas
                histvaluer[i] = histvaluer[i] / maxValueR * 100;
                histvalueg[i] = histvalueg[i] / maxValueG * 100;
                histvalueb[i] = histvalueb[i] / maxValueB * 100;
            }

            for (int i = 0; i < 255; i++)
            {
                Cv2.Line(outputImg, new Point(i, 100 - (int)histvaluer[i]), new Point(i + 1, 100 - (int)histvaluer[i + 1]), Scalar.Red, 1);
                Cv2.Line(outputImg, new Point(i, 100 - (int)histvalueg[i]), new Point(i + 1, 100 - (int)histvalueg[i + 1]), Scalar.Green, 1);
                Cv2.Line(outputImg, new Point(i, 100 - (int)histvalueb[i]), new Point(i + 1, 100 - (int)histvalueb[i + 1]), Scalar.Blue, 1);
            }

            histoMeasure = histvalueb;

            return outputImg;
        }

        private static void getStatistics(float[] rawHist, out float mean, out float variance, out float skness, out float kurtosis, out float energy, out float entropy)
        {
            float NM = 0;
            for (int i= 0; i<rawHist.Length; i++)
            {
                NM += rawHist[i];
            }

            float[] p_i = rawHist;
            for(int i=0; i<rawHist.Length; i++)
            {
                p_i[i] = p_i[i] / NM;
            }

            mean = 0;
            for(int i=0; i<rawHist.Length; i++) { mean += i * p_i[i]; }

            variance = 0;
            for (int i = 0; i < rawHist.Length; i++) { variance = (float)Math.Pow(Math.Sqrt(i-mean), 2) * p_i[i]; }

            skness = 0;
            for (int i = 0; i < rawHist.Length; i++) { skness = (float)Math.Pow(Math.Sqrt(i - mean), 3) * p_i[i]; }
            skness = (float) Math.Pow(Math.Sqrt(variance), -3) * skness;

            kurtosis = 0;
            for (int i = 0; i < rawHist.Length; i++)
            {
                kurtosis += (float)Math.Pow(i - mean, 4) * p_i[i] - 3;
            }

            kurtosis = (float)(kurtosis * Math.Pow(Math.Sqrt(variance), -4));

            energy = 0;
            for (int i = 0; i < rawHist.Length; i++)
            {
                energy += (float)Math.Pow(p_i[i], 2);
            }

            energy = 0;
            for (int i = 0; i < rawHist.Length; i++) { energy += (float)Math.Pow(p_i[i], 2); }

            entropy = 0;
            for(int i=0; i < rawHist.Length; i++) { entropy += p_i[i] * (float)Math.Log(p_i[i], 2); }
        }
    }
}
