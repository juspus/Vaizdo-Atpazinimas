using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OroSalyguAtpazinimas
{
    public class Skies
    {
        //public Mat Image { get; set; }
        public float Mean { get; set; }
        public float Variance { get; set; }
        public string FileName { get; set; }
        //public double Moments { get; set; }
        public float Energy { get; set; }
        public float Entropy { get; set; }
        public float Smoothness { get; set; }
        public string Name { get; set; }
        public double Matching { get; set; }

        //public double[] Normalized { get; set; }

        public Skies( float mean, float variance, string fileName, float energy, float entropy, float smoothness, string name, double matching)
        {

           // Image = image;
            Mean = mean;
            Variance = variance;
            FileName = fileName;
           // Moments = moments;
            Energy = energy;
            Entropy = entropy;
            Smoothness = smoothness;
            Name = name;
            Matching = matching;
        }
    }
}
