using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using SendGrid;
using SendGrid.Helpers.Mail;

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
            string subject = "Roksa2 - Activate your account";
            string email = user.Email;

            var userDb = _context.Users.Where(x => x.Email == user.Email).FirstOrDefault();
            userDb.EmailConfirmation = Guid.NewGuid().ToString();
            _context.SaveChanges();
            var htmlContext = string.Format("Activate your account http://roksa2.pl/account/confirmation?confirmationId={0}", userDb.EmailConfirmation);
            SendMail(subject, email,"",htmlContext).Wait();
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

        public async Task ResetPassword(User user, string password)
        {
            
            _context.Users.Where(x => x.Email == user.Email).FirstOrDefault().PasswordReset=password;
            _context.SaveChanges();

            string subject = "Roksa2 - Reset password";
            string email = user.Email;

            
            var htmlContext = string.Format("Reset your password http://roksa2.pl/account/reset_password?guid={0}", password);
            SendMail(subject, email, "", htmlContext).Wait();

        }
        public async Task<User> GetUserByResetPassword(string guid)
        {
            User user = _context.Users.Where(x => x.PasswordReset == guid).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            return null;
        }
        public async Task UpdatePassword(string email,string password)
        {
            _context.Users.Where(x => x.Email == email).FirstOrDefault().Password = password;
            _context.Users.Where(x => x.Email == email).FirstOrDefault().PasswordReset = Guid.NewGuid().ToString();
           _context.SaveChanges();
        }

        static async Task SendMail(string subject, string emailTo, string plainTextContent, string htmlContent)
        {
            var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("admin@roxa2.pl", "Roksa2");
            var to = new EmailAddress(emailTo, "Roksa2");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
    }