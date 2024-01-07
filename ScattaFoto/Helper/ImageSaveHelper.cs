namespace ScattaFoto.Helper
{
    public static class ImageSaveHelper
    {
        public static async Task SaveImage(Stream imageStream, string nome)
        {
            using var stream = imageStream;
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            stream.Position = 0;
            memoryStream.Position = 0;
#if WINDOWS
            await System.IO.File.WriteAllBytesAsync(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), nome + ".jpg"), memoryStream.ToArray());
#elif ANDROID
var context = Platform.CurrentActivity;

    if (OperatingSystem.IsAndroidVersionAtLeast(29))
    {
        Android.Content.ContentResolver resolver = context.ContentResolver;
        Android.Content.ContentValues contentValues = new();
        contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, nome);
        contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "image/jpg");
        
        contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, "DCIM/AppCode/");
        Android.Net.Uri imageUri = resolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, contentValues);
        var os = resolver.OpenOutputStream(imageUri);
        Android.Graphics.BitmapFactory.Options options = new();
        options.InJustDecodeBounds = true;
        var bitmap = Android.Graphics.BitmapFactory.DecodeStream(stream);
        bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
        os.Flush();
        os.Close();
    }
    else
    {
        Java.IO.File storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
        string path = System.IO.Path.Combine(storagePath.ToString(), "image.png");
        System.IO.File.WriteAllBytes(path, memoryStream.ToArray());
        var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
        mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
        context.SendBroadcast(mediaScanIntent);
    }
#elif IOS || MACCATALYST
var image = new UIKit.UIImage(Foundation.NSData.FromArray(memoryStream.ToArray()));
image.SaveToPhotosAlbum((image, error) =>
{
});
#endif
        }
    }
}
