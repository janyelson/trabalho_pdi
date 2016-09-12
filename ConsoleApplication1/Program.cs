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
            ManagerImage mig = new ManagerImage("lenac.png");
            ManagerImage mono = new ManagerImage("lena.png");
            Bitmap lena = new Bitmap("lenac.png");
            Bitmap android = new Bitmap("VGA.png");
            
            mig.convertToYIQ();
            mono.convertToYIQ();
            mig.convertToRGB().Save("lenaConvert.png");
            mig.monocromatic(2).Save("monocromatic2.png");
            mig.brightnessAdd(-50).Save("brilhoRGB-50.png");
            mig.brightnessAddY(-50).Save("brilhoY-50.png");
            mig.brightnessMult(2).Save("brilhoRGBv2.png");
            mig.brightnessMultY(2).Save("brilhoYv2.png");
            mig.negativoY().Save("negativoY.png");
            mig.negativoRGB().Save("negativoRGB.png");
            mig.oneColorTone(2).Save("colorido2.png");
            mig.filtroMediana(15).Save("mediana7.png");
            mig.filtroMedia(7).Save("media7.png");
            
            mig.unirImagens(lena, android).Save("unir.png");
            mono.limiarizacao(127, true).Save("limiar127.png");
            mono.limiarizacao(127, false).Save("limiarPadrao.png");
        }
    }

    public class ManagerImage
    {
        private MyImage img;
        private double[] yiq;
        
        public ManagerImage(String url)
        {
            img = new MyImage(url);
            
        }

        public void convertToYIQ()
        {
            int count = 0;
            yiq = new double[img.getWidth() * img.getHeight()*3];
            double[] aux = new double[3];
            for(int i = 0;i < img.getHeight(); ++i)
            {
                for(int j = 0;j < img.getWidth(); ++j)
                {
                    aux = img.getColorYIQ(i, j);
                    yiq[count] = aux[0];
                    count++;
                    yiq[count] = aux[1];
                    count++;
                    yiq[count] = aux[2];
                    count++;
                }
            }     
        }

        public Bitmap convertToRGB()
        {
            int count = 0, r, b, g;
            double y, i, q;
            Bitmap image = (Bitmap)img.getImg();
            for (int n = 0; n < img.getHeight(); ++n)
            {
                for (int p = 0; p < img.getWidth(); ++p)
                {
                    y = yiq[count];
                    count++;
                    i = yiq[count];
                    count++;
                    q = yiq[count];
                    count++;
                    r = (int)(1.0 * y + 0.956 * i + 0.621 * q);
                    g = (int)(1.0 * y - 0.272 * i - 0.647 * q);
                    b = (int)(1.0 * y - 1.106 * i + 1.703 * q);

                    r = limite(r);
                    g = limite(g);
                    b = limite(b);
                    
                    image.SetPixel(n, p, Color.FromArgb(r, g, b));
                }
            }

            return image;
        }

        public Bitmap oneColorTone(int  banda)
        {
            Object obj_aux = img.getImg();
            Bitmap monoImg = (Bitmap) obj_aux;

            for(int i = 0;i < img.getHeight(); i++)
            {
                for(int j = 0;j < img.getWidth(); j++)
                {
                    switch(banda) {
                        case 1:
                            monoImg.SetPixel(i, j, Color.FromArgb(monoImg.GetPixel(i, j).R, 0, 0));
                            break;
                        case 2:
                            monoImg.SetPixel(i, j, Color.FromArgb(0, monoImg.GetPixel(i, j).G, 0));
                            break;
                        case 3:
                            monoImg.SetPixel(i, j, Color.FromArgb(0, 0, monoImg.GetPixel(i, j).B));
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

            for(int i = 0; i < img.getHeight(); i++)
            {
                for(int j = 0; j < img.getWidth(); j++) {
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

        public Bitmap negativoRGB()
        {
            Bitmap image = (Bitmap)img.getImg();
            int r, g, b;
            for(int i = 0; i < img.getHeight(); ++i)
            {
                for (int j = 0; j < img.getWidth(); ++j) {
                    r = 255 - image.GetPixel(i, j).R;
                    g = 255 - image.GetPixel(i, j).G;
                    b = 255 - image.GetPixel(i, j).B;
                    image.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return image;
        }

        public Bitmap negativoY()
        {
            Bitmap image;
            convertToYIQ();
            double y;
            for (int i = 0; i < yiq.Length; i += 3)
            {
                yiq[i] = 255 - yiq[i];
            }
            image = convertToRGB();
            return image;
        }

        public Bitmap brightnessAdd(int c)
        {
            int r, g, b;
            Bitmap imgBright = (Bitmap)img.getImg();
            
            if(c > 255) { c = 255; }
            if(c < -255) { c = -255; }

            for(int i = 0;i < img.getHeight(); i++)
            {
                for(int j = 0;j < img.getWidth(); j++)
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

        public Bitmap brightnessAddY(int c)
        {
            Bitmap image;

            if (c > 255) { c = 255; }
            if (c < -255) { c = -255; }
            convertToYIQ();
            
            for (int i = 0; i < yiq.Length; i += 3)
            {
                yiq[i] = yiq[i] + c;
            }
            image = convertToRGB();
            return image;
        }

        public Bitmap brightnessMult(int c)
        {
            int r, g, b;
            Bitmap imgBright = (Bitmap)img.getImg();

            if (c > 255) { c = 255; }
            if (c < -255) { c = -255; }

            for (int i = 0; i < img.getHeight(); i++)
            {
                for (int j = 0; j < img.getWidth(); j++)
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

        public Bitmap brightnessMultY(int c)
        {
            Bitmap image;

            if (c > 255) { c = 255; }
            if (c < -255) { c = -255; }
            convertToYIQ();

            for (int i = 0; i < yiq.Length; i += 3)
            {
                yiq[i] = yiq[i]*c;
            }
            image = convertToRGB();
            return image;
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
                    matrix[i] = count;
                }
                else
                {
                    matrix[i] = -count;
                    ++count;
                }
            }

            for(int i = 0; i< img.getHeight(); ++i)
            {
                for(int j = 0; j< img.getWidth(); ++j)
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

        public Bitmap filtroMediana(int d)
        {
            Bitmap newImg = (Bitmap)img.getImg();
            Bitmap imgClone = (Bitmap)img.getImg();
            int count = 0, aux = 0;
            int[] matrix = new int[d];
            int[] matrixFiltroR = new int[d * d];
            int[] matrixFiltroG = new int[d * d];
            int[] matrixFiltroB = new int[d * d];

            matrix[0] = 0;
            for (int i = 1; i < d; ++i)
            {
                if (i % 2 == 1)
                {
                    matrix[i] = aux;
                }
                else
                {
                    matrix[i] = -aux;
                    ++aux;
                }
            }

            for (int i = 0; i < img.getHeight(); ++i)
            {
                for (int j = 0; j < img.getWidth(); ++j)
                {
                    for (int k = 0; k < d; ++k)
                    {
                        for (int w = 0; w < d; ++w)
                        {
                            if ((i + matrix[k] < 0) || (j + matrix[w]) < 0 || (i + matrix[k]) > img.getWidth() - 1 || (j + matrix[w]) > img.getHeight() - 1)
                            {
                                matrixFiltroR[count] = 0;
                                matrixFiltroG[count] = 0;
                                matrixFiltroB[count] = 0;
                            }
                            else
                            {
                                matrixFiltroR[count] = imgClone.GetPixel(i + matrix[k], j + matrix[w]).R;
                                matrixFiltroG[count] = imgClone.GetPixel(i + matrix[k], j + matrix[w]).G;
                                matrixFiltroB[count] = imgClone.GetPixel(i + matrix[k], j + matrix[w]).B;
                            }
                            count++;
                        }
                    }
                    count = 0;
                    int medRed = mediana(d, matrixFiltroR);
                    int medGreen = mediana(d, matrixFiltroG);
                    int medBlue = mediana(d, matrixFiltroB);
                    newImg.SetPixel(i, j, Color.FromArgb(medRed, medGreen, medBlue));
                }
            }
            return newImg;
        }

        public Bitmap unirImagens(Bitmap img1, Bitmap img2)
        {
            if(img1.Height != img2.Height || img2.Width != img1.Width)
            {
                return img1;
            }
            Bitmap newImg = new Bitmap(img1.Width, img1.Height);

            int r = 0, g = 0, b = 0, a = 0, a1 = 0, a2 = 0, r1 = 0, r2 = 0, g1 = 0, g2 = 0, b1 = 0, b2 = 0;
            for (int i = 0; i< img1.Height; ++i)
            {
                for(int j = 0; j < img1.Width; ++j)
                {
                    r1 = img1.GetPixel(i, j).R;
                    r2 = img2.GetPixel(i, j).R;
                    g1 = img1.GetPixel(i, j).G;
                    g2 = img2.GetPixel(i, j).G;
                    b1 = img1.GetPixel(i, j).B;
                    b2 = img2.GetPixel(i, j).B;
                    a1 = img1.GetPixel(i, j).A;
                    a2 = img2.GetPixel(i, j).A;
                    a = (a1 + a2)/ 2;

                    r = (r1 + r2) / 2;
                    g = (g1 + g2) / 2;
                    b = (b1 + b2) / 2;
                    

                    newImg.SetPixel(i, j, Color.FromArgb(a, r, g, b));
                }
            }
            
            return newImg;
        }

        public Bitmap limiarizacao(int l, Boolean m)
        {
            int r, g, b;
           
            int limiar = l;
            Bitmap limiarImg = (Bitmap)img.getImg();
            if(!m)
            {
                int media = 0; ;
                for(int i = 0; i < img.getHeight(); ++i)
                {
                    for(int j = 0; j < img.getWidth(); ++j)
                    {
                        media += img.getColorRGB(i, j)[0] + img.getColorRGB(i, j)[1] + img.getColorRGB(i, j)[2];
                    }
                }
                limiar = (int) media / (img.getWidth() * img.getHeight() * 3);
            }
            
            for(int i = 0;i < img.getHeight(); i++)
            {
                for(int j = 0; j < img.getWidth(); ++j)
                {
                    r = limiarImg.GetPixel(i, j).R;
                    g = limiarImg.GetPixel(i, j).G;
                    b = limiarImg.GetPixel(i, j).B;
                    if(r != g || r != b || g != b) { return null; }
                    if (r < limiar)
                    {
                        limiarImg.SetPixel(i, j, Color.FromArgb(0,0,0));
                    }
                    else
                    {
                        limiarImg.SetPixel(i, j, Color.FromArgb(255,255,255));
                    }
                }
            }

            return limiarImg;
        }

        private int limite(int x)
        {
            if(x > 255)
            {
                x = 255;
            }
            if(x < 0)
            {
                x = 0;
            }

            return x;
        }

        private int mediana(int d, int[] matrix)
        {
            int i = 0;
            Boolean houveTroca = true;
            while(houveTroca)
            {
                houveTroca = false;
                for (i = 0; i < (d * d)-1; ++i)
                {
                    if(matrix[i] > matrix[i+1])
                    {
                        int aux = matrix[i];
                        matrix[i] = matrix[i + 1];
                        matrix[i + 1] = aux;
                        houveTroca = true;
                    }
                }
            }

            return matrix[i / 2];
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
