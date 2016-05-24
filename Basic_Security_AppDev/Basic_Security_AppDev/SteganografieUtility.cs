using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Basic_Security_AppDev
{
    class SteganografieUtility
    {
        public static void decodePicture(string imageLocation, string messageFile, string keyFile, string hashFile)
        {
            BitmapDecoder decoder = new BmpBitmapDecoder(new Uri(imageLocation), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource image = decoder.Frames[0];
            int stride = image.PixelWidth * 4;
            Byte[] pixels = new Byte[image.PixelHeight * stride];
            Byte[] decode = new Byte[image.PixelHeight * stride / 4];
            image.CopyPixels(pixels, stride, 0);
            int pixCounter = 0;
            int bitCounter = 0;
            byte b = 0;
            // read the entire message, 1 byte per pixel
            for (int i = 0; i < image.PixelHeight; i++)
            {
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    int index = i * stride + 4 * j;            
                    for(int pixIndex = 0; pixIndex < 3; pixIndex++)
                    {
                        b = (byte)(b + ((pixels[index+pixIndex] & 3) << bitCounter));
                        if (bitCounter == 6)
                        {
                            bitCounter = 0;
                            decode[pixCounter] = b;
                            pixCounter++;
                            b = 0;
                        }
                        else
                            bitCounter += 2;
                    }       
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
            Array.Copy(decode, 12 + lenght1, key, 0, lenght2);
            Array.Copy(decode, 12 + lenght1 + lenght2, hash, 0, lenght3);
            File.WriteAllBytes(messageFile, message);
            File.WriteAllBytes(keyFile, key);
            File.WriteAllBytes(hashFile, hash);
        }

        public static void embed(string file, string key, string hash, string imageLocation, string destinationFolder)
        {
            BitmapSource bmp = new BitmapImage(new Uri(imageLocation));



            Byte[] fileB = File.ReadAllBytes(file);
            Byte[] fileK = File.ReadAllBytes(key);
            Byte[] fileH = File.ReadAllBytes(hash);

            Byte[] toEmbed = new byte[12 + fileB.Length + fileK.Length + fileH.Length];
            Byte[] test = BitConverter.GetBytes(fileB.Length);
            Array.Copy(test, toEmbed, 4);
            int testInt = BitConverter.ToInt32(test, 0);
            Array.Copy(BitConverter.GetBytes(fileK.Length), 0, toEmbed, 4, 4);
            Array.Copy(BitConverter.GetBytes(fileH.Length), 0, toEmbed, 8, 4);

            Array.Copy(fileB, 0, toEmbed, 12, fileB.Length);
            Array.Copy(fileK, 0, toEmbed, 12 + fileB.Length, fileK.Length);
            Array.Copy(fileH, 0, toEmbed, 12 + fileB.Length + fileK.Length, fileH.Length);

            int total = fileB.Length + fileK.Length + fileH.Length + 12;
            int stride = bmp.PixelWidth * 4;
            byte[] pixels = new byte[bmp.PixelHeight * stride];
            int pixCounter = 0;
            int bitCounter = 0;
            bmp.CopyPixels(pixels, stride, 0);

            // pass through the rows
            for (int i = 0; i < bmp.PixelHeight && pixCounter < total -1; i++)
            {
                // pass through each row
                for (int j = 0; j < bmp.PixelWidth && pixCounter < total -1; j++)
                {
                    int index = i * stride + 4 * j;
                    // RGBA value positions of pixel
                    // R = pixels[index];
                    // G = pixels[index + 1];
                    // B = pixels[index + 2];
                    // A = pixels[index + 3];
                    // now, replace the LSB with the next value to hide

                    // store 1 byte in every pixel (2 bit in R G B and A)
                    for(int pixIndex = 0; pixIndex < 3; pixIndex++)
                    {
                        pixels[index+pixIndex] = (byte)(((byte)(toEmbed[pixCounter] & GetAndFromBitCounter(bitCounter)) >> bitCounter) + (pixels[index+pixIndex] & 252));
                        if (bitCounter >= 6)
                        {
                            bitCounter = 0;
                            pixCounter++;
                        }
                        else
                            bitCounter += 2;
                    }
                }
            }
            BitmapSource src = BitmapSource.Create(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, bmp.Format, bmp.Palette, pixels, stride);
            using (var fileStream = new FileStream(destinationFolder + "\\embedded.bmp", FileMode.Create))
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                encoder.Save(fileStream);
            }
            Console.WriteLine("Geslaagd");
        }

        private static int GetAndFromBitCounter(int bitCounter)
        {
            switch (bitCounter)
            {
                case 0:
                    return 3;
                case 2:
                    return 12;
                case 4:
                    return 48;
                default:
                    return 192;
            }
        }
    }
}
