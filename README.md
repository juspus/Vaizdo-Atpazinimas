Vaizdo-Atpazinimas

OpenCV intall line: Install-Package OpenCvSharp-AnyCPU -Version 2.4.10.20170306
Setup su 2.4.10:

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

Entropija - netvarkingumo matas.

Iš histogramos galima paskaičiuoti - Vidutinę reikšmę, Variaciją, Entropiją, Kontrastą.
Thresholding - Slenksteliavimas.

Konvoliucija - sujungiam aplinkinius pixelius tam tikru atstumu nuo norimo pixelio ir jiem duodam kažkokį gain.

#Hough transformation

Skirta kreivių atpažinimui.
Atpažinsime apskritimą ar kreives.

y=mx+b

ro ir theta

ro = x*cos(theta)+y*sin(theta)

per linijos taskus piesiant tieses gauname polineje sistemoje kreives ir vieta, kurioje susikerta kreivės bus tiesė.

#Surf Sift
Sift - mato taskus betkokiuose masteliuose ir pakreipimuose.
Keypoints - taskai kuriuos randam
Descriptors - gradientu aprasas sitiem Keypoints

#Find Homography
#Texturu ivertinimo koeficientai 
