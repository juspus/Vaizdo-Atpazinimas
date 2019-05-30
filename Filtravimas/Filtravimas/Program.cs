using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
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

        static void CameraWork(VideoCapture cap, Mat kernel, Mat thresholded)
        {
            while (true)
            {
                Mat img = new Mat();
                cap.Read(img);
                if (img.Cols > 0)
                {
                    
                    Mat filtered = new Mat();
                    Cv2.Filter2D(img, filtered, img.Depth(), kernel);
                    Cv2.Threshold(filtered, thresholded, 120, 255, ThresholdType.Binary);
                }
            }
        }



        static void Main(string[] args)
        {
            Mat orig_img = Cv2.ImRead(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\Filtravimas\spatialnoise_ship.png", LoadMode.GrayScale);
            Mat img2 = Cv2.ImRead(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\Filtravimas\download.jpg",
                LoadMode.GrayScale);
            Mat img3 = Cv2.ImRead(@"C:\Users\Laptop\Documents\Projektai\Vaizdo-Atpazinimas\Filtravimas\poses-selfie-inst.jpg", LoadMode.Color);
            Mat gray_img3 = new Mat();
            Mat filtered = new Mat();
            Mat thresholded = new Mat();
            VideoCapture cap = new VideoCapture(0);
            Mat video = new Mat();
            Window show = new Window("originalas", WindowMode.FreeRatio);
            Window kernel_wind = new Window("Kernel", WindowMode.FreeRatio);
            Window median_wind = new Window("Median", WindowMode.FreeRatio);
            Window sharp_wind = new Window("Sharp", WindowMode.FreeRatio);
            Window sharp_wind2 = new Window("Sharping2", WindowMode.FreeRatio);
            Window threshold = new Window("Threshold", WindowMode.FreeRatio);

            //Cv2.CvtColor(img3, gray_img3, ColorConversion.RgbToGray);

            Mat kernel = Mat.Ones(new Size(3, 3), MatType.CV_8U);


            // Mat kernel_second = Mat.Ones(new Size(3, 3), MatType.CV_8U);
            int[,] kernelArray2 = { { 0, -1, 0 }, { -1, 4, -1 }, {0, -1 ,0} };
            Mat kernel_second = Mat.Ones(new Size(3, 3), MatType.CV_32F);

            kernel_second.Set<Vec2f>(0, 0, new Vec2f(0, 0));
            kernel_second.Set<Vec2f>(0, 1, new Vec2f(-2, -2));
            kernel_second.Set<Vec2f>(0, 2, new Vec2f(0, 0));

            kernel_second.Set<Vec2f>(1, 0, new Vec2f(-2, -2));
            kernel_second.Set<Vec2f>(1, 1, new Vec2f(8, 8));
            kernel_second.Set<Vec2f>(1, 2, new Vec2f(-2, -2));

            kernel_second.Set<Vec2f>(2, 0, new Vec2f(0, 0));
            kernel_second.Set<Vec2f>(2, 1, new Vec2f(-2, -2));
            kernel_second.Set<Vec2f>(2, 2, new Vec2f(0, 0));

            
            Mat filtered_img = doKernel(orig_img, kernel);
            Mat median_img = doMedianFilter(orig_img, kernel);
            Mat filteredImg2 = doKernel(median_img, kernel_second);
            Cv2.Filter2D(img3, filtered, img3.Depth(), kernel_second);
            Cv2.Threshold(filtered, thresholded, 120, 255, ThresholdType.Binary);
           
            Mat shape1 = Cv2.GetStructuringElement(StructuringElementShape.Cross, new Size(5, 5));
            Mat shape2 = Cv2.GetStructuringElement(StructuringElementShape.Rect, new Size(5, 5));
            Mat shape3 = Cv2.GetStructuringElement(StructuringElementShape.Ellipse, new Size(5, 5));
            while (true)
            {
                cap.Read(video);
                if (video.Cols > 0)
                {
                    Mat[] channels = Cv2.Split(video);


                    Cv2.Canny(channels[0], channels[0], 60, 60);
                    Cv2.Canny(channels[1], channels[1], 60, 60);
                    Cv2.Canny(channels[2], channels[2], 60, 60);

                    Cv2.Dilate(channels[0], channels[0], shape1, null, 1);
                    Cv2.Dilate(channels[1], channels[1], shape2, null, 1);
                    Cv2.Dilate(channels[2], channels[1], shape3, null, 1);

                    //Cv2.Canny(orig_img, filtered, 60, 60);
                    //Cv2.Dilate(filtered, filtered, shape, null, 2);

                    Cv2.Merge(channels, video);
                    show.Image = video;
                    GC.Collect();
                }

                if (Cv2.WaitKey(10) == 'q')
                {
                    break;
                }
            }
            cap.Release();

            var testVector = filteredImg2.ToCvMat();

          //  Console.WriteLine(testVector);

            
            

            show.Image = orig_img;
            median_wind.Image = median_img;
            sharp_wind.Image = filteredImg2;
            sharp_wind2.Image = filtered;
            threshold.Image = thresholded;

            Cv2.WaitKey(0);
            show.Close();
            kernel_wind.Close();
            threshold.Close();
            sharp_wind2.Close();
            median_wind.Close();
            sharp_wind.Close();
            filteredImg2.Release();
            filtered.Release();
            median_img.Release();
            orig_img.Release();
        }
    }
}
