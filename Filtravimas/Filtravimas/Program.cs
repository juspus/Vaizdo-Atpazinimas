using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace Filtravimas
{
    class Program
    {
        static Mat doKernel(Mat img, Mat kernel)
        {
            Mat outputImg = new Mat();

            int kernel_size = kernel.Cols;
            int pad = (kernel_size - 1)/2;

            img.CopyTo(outputImg);

            MatOfByte mat = new MatOfByte(outputImg);
            var idx = mat.GetIndexer();

            MatOfByte kernel_mat = new MatOfByte(kernel);
            var kernel_idx = kernel_mat.GetIndexer();
            
            Console.WriteLine(kernel_mat.ToCvMat());

            for (int k_row = 0; k_row < kernel_size; k_row++) // k - kernel
            {
                for (int k_col = 0; k_col < kernel_size; k_col++)
                {
                    Console.WriteLine(kernel_idx[k_row, k_col]);
                }
            }

            float sum = 0;

            for (int c_row = pad; c_row < mat.Rows; c_row++) // c- center
            {
                for (int c_col = pad; c_col < mat.Cols - pad; c_col++)
                {
                    sum = 0;

                    for (int k_row = 0; k_row < kernel_size; k_row++) // k - kernel
                    {
                        for (int k_col = 0; k_col < kernel_size; k_col++)
                        {
                            sum += idx[c_row - pad + k_row, c_col - pad + k_col] * kernel_idx[k_row, k_col] *
                                1/(kernel_size* kernel_size);
                            var testShit = idx[c_row - pad + k_row, c_col - pad + k_col].ToString();
                            var testKernel = kernel_idx[k_row, k_col];
                           // Console.WriteLine(testShit + " " + testKernel);
                        }
                    }
                    idx[c_row, c_col] = (byte) sum;
                }
            }

            return outputImg;
        }

        static Mat doMedianFilter(Mat img, Mat kernel)
        {
            Mat outputImg = new Mat();

            int kernel_size = kernel.Cols;
            int pad = (kernel_size - 1) / 2;

            img.CopyTo(outputImg);

            MatOfByte mat = new MatOfByte(outputImg);
            var idx = mat.GetIndexer();

            int k = 0;
            decimal[] vector = new decimal[kernel_size*kernel_size];

            for (int c_row = pad; c_row < mat.Rows - pad; c_row++) // c- center
            {
                for (int c_col = pad; c_col < mat.Cols - pad; c_col++)
                {
                    k = 0;
                    for (int k_row = 0; k_row < kernel_size; k_row++) // k - kernel
                    {
                        for (int k_col = 0; k_col < kernel_size; k_col++)
                        {
                            vector[k] = idx[c_row - pad + k_row, c_col - pad + k_col];
                            k++;
                        }
                    }

                    idx[c_row, c_col] = (byte)Median(vector);
                }
            }

            return outputImg;
        }

        static decimal Median(decimal[] xs)
        {
            Array.Sort(xs);
            return xs[xs.Length / 2];
        }

        static void Main(string[] args)
        {
            Mat orig_img = Cv2.ImRead(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\Filtravimas\spatialnoise_ship.png", LoadMode.GrayScale);
            Mat img2 = Cv2.ImRead(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\Filtravimas\download.jpg",
                LoadMode.GrayScale);

            Mat kernel = Mat.Ones(new Size(3, 3), MatType.CV_8U);
            // Mat kernel_second = Mat.Ones(new Size(3, 3), MatType.CV_8U);
            int[,] kernelArray2 = { { 0, -1, 0 }, { -1, 4, -1 }, {0, -1 ,0} };
            Mat kernel_second = Mat.Ones(new Size(3, 3), MatType.CV_32F);

            kernel_second.Set<float>(0, 0, 0);
            kernel_second.Set<float>(0, 1, -1);
            kernel_second.Set<float>(0, 2, 0);

            kernel_second.Set<float>(1, 0, -1);
            kernel_second.Set<float>(1, 1, 4);
            kernel_second.Set<float>(1, 2, -1);

            kernel_second.Set<float>(2, 0, 0);
            kernel_second.Set<float>(2, 1, -1);
            kernel_second.Set<float>(2, 2, 0);

            
            //Mat filtered_img = doKernel(orig_img, kernel);
            Mat median_img = doMedianFilter(orig_img, kernel);
            Mat filteredImg2 = doKernel(median_img, kernel_second);
            var testVector = filteredImg2.ToCvMat();

          //  Console.WriteLine(testVector);

            Window show = new Window("originalas", WindowMode.FreeRatio);
           // Window kernel_wind = new Window("Kernel", WindowMode.FreeRatio);
            Window median_wind = new Window("Median", WindowMode.FreeRatio);
            Window sharp_wind = new Window("Sharp", WindowMode.FreeRatio);

            show.Image = orig_img;
          //  kernel_wind.Image = filtered_img;
            median_wind.Image = median_img;
            sharp_wind.Image = filteredImg2;

            Cv2.WaitKey(0);
            show.Close();
          //  kernel_wind.Close();
            median_wind.Close();
            sharp_wind.Close();
            filteredImg2.Release();
            median_img.Release();
            orig_img.Release();
           // filtered_img.Release();
        }
    }
}
