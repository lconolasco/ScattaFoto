using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics.Platform;


namespace ScattaFoto.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
       // /data/user/0/it.lconolasco.scattaphoto/cache/1000069284.jpg"
        
        [ObservableProperty]
        public ImageSource? _imageOriginale;

        [ObservableProperty]
        public ImageSource? _imageResize;


        [RelayCommand]
        private async Task CaricaFoto(object obj)
        {
            FileResult foto = await MediaPicker.Default.PickPhotoAsync();
             
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, foto.FileName);
            using Stream sourceStream = await foto.OpenReadAsync();
            using FileStream localFileStream = File.OpenWrite(localFilePath);

            ImageResize = foto.FullPath;
        }

       
        [RelayCommand]
        private async Task ScattaFoto(object obj)
        {
            

            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                //Acquisizione dell'imagine e salvataggio di essa
                try
                {
                    if(photo is not null)
                    {
                        //Trasferimento della imagine al flusso di dati
                        using Stream sourceStream = await photo.OpenReadAsync();
                        ImageOriginale = photo.FullPath;

                        //Ridimesionamento dell'imagine con perdita di qualità dimensione ridottissima
                        Microsoft.Maui.Graphics.IImage image = PlatformImage.FromStream(sourceStream);
                        var imageR = image.Downsize(480, false).AsStream();
                        
                        //Salvataggio della imagine ridimenzionata
                        _ = Helper.ImageSaveHelper.SaveImage(imageR, "rezise"+photo.FileName);
                        
                        //Salvataggio dell'imagine originale(dimensione elevata)
                       // _ = Helper.ImageSaveHelper.SaveImage(sourceStream, photo.FileName);

                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Errore",ex.Message,"Ok");
                }
                
            }
        }
    }
}
