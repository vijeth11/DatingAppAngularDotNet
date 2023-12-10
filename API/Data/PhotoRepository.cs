using API.Dto;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _dataContext;

        public PhotoRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Photo> GetPhotoById(int id)
        {
            return await _dataContext.Photos
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _dataContext.Photos
                    .IgnoreQueryFilters()
                    .Where(p => p.IsApproved == false)
                    .Select(p => new PhotoForApprovalDto { 
                        Id = p.Id,
                        IsApproved = p.IsApproved,
                        Url = p.Url,
                        Username = p.AppUser.UserName
                    }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            _dataContext.Photos.Remove(photo);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
