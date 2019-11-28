using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Uri = Android.Net.Uri;
using System.IO;
using Xamarin.Forms;
using Android.Database;
using Android.Provider;
using Android.Support.V4.App;

namespace GalleryAndCameraSample.Droid
{
    public static class AppClass
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }
    [Activity(Label = "GalleryAndCameraSample.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		public bool IsCamera;
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);
            ActivityCompat.RequestPermissions(this, new string[] { global::Android.Manifest.Permission.ReadExternalStorage,
            global::Android.Manifest.Permission.WriteExternalStorage,
            global::Android.Manifest.Permission.Camera }, 0);
            global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			IsCamera = SamplePage.IsCamera;

			base.OnActivityResult(requestCode, resultCode, data);

			if (IsCamera == true)
			{
				Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
				Uri contentUri = Uri.FromFile(AppClass._file);
				mediaScanIntent.SetData(contentUri);
				SendBroadcast(mediaScanIntent);

				//int height = Resources.DisplayMetrics.HeightPixels;
				int width = Resources.DisplayMetrics.WidthPixels;
				AppClass.bitmap = AppClass._file.Path.LoadAndResizeBitmap(width, width);

				byte[] bitmapData = new byte[0];

				if (AppClass.bitmap != null)
				{
					using (var stream = new MemoryStream())
					{
						AppClass.bitmap.Compress(Bitmap.CompressFormat.Png, 50, stream);
						bitmapData = stream.ToArray();
					}

					AppClass.bitmap = null;
				}

				GC.Collect();

				SamplePage.Cameraimage(bitmapData);
			}
			else
			{
				if (requestCode == 1)
				{
					if (resultCode == Result.Ok)
					{
						if (data.Data != null)
						{
							Android.Net.Uri uri = data.Data;

							int orientation = getOrientation(uri);
							BitmapWorkerTask task = new BitmapWorkerTask(this.ContentResolver, uri);
							task.Execute(orientation);
						}
					}
				}
			}
		}

		public int getOrientation(Android.Net.Uri photoUri)
		{
			ICursor cursor = Application.ApplicationContext.ContentResolver.Query(photoUri, new String[] { MediaStore.Images.ImageColumns.Orientation }, null, null, null);

			if (cursor.Count != 1)
			{
				return -1;
			}

			cursor.MoveToFirst();
			return cursor.GetInt(0);
		}

	}
}