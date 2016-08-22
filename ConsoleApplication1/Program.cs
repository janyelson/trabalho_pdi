using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ManagerImage mig = new ManagerImage("test1.png");
            Bitmap img = mig.filtroMedia(15);
            img.Save("test4.png");
        }
    }

    public class ManagerImage
    {
        private MyImage img;
        
        public ManagerImage(String url)
        {
            img = new MyImage(url);
        }

        public void convertToYIQ()
        {
            int count = 0;
            double[] yiq = new double[img.getWidth() * img.getHeight()];
            for(int i = 0;i < img.getWidth(); ++i)
            {
                for(int j = 0;j < img.getHeight(); ++j)
                {

                    yiq[count] = img.getColorYIQ(i, j)[0];
                    count++;
                    yiq[count] = img.getColorYIQ(i, j)[1];
                    count++;
                    yiq[count] = img.getColorYIQ(i, j)[2];
                    count++;
                }
            }     
        }

        public Bitmap oneColorTone(int  banda)
        {
            Object obj_aux = img.getImg();
            Bitmap monoImg = (Bitmap) obj_aux;

            for(int i = 0;i < img.getWidth(); i++)
            {
                for(int j = 0;j < img.getHeight(); j++)
                {
                    switch(banda) {
                        case 1:
                            monoImg.SetPixel(i, j, Color.FromArgb(monoImg.GetPixel(i, j).R, 0, 0));
                            break;
                        case 2:
                            monoImg.SetPixel(i, j, Color.FromArgb(monoImg.GetPixel(i, j).G, 0, 0));
                            break;
                        case 3:
                            monoImg.SetPixel(i, j, Color.FromArgb(monoImg.GetPixel(i, j).B, 0, 0));
                            break;
                        default:
                            Console.WriteLine("Erro na escolha da banda, 1 - Red, 2 - Green, 3 - Blue.");
                            return null;
                    }
                }
            }

            return monoImg;
        }

        public Bitmap monocromatic(int banda)
        {
            int r, g, b;
            Bitmap toneImg = (Bitmap)img.getImg();

            for(int i = 0; i < img.getWidth(); i++)
            {
                for(int j = 0; j < img.getHeight(); j++) {
                    r = toneImg.GetPixel(i, j).R;
                    g = toneImg.GetPixel(i, j).G;
                    b = toneImg.GetPixel(i, j).B;
                    switch(banda)
                    {
                        case 1:
                            toneImg.SetPixel(i, j, Color.FromArgb(r, r, r));
                            break;
                        case 2:
                            toneImg.SetPixel(i, j, Color.FromArgb(g, g, g));
                            break;
                        case 3:
                            toneImg.SetPixel(i, j, Color.FromArgb(b, b, b));
                            break;

                        default:
                            Console.WriteLine("Erro na escolha da banda, 1 - Red, 2 - Green, 3 - Blue.");
                            return null;
                    }
                }
            }

            return toneImg;
        }

        public Bitmap brightnessAdd(int c)
        {
            int r, g, b;
            Bitmap imgBright = (Bitmap)img.getImg();
            
            if(c > 255) { c = 255; }
            if(c < -255) { c = -255; }

            for(int i = 0;i < img.getWidth(); i++)
            {
                for(int j = 0;j < img.getHeight(); j++)
                {
                    r = imgBright.GetPixel(i, j).R + c;
                    g = imgBright.GetPixel(i, j).G + c;
                    b = imgBright.GetPixel(i, j).B + c;

                    if (r < 0) { r = 1;}
                    if (r > 255) { r = 255;}

                    if (g < 0) { g = 1; }
                    if (g > 255) { g = 255; }

                    if (b < 0) { b = 1; }
                    if (b > 255) { b = 255; }

                    imgBright.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return imgBright;
        }

        public Bitmap brightnessMult(int c)
        {
            int r, g, b;
            Bitmap imgBright = (Bitmap)img.getImg();

            if (c > 50) { c = 255; }
            if (c < -50) { c = -255; }

            for (int i = 0; i < img.getWidth(); i++)
            {
                for (int j = 0; j < img.getHeight(); j++)
                {
                    r = imgBright.GetPixel(i, j).R*c;
                    g = imgBright.GetPixel(i, j).G*c;
                    b = imgBright.GetPixel(i, j).B*c;

                    if (r < 0) { r = 1; }
                    if (r > 255) { r = 255; }

                    if (g < 0) { g = 1; }
                    if (g > 255) { g = 255; }

                    if (b < 0) { b = 1; }
                    if (b > 255) { b = 255; }

                    imgBright.SetPixel(i, j, Color.FromArgb(r, g, b));


                }
            }

            return imgBright;
        }

        public Bitmap filtroMedia(int d)
        {
            Bitmap newImg = (Bitmap)img.getImg();
            Bitmap imgClone = (Bitmap)img.getImg();
            int mediaR = 0, mediaG = 0, mediaB = 0, count = 1 ;
            int[] matrix = new int[d];
            matrix[0] = 0;
            for(int i = 1; i < d; ++i)
            {
                if(i%2 == 1)
                {
                    matrix[i] = -count;
                }
                else
                {
                    matrix[i] = count;
                    ++count;
                }
            }

            for(int i = 0; i< img.getWidth(); ++i)
            {
                for(int j = 0; j< img.getHeight(); ++j)
                {
                    for(int k = 0;k<d;++k)
                    {
                       for(int w = 0;w<d;++w)
                        {
                            if((i+matrix[k] < 0) || (j+matrix[w]) < 0 || (i + matrix[k]) > img.getWidth() - 1  || (j + matrix[w]) > img.getHeight() - 1)
                            {
                                mediaR += 0;
                                mediaG += 0;
                                mediaB += 0;
                            }
                            else
                            {
                                mediaR += imgClone.GetPixel(i + matrix[k], j + matrix[w]).R;
                                mediaG += imgClone.GetPixel(i + matrix[k], j + matrix[w]).G;
                                mediaB += imgClone.GetPixel(i + matrix[k], j + matrix[w]).B;
                            }
                           
                        }
                    }
                    mediaR = mediaR / (d * d);
                    mediaG = mediaG / (d * d);
                    mediaB = mediaB / (d * d);
                    newImg.SetPixel(i, j, Color.FromArgb(mediaR, mediaG, mediaB));
                    mediaR = 0;
                    mediaG = 0;
                    mediaB = 0;
                }
            }

            return newImg;
        }

        public Bitmap unirImagens(Bitmap img1, Bitmap img2)
        {
            if(img1.Height != img2.Height || img2.Width != img1.Width)
            {
                return null;
            }

            Bitmap newImg = (Bitmap) img1.Clone();
            int r, g, b;
            for(int i = 0; i< img1.Width; ++i)
            {
                for(int j = 0; i < img1.Height; ++j)
                {
                    r = (img1.GetPixel(i, j).R + img2.GetPixel(i, j).R )/ 2;
                    g = (img1.GetPixel(i, j).G + img2.GetPixel(i, j).G) / 2;
                    b = (img1.GetPixel(i, j).B + img2.GetPixel(i, j).B) / 2;
                    newImg.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return newImg;
        }

        public Bitmap limiarizacao(int limiar)
        {
            int r, g, b;
            Bitmap limiarImg = (Bitmap)img.getImg();

            
            for(int i = 0;i < img.getWidth(); i++)
            {
                for(int j = 0; j < img.getHeight(); ++j)
                {
                    r = limiarImg.GetPixel(i, j).R;
                    g = limiarImg.GetPixel(i, j).G;
                    b = limiarImg.GetPixel(i, j).B;
                    if(r != g || r != b || g != b) { return null; }
                    if (r < limiar)
                    {
                        limiarImg.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        limiarImg.SetPixel(i, j, Color.White);
                    }
                }
            }

            return limiarImg;

        }

    }



    public class MyImage
    {
        private Bitmap img;

        public MyImage() { }
        
        public MyImage(String url)
        {
            this.img = new Bitmap(url);
        }

        ~MyImage()
        {
            img.Dispose();
        }

        public int getWidth()
        {
            return img.Width;
        }

        public int getHeight()
        {
            return img.Height;
        }

        public int[] getColorRGB(int x, int y)
        {
            Color color = img.GetPixel(x,y);
            int[] rgb = { color.R, color.G, color.B };
            return rgb;
        }

        public double[] getColorYIQ(int n, int p)
        {
            Color color = img.GetPixel(n, p);
            double y, i, q;
            
            y = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
            i = 0.596 * color.R - 0.274 * color.G - 0.322 * color.B;
            q = 0.211 * color.R - 0.523 * color.G + 0.312 * color.B;
            double[] yiq = { y, i, q };
            return yiq;
        }

        public Object getImg()
        {
            return img.Clone();
        }
    }
}