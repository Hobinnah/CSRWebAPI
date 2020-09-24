using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface IFileManager
    {
        IEnumerable<byte> ConvertImageToByte(string ImageUrl);

        string ConvertByteToBase64String(byte[] imageByteArray);
        string ConvertImageToByte_FromByteToBase64String(string ImageUrl);

        byte[] ConvertBase64StringToByte(string base64ImageString);

        IEnumerable<byte> ConvertByteToImage(string ImageUrl);

        string ConvertImageToBase64String(string ImageUrl);

        void DeleteFIle(string Url);
    }
}
