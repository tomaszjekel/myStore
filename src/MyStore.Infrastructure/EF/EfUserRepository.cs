﻿using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Domain.Repositories;

namespace MyStore.Infrastructure.EF
{
    public class EfUserRepository : IUserRepository
    {
        private readonly MyStoreContext _context;

        public EfUserRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<User> GetAsync(Guid id)
            => await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

        public async Task<User> GetAsync(string email)
        {
           var user= await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null){
                return user; 
            }
            if (user.EmailConfirmation != "OK")
            {
                throw new Exception($"User: {user.Email} is not activate");
            }
            return user;
        }
 

        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userDb = _context.Users.Where(x => x.Id == user.Id).FirstOrDefault();

            var TO_MAIL = user.Email;
            var MAIL_MSG = string.Format("Activate your account http://localhost:5000/account/confirmation?confirmationId={0}", userDb.EmailConfirmation);


            var url = string.Format("http://siedzetu.pl/mail.php?TO_MAIL={0}&MESSAGE={1}", TO_MAIL, MAIL_MSG);
            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString(url);
                Console.WriteLine(response);
            }
            return ;
        }
       
        public async Task<string> Confirmation(string userId, string confirmationId)
        {
            var user = _context.Users.Where(x=>x.EmailConfirmation ==confirmationId).FirstOrDefault();
            
            if (user!=null)
            {
                _context.Users.Where(x => x.Id == user.Id).FirstOrDefault().EmailConfirmation = "OK";
                _context.SaveChanges();
                return $"User {user.Email} is activate";
            }
            return "User not exist";
            }
        }
    }