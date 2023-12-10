using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoRepository _photoRepository;
        private readonly IPhotoService _photoService;
        private readonly IUserRepository _userRepository;
        public AdminController(UserManager<AppUser> userManager,IPhotoRepository photoRepository, IPhotoService photoService, IUserRepository userRepository) 
        {
            _userManager = userManager;
            _photoRepository = photoRepository;
            _photoService = photoService;
            _userRepository = userRepository;
        }

        [Authorize(Policy= "RequireAdminRole")]
        [HttpGet("users-with-roles")]
       public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
               .OrderBy(u => u.UserName)
               .Select(u => new
               {
                   u.Id,
                   Username = u.UserName,
                   Roles = u.UserRoles.Select(r => r.Role.Name).ToList(),
               }).ToListAsync();
            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");
            var selectedRoles = roles.Split(",").ToArray();
            var user = await _userManager.FindByNameAsync(username);
            
            if(user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy ="ModeratePhotoRule")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetphotosForModeration()
        {
            var photos = await _photoRepository.GetUnapprovedPhotos();
            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRule")]
        [HttpGet("photos-for-approval")]
        public async Task<ActionResult> GetPhotosForApproval()
        {
            var photos = await _photoRepository.GetUnapprovedPhotos();
            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRule")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _photoRepository.GetPhotoById(photoId);
            if (photo == null) return BadRequest("Photo was not found");
            photo.IsApproved = true;
            var user = await this._userRepository.GetUserByPhotoId(photoId);
            if(!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
            await _photoRepository.SaveAllAsync();
            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRule")]
        [HttpPost("RejectPhoto/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
           var photo = await this._photoRepository.GetPhotoById(photoId);
            if(photo.PublicId != null)
            {
                await this._photoService.DeletePhotoAsync(photo.PublicId);
                this._photoRepository.RemovePhoto(photo);
            }
            else
            {
                this._photoRepository.RemovePhoto(photo);
            }            
            await this._photoRepository.SaveAllAsync();
            return Ok();
        }
        
    }
}
