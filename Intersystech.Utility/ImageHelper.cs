using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersystech.Utility
{
    /// <summary>
    /// 画像ヘルパー
    /// </summary>
    public sealed class ImageHelper
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ImageHelper() { }

        /// <summary>
        /// 画像をバイナリ形式へ変換します。
        /// </summary>
        /// <param name="image">画像</param>
        /// <returns>バイナリ形式</returns>
        public static byte[] ToByte(Image image)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] imageByte = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
            return imageByte;
        }

        /// <summary>
        /// 画像をバイナリ文字列へ変換します。
        /// </summary>
        /// <param name="image">画像</param>
        /// <returns>バイナリ文字列</returns>
        public static string ToString(Image image)
        {
            return string.Join("_", ToByte(image));
        }

        /// <summary>
        /// バイナリ形式を画像へ変換します。
        /// </summary>
        /// <param name="imageByte">バイナリ形式</param>
        /// <returns>画像</returns>
        public static Image ToImage(byte[] imageByte)
        {
            ImageConverter imageConverter = new ImageConverter();
            Image image = (Image)imageConverter.ConvertFrom(imageByte);
            return image;
        }

        /// <summary>
        /// バイナリ文字列を画像へ変換します。
        /// </summary>
        /// <param name="imageByteString">バイナリ文字列</param>
        /// <returns>画像</returns>
        public static Image ToImage(string imageByteString)
        {
            string[] imageByteStringArray = imageByteString.Split('_');
            byte[] imageByte = new byte[imageByteStringArray.Length];
            int i = 0;
            foreach (string item in imageByteStringArray)
            {
                imageByte[i] = byte.Parse(item);
                i++;
            }

            return ToImage(imageByte);
        }
    }
}
