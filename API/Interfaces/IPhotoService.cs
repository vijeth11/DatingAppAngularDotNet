namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<string> AddPhotoAsync(IFormFile photoFile);
        Task DeletePhotoAsync(string filename);

    }
}
