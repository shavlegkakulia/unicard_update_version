using System;
using UIKit;
using Foundation;
using System.Drawing;
using CoreGraphics;

namespace iCunOS.BillBoards
{
	public static class ImageHelper
	{
		public static UIImage FromBase64 (string base64Data)
		{  
			NSData imageData;  
			try {  
				imageData = new NSData (base64Data, NSDataBase64DecodingOptions.None);  
			} catch (Exception ex) {  
				if (!ex.Message.StartsWith ("Could not initialize an instance of the type 'MonoTouch.Foundation.NSData'")) {  
					throw;  
				}  
				var encodedDataAsBytes = Convert.FromBase64String (base64Data);  
				imageData = NSData.FromArray (encodedDataAsBytes);    
			}  
			return UIImage.LoadFromData (imageData);  
		}

		public static string ToBase64String (UIImage image)
		{
			return image.AsJPEG ().GetBase64EncodedString (NSDataBase64EncodingOptions.None);
		}

		public static UIImage MinResizeImage (UIImage sourceImage, nfloat maxWidth, nfloat maxHeight)
		{
			var sourceSize = sourceImage.Size;
			nfloat maxResizeFactor = (nfloat)Math.Min (maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1)
				return sourceImage;
			nfloat width = maxResizeFactor * sourceSize.Width;
			nfloat height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext (new CoreGraphics.CGSize (width, height));
			sourceImage.Draw (new CoreGraphics.CGRect (0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return resultImage;
		}

		public static UIImage MaxResizeImage (UIImage sourceImage, nfloat maxWidth, nfloat maxHeight)
		{
			var sourceSize = sourceImage.Size;
			nfloat maxResizeFactor = (nfloat)Math.Max (maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1)
				return sourceImage;
			nfloat width = maxResizeFactor * sourceSize.Width;
			nfloat height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext (new CoreGraphics.CGSize (width, height));
			sourceImage.Draw (new CoreGraphics.CGRect (0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return resultImage;
		}

		// resize the image (without trying to maintain aspect ratio)
		public static UIImage ResizeImage (UIImage sourceImage, float width, float height)
		{
			UIGraphics.BeginImageContext (new SizeF (width, height));
			sourceImage.Draw (new RectangleF (0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return resultImage;
		}

		// crop the image, without resizing
		private static UIImage CropImage (UIImage sourceImage, int crop_x, int crop_y, int width, int height)
		{
			var imgSize = sourceImage.Size;
			UIGraphics.BeginImageContext (new SizeF (width, height));
			var context = UIGraphics.GetCurrentContext ();
			var clippedRect = new RectangleF (0, 0, width, height);
			context.ClipToRect (clippedRect);
			var drawRect = new CoreGraphics.CGRect (-crop_x, -crop_y, imgSize.Width, imgSize.Height);
			sourceImage.Draw (drawRect);
			var modifiedImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return modifiedImage;
		}

		public static UIImage FromUrl (string uri)
		{
			uri = uri.Replace (@"\", "/");
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		public static UIImage Rotate90 (this UIImage imageIn)
		{			
			imageIn = imageIn.ScaleAndRotateImage (UIImageOrientation.Right);
			return imageIn;
		}

		public static UIImage ScaleAndRotateImage (this UIImage imageIn, UIImageOrientation orIn)
		{
			int kMaxResolution = 2048;

			CGImage imgRef = imageIn.CGImage;
			float width = imgRef.Width;
			float height = imgRef.Height;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity ();
			RectangleF bounds = new RectangleF (0, 0, width, height);

			if (width > kMaxResolution || height > kMaxResolution) {
				float ratio = width / height;

				if (ratio > 1) {
					bounds.Width = kMaxResolution;
					bounds.Height = bounds.Width / ratio;
				} else {
					bounds.Height = kMaxResolution;
					bounds.Width = bounds.Height * ratio;
				}
			}

			float scaleRatio = bounds.Width / width;
			SizeF imageSize = new SizeF (width, height);
			UIImageOrientation orient = orIn;
			float boundHeight;

			switch (orient) {
			case UIImageOrientation.Up:                                        //EXIF = 1
				transform = CGAffineTransform.MakeIdentity ();
				break;

			case UIImageOrientation.UpMirrored:                                //EXIF = 2
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, 0f);
				transform = CGAffineTransform.MakeScale (-1.0f, 1.0f);
				break;

			case UIImageOrientation.Down:                                      //EXIF = 3
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, imageSize.Height);
				transform = CGAffineTransform.Rotate (transform, (float)Math.PI);
				break;

			case UIImageOrientation.DownMirrored:                              //EXIF = 4
				transform = CGAffineTransform.MakeTranslation (0f, imageSize.Height);
				transform = CGAffineTransform.MakeScale (1.0f, -1.0f);
				break;

			case UIImageOrientation.LeftMirrored:                              //EXIF = 5
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, imageSize.Width);
				transform = CGAffineTransform.MakeScale (-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate (transform, 3.0f * (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.Left:                                      //EXIF = 6
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (0.0f, imageSize.Width);
				transform = CGAffineTransform.Rotate (transform, 3.0f * (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.RightMirrored:                             //EXIF = 7
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeScale (-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate (transform, (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.Right:                                     //EXIF = 8
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, 0.0f);
				transform = CGAffineTransform.Rotate (transform, (float)Math.PI / 2.0f);
				break;

			default:
				throw new Exception ("Invalid image orientation");
			}

			UIGraphics.BeginImageContext (bounds.Size);

			CGContext context = UIGraphics.GetCurrentContext ();

			if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left) {
				context.ScaleCTM (-scaleRatio, scaleRatio);
				context.TranslateCTM (-height, 0);
			} else {
				context.ScaleCTM (scaleRatio, -scaleRatio);
				context.TranslateCTM (0, -height);
			}

			context.ConcatCTM (transform);
			context.DrawImage (new RectangleF (0, 0, width, height), imgRef);

			UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return imageCopy;
		}

		public static UIImage ImageOfCircle (System.Drawing.RectangleF frame, UIColor strokeColor, float strokeWidth)
		{
			UIGraphics.BeginImageContextWithOptions (new SizeF (frame.Width, frame.Height), false, 0);

			DrawCircle (frame, strokeColor, strokeWidth);

			var imageOfCircle = UIGraphics.GetImageFromCurrentImageContext ();

			UIGraphics.EndImageContext ();

			return imageOfCircle;
		}

		public static void DrawCircle (RectangleF frame, UIColor strokeColor, float strokeWidth)
		{
			var ovalPath = UIBezierPath.FromOval (new RectangleF (frame.X + 2.0f, frame.Y + 2.0f, frame.Width - (strokeWidth + 2.0f), frame.Height - (strokeWidth + 2.0f)));

			strokeColor.SetStroke ();

			ovalPath.LineWidth = strokeWidth;

			ovalPath.Stroke ();

		}
	}
}

