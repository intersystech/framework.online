using System;

namespace Intersystech.Utility.QRCode.Codec.Data
{
	public interface QRCodeImage
	{
        int Width
        {
            get;

        }
        int Height
        {
            get;

        }
        int getPixel(int x, int y);
	}
}