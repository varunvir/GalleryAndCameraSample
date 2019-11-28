using System;
using System.IO;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;

namespace GalleryAndCameraSample.Droid
{
	public static class BitmapHelpers
	{
		public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
		{
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile(fileName, options);

			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > height || outWidth > width)
			{
				inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;
			}

			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

			return resizedBitmap;
		}
	}


	public class BitmapWorkerTask : AsyncTask<int, int, Bitmap>
	{
		private Android.Net.Uri uriReference;
		private int data = 0;
		private ContentResolver resolver;

		public BitmapWorkerTask(ContentResolver cr, Android.Net.Uri uri)
		{
			uriReference = uri;
			resolver = cr;
		}

		protected override Bitmap RunInBackground(params int[] p)
		{
			data = p[0];

			Bitmap mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(resolver, uriReference);
			Bitmap myBitmap = null;

			if (mBitmap != null)
			{
				Matrix matrix = new Matrix();
				if (data != 0)
				{
					matrix.PreRotate(data);
				}

				myBitmap = Bitmap.CreateBitmap(mBitmap, 0, 0, mBitmap.Width, mBitmap.Height, matrix, true);
				return myBitmap;
			}

			return null;
		}


		protected override void OnPostExecute(Bitmap bitmap)
		{
			if (bitmap != null)
			{
				MemoryStream stream = new MemoryStream();
				bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
				byte[] bitmapData = stream.ToArray();

				SamplePage.Galleryimage(bitmapData);

				bitmap.Recycle();
				GC.Collect();
			}
		}
	}
}

