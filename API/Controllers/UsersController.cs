using API.Dto;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() { 
            var users = await _userRepository.GetUsersAsync();
            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(usersToReturn);
        }

        
        [HttpGet("{id}")] // api/user/{id}
        public async Task<ActionResult<MemberDto>> GetUser(int id)
        {
            var user =  await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<MemberDto>(user));
        }

        [HttpGet("name/{username}")] // api/user/name/{username}
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user  = await _userRepository.GetMemberAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if(user == null) {
                return NotFound();
            }
            _mapper.Map(memberUpdateDto, user);
            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            var result = await _photoService.AddPhotoAsync(file);

            if (String.IsNullOrWhiteSpace(result))
            {
                return BadRequest("error uploading file");
            }

            var photo = new Photo
            {
                Url = result,
                PublicId = file.FileName,

            };

            if(user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser),new { username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null)
            {
                return NotFound();
            }

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if(photo == null)
            {
                return NotFound();
            }
            
            if(photo.IsMain)
            {
                return BadRequest("this is already yout main photo");
            }

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) 
            {
                currentMain.IsMain = false;
            }
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync() )
            {
                return NoContent();
            }
            return BadRequest("problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null)
            {
                return NotFound();
            }
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null)
            {
                return NotFound();
            }
            if(photo.IsMain)
            { 
                return BadRequest("You cannot delete your main photo"); 
            }

            if(photo.PublicId != null)
            {
                await _photoService.DeletePhotoAsync(photo.PublicId);
            }
            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting photo");
        }

    }
}
