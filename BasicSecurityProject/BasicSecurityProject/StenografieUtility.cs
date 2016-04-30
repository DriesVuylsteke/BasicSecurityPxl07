using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BasicSecurityProject
{
    class StenografieUtility
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
                    b = (byte)(b + (pixels[index] & 3));
                    b = (byte)(b + ((pixels[index + 1] & 3) << 2));
                    b = (byte)(b + ((pixels[index + 2] & 3) << 4));
                    b = (byte)(b + ((pixels[index + 3] & 3) << 6));
                    decode[pixCounter] = b;
                    pixCounter++;
                }
            }

            //the first 12 bytes are the lenght of the message, key and hash
            byte[] byteLenght1 = { decode[0], decode[1], decode[2], decode[3] };
            byte[] byteLenght2 = { decode[4], decode[5], decode[6], decode[7] };
            byte[] byteLenght3 = { decode[8], decode[9], decode[10], decode[11] };
            int lenght1 = BitConverter.ToInt32(byteLenght1, 0);
            int lenght2 = BitConverter.ToInt32(byteLenght2, 0);
            int lenght3 = BitConverter.ToInt32(byteLenght3, 0);

            byte[] message = new byte[lenght1];
            byte[] key = new byte[lenght2];
            byte[] hash = new byte[lenght3];
            Array.Copy(decode, 12, message, 0, lenght1);
            Array.Copy(decode, 12 + lenght1, message, 0, lenght2);
            Array.Copy(decode, 12 + lenght1 + lenght2, message, 0, lenght3);
            File.WriteAllBytes(messageFile, message);
            File.WriteAllBytes(keyFile, message);
            File.WriteAllBytes(hashFile, message);
        }

        public static BitmapSource embed(string file, string key, string hash, BitmapSource bmp)
        {
            Byte[] fileB = File.ReadAllBytes(file);
            Byte[] fileK = File.ReadAllBytes(key);
            Byte[] fileH = File.ReadAllBytes(hash);

            Byte[] toEmbed = new byte[12 + fileB.Length + fileK.Length + fileH.Length];
            Array.Copy(BitConverter.GetBytes(fileB.Length), toEmbed, 4);
            Array.Copy(BitConverter.GetBytes(fileK.Length), 0, toEmbed, 4, 4);
            Array.Copy(BitConverter.GetBytes(fileH.Length), 0, toEmbed, 8, 4);

            Array.Copy(fileB, 0, toEmbed, 12, fileB.Length);
            Array.Copy(fileK, 0, toEmbed, 12 + fileB.Length, fileK.Length);
            Array.Copy(fileH, 0, toEmbed, 12 + fileB.Length + fileK.Length, fileH.Length);

            int total = fileB.Length + fileK.Length + fileH.Length + 12;
            int stride = bmp.PixelWidth * 4;
            byte[] pixels = new byte[bmp.PixelHeight * stride];
            int pixCounter = 0;
            bmp.CopyPixels(pixels, stride, 0);

            // pass through the rows
            for (int i = 0; i < bmp.PixelHeight && pixCounter < total; i++)
            {
                // pass through each row
                for (int j = 0; j < bmp.PixelWidth && pixCounter < total; j++)
                {
                    int index = i * stride + 4 * j;
                    // RGBA value positions of pixel
                    // R = pixels[index];
                    // G = pixels[index + 1];
                    // B = pixels[index + 2];
                    // A = pixels[index + 3];
                    // now, replace the LSB with the next value to hide

                    // store 1 byte in every pixel (2 bit in R G B and A)
                    pixels[index] = (byte)(((byte)(toEmbed[pixCounter] & 3) >> 0) + (pixels[index]&252));
                    pixels[index + 1] = (byte)(((byte)(toEmbed[pixCounter] & 12) >> 2) + (pixels[index] & 252));
                    pixels[index + 2] = (byte)(((byte)(toEmbed[pixCounter] & 48) >> 4) + (pixels[index] & 252));
                    pixels[index + 3] = (byte)(((byte)(toEmbed[pixCounter] & 192) >> 6) + (pixels[index] & 252));
                    pixCounter++;
                }
            }
            BitmapSource src = BitmapSource.Create(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, bmp.Format, bmp.Palette, pixels, stride);
            return src;

            //not needed
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
