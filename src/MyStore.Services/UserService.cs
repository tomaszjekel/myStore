using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using MyStore.Services.DTO;
using Microsoft.AspNetCore.Authentication;

namespace MyStore.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;


        public UserService(IUserRepository userRepository,
            IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> GetAsync(string email)
            => _mapper.Map<UserDto>(await _userRepository.GetAsync(email));

        public async Task RegisterAsync(string email, string password, string role)
        {
            var user = await _userRepository.GetAsync(email);
            if (user != null)
            {
                throw new Exception($"Email: {email} already in use.");
            }
            user = new User(email, "", "");
            var passwordHash = _passwordHasher.HashPassword(user, password);
            user.SetPassword(passwordHash);
            await _userRepository.CreateAsync(user);
        }

        public async Task<string> Confirmation(string userId, string confirmationId)
        {
            return await _userRepository.Confirmation(userId, confirmationId);
        }
        public async Task<UserDto> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetAsync(email);
            if (user == null)
            {
                throw new Exception("Invalid credentials.");
            }
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user,
                user.Password, password);
            if (passwordVerificationResult != PasswordVerificationResult.Failed)
            {
                return _mapper.Map<UserDto>(user);
            }
            throw new Exception("Invalid credentials.");
        }
    }
}