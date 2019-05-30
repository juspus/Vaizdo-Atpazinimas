using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OroSalyguAtpazinimas
{
    class TempSky
    {
        public Mat Image { get; set; }
        public float Mean { get; set; }
        public float Variance { get; set; }
        public string FileName { get; set; }
        //public double Moments { get; set; }
        public float Energy { get; set; }
        public float Entropy { get; set; }
        public float Smoothness { get; set; }

        public TempSky(Mat image, float mean, float variance, string fileName, float energy, float entropy, float smoothness)
        {

            Image = image;
            Mean = mean;
            Variance = variance;
            FileName = fileName;
           // Moments = moments;
            Energy = energy;
            Entropy = entropy;
            Smoothness = smoothness;
        }

        

    }
}
