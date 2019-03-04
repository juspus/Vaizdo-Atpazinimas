using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace TreciasLaboratorinis
{
    class Program
    {
        static void Main(string[] args)
        {
            Mat img = Cv2.ImRead(@"C:\Users\Laptop\Desktop\1482848234_gaidys-3-x.jpg", LoadMode.Color);
            Window show = new Window("originalas", WindowMode.FreeRatio);
            float[] histogram = HistogramCalculations(img);


        }

        private static float getDistance(float[] vec1, float[] vec2)
        {
            float error = 9999999999999;
            float sum = 0;

            if (vec1.Length == vec2.Length)
            {
                for (int i = 0; i < vec1.Length; i++)
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
    }
}
