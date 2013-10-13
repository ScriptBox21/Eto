using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Platform.Android.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphics"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : WidgetHandler<ag.Canvas, Graphics>, IGraphics
	{
		public GraphicsHandler()
		{
		}

		public PixelOffsetMode PixelOffsetMode { get; set; } // TODO

		public void CreateFromImage(Bitmap image)
		{
			Control = new ag.Canvas((ag.Bitmap)image.ControlObject);
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			Control.DrawLine(startx, starty, endx, endy, pen.ToAndroid());
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawRect(new RectangleF(x, y, width, height).ToAndroid(), pen.ToAndroid());
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			Control.DrawRect(new RectangleF(x, y, width, height).ToAndroid(), brush.ToAndroid());
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			Control.DrawOval(new RectangleF(x, y, width, height).ToAndroid(), brush.ToAndroid());
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawOval(new RectangleF(x, y, width, height).ToAndroid(), pen.ToAndroid());
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			Control.DrawPath(path.ToAndroid(), brush.ToAndroid());
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			Control.DrawPath(path.ToAndroid(), pen.ToAndroid());
		}

		public void DrawImage(Image image, float x, float y)
		{
			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, x, y);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, x, y, width, height);
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, source, destination);
		}


		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			throw new NotImplementedException();
		}

		public SizeF MeasureString(Font font, string text)
		{
			if(string.IsNullOrEmpty(text)) // needed to avoid exception
				return SizeF.Empty;

			// See http://stackoverflow.com/questions/7549182/android-paint-measuretext-vs-gettextbounds
			
			var paint = (font.Handler as FontHandler).Paint;
			var bounds = new ag.Rect();
			paint.GetTextBounds(text, 0, text.Length, bounds);
			
			// TODO: see the above article; the width may be truncated to the nearest integer.
			return new SizeF(bounds.Width(), bounds.Height()); 
		}

		public void Flush()
		{
			throw new NotImplementedException();
		}

		public bool Antialias
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ImageInterpolation ImageInterpolation
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool IsRetained
		{
			get { return false; }
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			Control.Translate(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			Control.Rotate(angle);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			Control.Scale(scaleX, scaleY);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			Control.Concat(matrix.ToAndroid());
		}

		public void SaveTransform()
		{
			Control.Save(ag.SaveFlags.Matrix);
		}

		public void RestoreTransform()
		{
			Control.Restore();
		}

		public RectangleF ClipBounds
		{
			get { return Control.ClipBounds.ToEto(); }
		}

		public void SetClip(RectangleF rectangle)
		{
			Control.ClipRect(rectangle.ToAndroid(), ag.Region.Op.Replace);
		}

		public void SetClip(IGraphicsPath path)
		{
			Control.ClipPath(path.ToAndroid(), ag.Region.Op.Replace);
		}

		public void ResetClip()
		{
			throw new NotImplementedException();
		}

		public void Clear(SolidBrush brush)
		{
			throw new NotImplementedException();
		}
	}
}