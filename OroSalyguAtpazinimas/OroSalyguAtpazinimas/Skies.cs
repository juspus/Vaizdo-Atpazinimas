using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OroSalyguAtpazinimas
{
    [Serializable]
    public class Skies
    {
        public float Mean { get; set; }
        public float Variance { get; set; }
        public string FileName { get; set; }
        public float Energy { get; set; }
        public float Entropy { get; set; }
        public float Smoothness { get; set; }
        public string Name { get; set; }

        
        public Skies ()
        {

        }

        public void getSkies( float mean, float variance, string fileName, float energy, float entropy, float smoothness, string name)
        {
            Mean = mean;
            Variance = variance;
            FileName = fileName;
            Energy = energy;
            Entropy = entropy;
            Smoothness = smoothness;
            Name = name;
        }
        
    }

}
