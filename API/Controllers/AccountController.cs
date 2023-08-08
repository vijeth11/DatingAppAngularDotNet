using API.Data;
using API.Dto;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AccountController(DataContext context, ITokenService tokenService, IUserRepository userRepository, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("register")] //POST: api/account/register
        // no need to add [FromBody] as ApiController in BaseController will handle mapping of json
        // body object to parameter object. If ApiController is not there then it is required
        // and you need to check ModelState for validation errors as well.
        // I added here simply to show how to use
        public async Task<ActionResult<AppUser>> Register([FromBody]RegisterDto register)
        {
            if(await UserExixts(register.Username))
            {
                return BadRequest("Username is taken");
            }
            var user = _mapper.Map<AppUser>(register);

            using var hmac = new HMACSHA512();
            user.UserName = register.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            user.PasswordSalt = hmac.Key;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDto() { 
                Username = user.UserName, 
                Token = _tokenService.CreateToken(user), 
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return Unauthorized("invalid username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("invalid password");
            }

            return Ok(new UserDto() { 
                Username = user.UserName, 
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            });
        }
        private async Task<bool> UserExixts (string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}
