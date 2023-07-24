using API.Helpers;
using API.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        FirebaseStorageSettings _storageSettings;
        public PhotoService(IOptions<FirebaseStorageSettings> config) { 
            _storageSettings = config.Value;
        }
        public async Task<string> AddPhotoAsync(IFormFile photoFile)
        {
            string publicLink = "";
            if(photoFile.Length > 0)
            {
                using var stream = photoFile.OpenReadStream();
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_storageSettings.ApiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(_storageSettings.AuthEmail, _storageSettings.AuthPassword);

                var cancellation = new CancellationTokenSource();

                var upload = new FirebaseStorage(
                    _storageSettings.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("images")
                    .Child(photoFile.FileName)
                    .PutAsync(stream, cancellation.Token);
                try
                {
                    publicLink = await upload;
                }catch (Exception ex) { 
                    Console.WriteLine("Exception was thrown: {0}",ex);
                }
            }

            return publicLink;
        }

        public async Task DeletePhotoAsync(string filename)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(_storageSettings.ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(_storageSettings.AuthEmail, _storageSettings.AuthPassword);

            var cancellation = new CancellationTokenSource();

            var delete = new FirebaseStorage(
                _storageSettings.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                }).Child("images")
                .Child(filename)
                .DeleteAsync();

            try
            {
                await delete;
            }catch(Exception ex) 
            { 
                Console.WriteLine("Exception thrown while deleting: {0}", ex);
            }
        }
    }
}
