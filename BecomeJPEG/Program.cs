using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace BecomeJPEG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string windowName = "first window";
            CvInvoke.NamedWindow(windowName);

            Mat img = new Mat(200, 400, DepthType.Cv8U, 3);
            img.SetTo(new Bgr(255, 0, 0).MCvScalar);

            CvInvoke.PutText(
                img,
                "Hello World",
                new System.Drawing.Point(10, 80), FontFace.HersheyComplex,
                1.0,
                new Bgr(0, 255, 0).MCvScalar);

            CvInvoke.Imshow(windowName, img);
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyWindow(windowName);
        }
    }
}
