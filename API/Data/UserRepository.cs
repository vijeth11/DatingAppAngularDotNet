﻿using API.Dto;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper) 
        { 
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            // this reduces select query to bring only the data which are sent by
            // API of type MemberDto instead of query for all unwanted properties of 
            // AppUser then map only required fields to MemberDto thus optimizing select query
            // generated by below linq
            // Note: If there is any property in DTO which depends on some calculation done via 
            // adding get<Propertyname> method in AppUser then Automapper will require complete 
            // AppUser data from Db due to which there will be no optimization in query even
            // after modifing as below. So inorder to overcome this we need to add Customization of 
            // mapping certain members of DTO in automapper configuration itself as done for age property
            // of the DTO
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;

        }
    }
}
