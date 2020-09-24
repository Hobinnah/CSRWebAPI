using CSRWebAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Implementations
{
    public class FileManager : IFileManager
    {


        public FileManager()
        {

        }



        public IEnumerable<byte> ConvertImageToByte(string ImageUrl)
        {
            byte[] imageByteArray = null;

            using (FileStream stream = new FileStream(ImageUrl, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    imageByteArray = new byte[reader.BaseStream.Length];
                    for (int i = 0; i < reader.BaseStream.Length; i++)
                        imageByteArray[i] = reader.ReadByte();
                }
            }

            return imageByteArray;
        }

        public string ConvertByteToBase64String(byte[] imageByteArray)
        {

            string base64ImageString = Convert.ToBase64String(imageByteArray);

            return base64ImageString;
        }

        public string ConvertImageToByte_FromByteToBase64String(string ImageUrl)
        {
            IEnumerable<byte> imageByteArray = ConvertImageToByte(ImageUrl);
            string base64ImageString = $"data:image/jpg;base64,{ Convert.ToBase64String(imageByteArray.ToArray()) }";

            return base64ImageString;
        }

        public byte[] ConvertBase64StringToByte(string base64ImageString)
        {

            byte[] imageByteArray = null; // Convert.toby(imageByteArray);

            return imageByteArray;
        }

        public IEnumerable<byte> ConvertByteToImage(string ImageUrl)
        {
            byte[] imageByteArray = new byte[] { };



            return imageByteArray;
        }

        public string ConvertImageToBase64String(string ImageUrl)
        {
            IEnumerable<byte> imageByteArray = null;
            string base64ImageString = string.Empty;


            imageByteArray = ConvertByteToImage(ImageUrl);
            base64ImageString = Convert.ToBase64String(imageByteArray.ToArray());


            return base64ImageString;
        }

        public void DeleteFIle(string Url)
        {
            try
            {
                if (File.Exists(Url))
                {
                    File.Delete(Url);
                }
            }
            catch { }
        }
    }
}
