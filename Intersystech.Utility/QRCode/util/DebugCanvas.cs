using System;
using Line = Intersystech.Utility.QRCode.Geom.Line;
using Point = Intersystech.Utility.QRCode.Geom.Point;

namespace Intersystech.Utility.QRCode.Codec.Util
{
	public interface DebugCanvas
	{
		void  println(String str);
		void  drawPoint(Point point, int color);
		void  drawCross(Point point, int color);
		void  drawPoints(Point[] points, int color);
		void  drawLine(Line line, int color);
		void  drawLines(Line[] lines, int color);
		void  drawPolygon(Point[] points, int color);
		void  drawMatrix(bool[][] matrix);
	}
}