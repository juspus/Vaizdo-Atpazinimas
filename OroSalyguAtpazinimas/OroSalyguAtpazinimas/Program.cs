using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using static OpenCvSharp.CPlusPlus.Cv2;


namespace OroSalyguAtpazinimas
{
    class Program
    {
        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                }
            }
            catch (Exception ex)
            {
                //Log exception here
                Console.WriteLine(ex);
            }
        }

        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log exception here
                Console.WriteLine(ex);
            }

            return objectOut;
        }

        private static void getStats(Mat img, float mean, float variance, out float smoothness, out float energy, out float entropy)
        {
            float[] rawHist = HistogramCalculations(img);
            float NM = rawHist.Sum();
            
            float[] p_i = rawHist;
            for (int i = 0; i < rawHist.Length; i++)
            {
                p_i[i] = p_i[i] / NM;
            }

            smoothness = 0;
            smoothness = 1 - (1 / (1 + variance * variance));
            
            energy = 0;
            for (int i = 0; i < rawHist.Length; i++)
            {
                energy += (float) Math.Pow(p_i[i], 2);
            }

            entropy = 0;
            for (int i = 0; i < rawHist.Length; i++)
            {
                if(p_i[i] != 0)
                entropy += (float) (p_i[i] * Math.Log(p_i[i], 2));
            }

            entropy = (-1) * entropy;
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
        
        static void classMake(out List<Skies> dangus, string directory, Mat tempSky)
        {
            string[] files = Directory.GetFiles(directory);

            dangus = new List<Skies>();
            Mat dangusPhoto = new Mat();
            Mat bw = new Mat();
            Mat[] dangusSplit = new Mat[3];
            Mat dangusHSV = new Mat();
            //Moments moments = new Moments();
            
            string name = "";
            double matching =0;

            foreach (var file in files)
            {
                if (file.Contains("cloudy") && !file.Contains("verycloudy") && !file.Contains("partlycloudy"))
                    name = "Debesuota";
                else if (file.Contains("verycloudy"))
                    name = "Labai Debesuota";
                else if (file.Contains("partlycloudy"))
                    name = "Saulėta su debesimis";
                else if (file.Contains("lightcloud"))
                    name = "Lengvai debesuota";
                else if (file.Contains("clear"))
                    name = "Giedra";

                dangusPhoto = ImRead(file, LoadMode.Color);
                CvtColor(dangusPhoto, bw, ColorConversion.RgbToGray);
                CvtColor(dangusPhoto, dangusHSV, ColorConversion.RgbToHsv);
                Cv2.Split(dangusPhoto, out dangusSplit);
                float mean = 0;
                float variance = 0;
                float smoothness = 0;
                float energy = 0;
                float entropy = 0;

                for (int i = 0; i < 3; i++)
                {
                    MeanStdDev(dangusSplit[i], out Scalar meanScalar, out Scalar varianceScalar);
                    getStats(dangusSplit[i], (float)mean, (float)variance, out smoothness, out energy, out entropy);
                    mean += (float)meanScalar[0];
                    variance += (float)varianceScalar[0];
                    smoothness += smoothness;
                    energy += energy;
                    entropy += entropy;
                }
                
                //matching = machingas(dangusPhoto, tempSky, 1);
                //moments = Moments(bw);
                Skies sky = new Skies();
                sky.getSkies(mean, variance, file, energy, entropy, smoothness, name);
                dangus.Add(sky);
                GC.Collect();
            }
        }

        static void classMakeTemp(out List<TempSky> dangus, string directory)
        {
            string[] files = Directory.GetFiles(directory);

            dangus = new List<TempSky>();
            Mat dangusPhoto = new Mat();
            Mat bw = new Mat();
            Mat[] dangusSplit = new Mat[3];
            Mat dangusHSV = new Mat();
            //Moments moments = new Moments();
            

            foreach (var file in files)
            {
                dangusPhoto = ImRead(file, LoadMode.Color);
                CvtColor(dangusPhoto, bw, ColorConversion.RgbToGray);

                CvtColor(dangusPhoto, dangusHSV, ColorConversion.RgbToHsv);
                Cv2.Split(dangusPhoto, out dangusSplit);
                float mean = 0;
                float variance = 0;
                float smoothness = 0;
                float energy = 0;
                float entropy = 0;

                for (int i = 0; i < 3; i++)
                {
                    MeanStdDev(dangusSplit[i], out Scalar meanScalar, out Scalar varianceScalar);
                    getStats(dangusSplit[i], (float)mean, (float)variance, out smoothness, out energy, out entropy);
                    mean += (float)meanScalar[0];
                    variance += (float)varianceScalar[0];
                    smoothness += smoothness;
                    energy += energy;
                    entropy += entropy;
                }

                //moments = Moments(bw);
                dangus.Add(new TempSky(dangusPhoto, (float)mean, (float)variance, file,  energy, entropy, smoothness));
                GC.Collect();
            }
        }

        static Dictionary<int, double> Surykiavimas(Dictionary<int, double> skyDictionary, List<TempSky> tempDangus, List<Skies> dangus, int test, int atvejis)
        {
            Dictionary<int, double> skyDictionary2 = new Dictionary<int, double>();
            if (skyDictionary.Count > 0)
            {
                for (int i = 0; i < skyDictionary.Count-1; i++)
                {
                    double value = 99999999999;
                    switch (atvejis)
                    {
                        case 0:
                            value = Math.Abs(tempDangus[test].Energy - dangus[skyDictionary.Keys.ElementAt(i)].Energy);
                            break;
                        case 4:
                            value = Math.Abs(tempDangus[test].Mean - dangus[skyDictionary.Keys.ElementAt(i)].Mean);
                            break;
                        case 1:
                            value = Math.Abs(tempDangus[test].Entropy -
                                             dangus[skyDictionary.Keys.ElementAt(i)].Entropy);
                            break;
                        case 2:
                            value = Math.Abs(tempDangus[test].Smoothness -
                                             dangus[skyDictionary.Keys.ElementAt(i)].Smoothness);
                            break;
                        case 3:
                            value = Math.Abs(tempDangus[test].Variance -
                                             dangus[skyDictionary.Keys.ElementAt(i)].Variance);
                            break;
                    }

                    skyDictionary2.Add(skyDictionary.Keys.ElementAt(i), value);
                }
            }
            else if (!(tempDangus.Count > 0))
            {
                Console.WriteLine("Error: No sky provided.");
            }

            else
            {
                for (int i = 0; i < dangus.Count-1; i++)
                {
                    double value = 99999999999;
                    switch (atvejis)
                    {
                        case 0:
                            value = Math.Abs(tempDangus[test].Energy - dangus[i].Energy);
                            break;
                        case 4:
                            value = Math.Abs(tempDangus[test].Mean - dangus[i].Mean);
                            break;
                        case 1:
                            value = Math.Abs(tempDangus[test].Entropy - dangus[i].Entropy);
                            break;
                        case 2:
                            value = Math.Abs(tempDangus[test].Smoothness - dangus[i].Smoothness);
                            break;
                        case 3:
                            value = Math.Abs(tempDangus[test].Variance - dangus[i].Variance);
                            break;
                    }

                    skyDictionary2.Add(i, value);
                }
            }

            skyDictionary2 = skyDictionary2.OrderBy(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
            for (int i = 0; i < skyDictionary2.Count; i++)
            {
                switch (atvejis)
                {
                    case 0:
                        Console.WriteLine("Energy: {0}      {1}", skyDictionary2.Values.ElementAt(i), dangus[skyDictionary2.Keys.ElementAt(i)].FileName);
                        break;
                    case 1:
                        Console.WriteLine("Entropy: {0}      {1}", skyDictionary2.Values.ElementAt(i), dangus[skyDictionary2.Keys.ElementAt(i)].FileName);
                        break;
                    case 2:
                        Console.WriteLine("Smoothness: {0}      {1}", skyDictionary2.Values.ElementAt(i), dangus[skyDictionary2.Keys.ElementAt(i)].FileName);
                        break;
                    case 3:
                        Console.WriteLine("Variance: {0}      {1}", skyDictionary2.Values.ElementAt(i), dangus[skyDictionary2.Keys.ElementAt(i)].FileName);
                        break;
                    case 4:
                        Console.WriteLine("Mean: {0}      {1}", skyDictionary2.Values.ElementAt(i), dangus[skyDictionary2.Keys.ElementAt(i)].FileName);
                        break;
                }
            }
            return skyDictionary2;
        }

        static void counteris(List<Skies> dangus, Dictionary<int, double> skyDictionary, int [] array)
        {
            if (dangus[skyDictionary.Keys.ElementAt(0)].Name == "Debesuota")
            {
                array[0]++;
            }
            else if (dangus[skyDictionary.Keys.ElementAt(0)].Name == "Labai Debesuota")
            {
                array[1]++;
            }
            else if (dangus[skyDictionary.Keys.ElementAt(0)].Name == "Saulėta su debesimis")
            {
                array[2]++;
            }
            else if (dangus[skyDictionary.Keys.ElementAt(0)].Name == "Lengvai debesuota")
            {
                array[3]++;
            }
            else
            {
                array[4]++;
            }
        }

        static string oroSalygosFunc(int i)
        {
            string oroSalygos = "";
            switch (i)
            {
                case 0:
                    oroSalygos = "Debesuota";
                    break;
                case 1:
                    oroSalygos = "Labai Debesuota";
                    break;
                case 2:
                    oroSalygos = "Sauleta su debesimis";
                    break;
                case 3:
                    oroSalygos = "Lengvai debesuota";
                    break;
                case 4:
                    oroSalygos = "Giedra";
                    break;
            }

            return oroSalygos;
        }
        static void Main(string[] args)
        {
            int test = 0;
            
            List<Skies> dangus = new List<Skies>(); 
            List<TempSky> tempDangus = new List<TempSky>();
            classMakeTemp(out tempDangus, @"C:\Users\Laptop\Desktop\temp");
            Console.WriteLine("Press a or anything else.");
            if(tempDangus.Count>0 && (Console.ReadKey().KeyChar == 'a'))
                classMake(out dangus, @"C:\Users\Laptop\Desktop\asd2", tempDangus[0].Image);
            else if (tempDangus.Count > 0)
            {
                dangus = DeSerializeObject<List<Skies>>(@".\dangau.xml");
            }
            else
                System.Environment.Exit(1);
            string oroSalygos = "";
            int[] debesuotumasArray = new int[5];
            

            GC.Collect();
            Dictionary<int, double> skyDictionary = new Dictionary<int, double>();

            for (int i = 0; i < dangus.Count; i = i+5)
            {
                if (dangus.Count - i > 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        skyDictionary = Surykiavimas(skyDictionary, tempDangus, dangus, test, j);
                        counteris(dangus, skyDictionary, debesuotumasArray);
                    }
                }
                
            }

            Window show = new Window("Dangus", WindowMode.FreeRatio);

            oroSalygos = oroSalygosFunc(debesuotumasArray.ToList().IndexOf(debesuotumasArray.Max()));
            
            if (tempDangus.Count>0)
            {
                Skies sky = new Skies();
                

                Point vieta = new Point(100,100);
                Cv2.PutText(tempDangus[test].Image, oroSalygos, vieta, FontFace.Italic, 3, Scalar.Red, 5);
                show.Image = tempDangus[test].Image;
                WaitKey();
                if (Console.ReadKey().KeyChar == 'a')
                {
                    sky.getSkies(tempDangus[test].Mean, tempDangus[test].Variance, tempDangus[test].FileName,
                        tempDangus[test].Energy, tempDangus[test].Entropy, tempDangus[test].Smoothness, oroSalygos);
                    dangus.Add(sky);
                }
                else
                {
                    Console.WriteLine("0: Debesuota,  1:Labai Debesuota, 2:Sauleta su debesimis, 3:Lengvai Debesuota, 4:Giedra");
                    string irasyta= Console.ReadLine();
                    int inSk = Int32.Parse(irasyta);
                    oroSalygos = oroSalygosFunc(inSk);
                    if (oroSalygos == "")
                    {
                        System.Environment.Exit(0);
                    }
                    sky.getSkies(tempDangus[test].Mean, tempDangus[test].Variance, tempDangus[test].FileName,
                        tempDangus[test].Energy, tempDangus[test].Entropy, tempDangus[test].Smoothness, oroSalygos);
                    dangus.Add(sky);
                }
                SerializeObject<List<Skies>>(dangus, @".\dangau.xml");
            }
            else
            {
                System.Environment.Exit(1);
            }
        }
    }
}
