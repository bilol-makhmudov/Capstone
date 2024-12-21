namespace Capstone.Repositories.Interfaces;

public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile imageFile);
}