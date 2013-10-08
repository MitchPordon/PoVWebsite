using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Net.Http;
using System.Drawing.Imaging;
using PoVWebsite.Models;
using System.Web.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PoVWebsite.Content.Utility
{
    public static class Image
    {
        internal static bool CreateImage(byte[] imageData, int userId) //Once IIS is working properly SaveToDB should be moved to after the Bitmap is created but before it is saved. This way only image data will be saved.
        {
            PoVEntities db = new PoVEntities();
            using(MemoryStream ms = new MemoryStream(imageData)){
                try
                {

                    Bitmap image = new Bitmap(ms);
                    //Task.Run(() => SaveToDB(userId, imageData));
                    SaveToDB(userId, imageData);
                    User user = db.Users.SingleOrDefault(m => m.id == userId);
                    int picId = user.Pictures.Last().id;
                    Task.Run(() => SaveImages(imageData, user.username, picId));
                    //image.Save("C:\\inetpub\\wwwroot\\pov.jschroed.com\\UserImages\\" + user.username +"\\" + picId + ".jpg", ImageFormat.Jpeg); //Wont work until IIS will allow saving of files
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        
             
        }

        private static void SaveImages(byte[] imageData, String username, int picId)
        {
            System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                Bitmap orig = new Bitmap(ms);
                System.Drawing.Image Half = orig.GetThumbnailImage(orig.Width/2, orig.Height/2,myCallback,IntPtr.Zero);
                System.Drawing.Image Thumb_Lrg = GetThumb(orig, Size.Thumb_Lrg, myCallback);
                System.Drawing.Image Thumb_Med = GetThumb(orig, Size.Thumb_Med, myCallback);
                System.Drawing.Image Thumb_Sml = GetThumb(orig, Size.Thumb_Sml, myCallback);
                Half.Save("C:\\inetpub\\wwwroot\\pov.jschroed.com\\UserImages\\" + username + "\\" + picId + ".jpg", ImageFormat.Jpeg);
                Thumb_Lrg.Save("C:\\inetpub\\wwwroot\\pov.jschroed.com\\UserImages\\" + username + "\\" + picId + "_Lrg.jpg", ImageFormat.Jpeg);
                Thumb_Med.Save("C:\\inetpub\\wwwroot\\pov.jschroed.com\\UserImages\\" + username + "\\" + picId + "_Med.jpg", ImageFormat.Jpeg);
                Thumb_Sml.Save("C:\\inetpub\\wwwroot\\pov.jschroed.com\\UserImages\\" + username + "\\" + picId + "_Sml.jpg", ImageFormat.Jpeg);
            }
        }

        internal static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        internal static void SaveToDB(int id, byte[] imageData)
        {
            PoVEntities db = new PoVEntities();

                try
                {
                    byte[] resizedImage = ImageResizeByPercentAsnyc(imageData, 0, .5);
                    db.Pictures.Add(new Picture { bytes =imageData, user_id=id, half_size=resizedImage });
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    HttpResponseException exception = new HttpResponseException(HttpStatusCode.Conflict);
                    exception.Response.Content = new StringContent("Data could not be saved to the database.");
                    throw exception;
                }
            
        }

       internal  static bool IsValidImage(Stream imageStream)
        {
            if (imageStream.Length > 0)
            {
                byte[] header = new byte[4]; // Change size if needed.
                string[] imageHeaders = new[]{
                "\xFF\xD8", // JPEG
                "BM",       // BMP
                "GIF",      // GIF
                Encoding.ASCII.GetString(new byte[]{137, 80, 78, 71})}; // PNG

                imageStream.Read(header, 0, header.Length);

                bool isImageHeader = imageHeaders.Count(str => Encoding.ASCII.GetString(header).StartsWith(str)) > 0;
                if (isImageHeader == true)
                {
                    try
                    {
                        System.Drawing.Image.FromStream(imageStream).Dispose();
                        imageStream.Close();
                        return true;
                    }

                    catch
                    {

                    }
                }
            }

            imageStream.Close();
            return false;
        }


       private static byte[] ImageResizeByPercentAsnyc(byte[] origData, int pictureID, double scale)
       {
           byte[] newData;
           using (MemoryStream ms = new MemoryStream(), newStream = new MemoryStream())
           {
               ms.Write(origData, 0, origData.Length);
               Bitmap orig = new Bitmap(ms);
               int newHeight = (int)(orig.Height * .5);
               int newWidth = (int)(orig.Width * .5);
               Bitmap newImage = new Bitmap(newWidth, newHeight);
               ResizeImage(orig, ref newImage);
               newImage.Save(newStream, ImageFormat.Bmp);
               newData = newStream.ToArray();
           }
           return newData;
       }

       private static System.Drawing.Image GetThumb(Bitmap orig, PoVWebsite.Content.Utility.Image.Size s,  System.Drawing.Image.GetThumbnailImageAbort myCallback)
       {
           int max;
           switch (s)
           {
               case Size.Thumb_Lrg:
                   max = 200;
                   break;
               case Size.Thumb_Med:
                   max = 150;
                   break;
               default:
                   max=100;
                   break;
           }
           int newHeight, newWidth;
           double resizeFactor;
           if (orig.Height > orig.Width)
           {
               newHeight = max;
               resizeFactor = (double)orig.Height / max;
               newWidth = (int)(orig.Width / resizeFactor);
           }
           else
           {
               newWidth = max;
               resizeFactor = (double)orig.Width / max;
               newHeight = (int)(orig.Height / resizeFactor);
           }
           return orig.GetThumbnailImage(newWidth, newHeight, myCallback, IntPtr.Zero);

       }

       private static void ResizeImage(Bitmap orig, ref Bitmap newImage)
       {
           using (Graphics gfx = Graphics.FromImage(newImage))
           {
               gfx.DrawImage(orig, new Rectangle(0, 0, newImage.Width, newImage.Height), new Rectangle(0, 0, orig.Width, orig.Height), GraphicsUnit.Pixel);
           }

       }
       public static bool ThumbnailCallback()
       {
           return false;
       }

       public enum Size
       {
           Half, Thumb_Lrg, Thumb_Med, Thumb_Sml
       }
    }


}