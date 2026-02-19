namespace PhotoApp.Api.MediaConverter
{
    public class MediaConverter : IDisposable
    {
        private IFormFile _originalFile;
        public MediaConverter(IFormFile file) { 
            _originalFile = file;
        }

        public void Dispose() { }

        //public IFormFile ConvertToPreview()
        //{
        //    return new FormFile();
        //}

        //public IFormFile ConvertToThumbnail()
        //{
        //    return new FormFile();
        //}


    }
}
