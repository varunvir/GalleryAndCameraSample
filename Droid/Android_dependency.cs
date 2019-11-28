using System;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Support.V4.Content;
using GalleryAndCameraSample.Droid;
using Java.IO;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(Android_dependency))]
namespace GalleryAndCameraSample.Droid
{
	public class Android_dependency : Activity, ICameraGallery
	{
		public void CameraMedia()
		{

            AppClass._dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "beeWatch");

            if (!AppClass._dir.Exists())
            {
                AppClass._dir.Mkdirs();
            }


            Intent intent = new Intent(MediaStore.ActionImageCapture);

            AppClass._file = new Java.IO.File(AppClass._dir, "sample.jpg");
           
            if (AppClass._file.Exists())
            {
                AppClass._file.Delete();

            }
            
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(AppClass._file));

            Activity activity = (Activity)Forms.Context;
            activity.StartActivityForResult(intent, 0);
        }
        public void GalleryMedia()
		{
			var imageIntent = new Intent();
			imageIntent.SetType("image/*");
			imageIntent.SetAction(Intent.ActionGetContent);
			((Activity)Forms.Context).StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
		}
	}

}
