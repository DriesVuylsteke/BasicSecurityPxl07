using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stegnografie
{
    class SteganographyHelper
    {


        public static void decodePicture(BitmapSource image, string messageFile, string keyFile, string hashFile)
        {
            int stride = image.PixelWidth * 4;
            Byte[] pixels = new Byte[image.PixelHeight * stride];
            Byte[] decode = new Byte[image.PixelHeight * stride / 4];
            image.CopyPixels(pixels, stride, 0);
            int pixCounter = 0;

            // read the entire message, 1 byte per pixel
            for (int i = 0; i < image.PixelHeight; i++)
            {
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    int index = i * stride + 4 * j;
                    byte b = 0;
                    b = (byte)(b ^ pixels[index] & 3);
                    b = (byte)(b ^ pixels[index+1] & 3 << 2);
                    b = (byte)(b ^ pixels[index+2] & 3 << 4);
                    b = (byte)(b ^ pixels[index+3] & 3 << 6);
                    decode[pixCounter] = b;
                    pixCounter++;
                }
            }

            //the first 4 bytes are the lenght of the message
            byte[] byteLenght = { decode[0], decode[1], decode[2], decode[3] };
            int lenght = BitConverter.ToInt32(byteLenght, 0);
            byte[] message = new byte[lenght];
            Array.Copy(decode, 4, message, 0, lenght);
            File.WriteAllBytes(messageFile, message);
        }

        public static BitmapSource embed(Byte[] toEmbed, BitmapSource bmp)
        {
            // het gebruikte algoritme is een persoonlijk algoritme. Ik weet dus dat de eerste 4 bytes (32 bit integer) de lengte van mijn boodschap aanduiden
            Byte[] byteLenght = { toEmbed[0], toEmbed[1] , toEmbed[2] , toEmbed[3] };
            int lenght = BitConverter.ToInt32(byteLenght,0);
            int stride = bmp.PixelWidth * 4;
            byte[] pixels = new byte[bmp.PixelHeight * stride];
            int pixCounter = 0;
            bmp.CopyPixels(pixels, stride, 0);

            // pass through the rows
            for (int i = 0; i < bmp.PixelHeight && pixCounter < lenght + 4; i++)
            {
                // pass through each row
                for (int j = 0; j < bmp.PixelWidth && pixCounter < lenght + 4; j++)
                {
                    int index = i * stride + 4 * j;
                    // RGBA value positions of pixel
                    // R = pixels[index];
                    // G = pixels[index + 1];
                    // B = pixels[index + 2];
                    // A = pixels[index + 3];
                    // now, replace the LSB with the next value to hide

                    // store 1 byte in every pixel (2 bit in R G B and A)
                    pixels[index] = (byte)(((byte)(toEmbed[pixCounter] & 3)) ^ pixels[index]);
                    pixels[index+1] = (byte)(((byte)(toEmbed[pixCounter] & 12)) ^ pixels[index]);
                    pixels[index+2] = (byte)(((byte)(toEmbed[pixCounter] & 48)) ^ pixels[index]);
                    pixels[index+3] = (byte)(((byte)(toEmbed[pixCounter] & 192)) ^ pixels[index]);
                    pixCounter++;
                }
            }
            BitmapSource src = BitmapSource.Create(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, bmp.Format, bmp.Palette, pixels, stride);
            return src;

            //not needed (I think?)
            // Create a bitmap image from the bitmap source
            /*PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage bImg = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(src));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bImg.BeginInit();
            bImg.StreamSource = memoryStream;
            bImg.EndInit();

            memoryStream.Close();


            return bImg;*/
        }
    }
}
