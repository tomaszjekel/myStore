using System;

namespace MyStore.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; set; }
        public string Role { get; private set; }
        public string EmailConfirmation { get; set; }
        public string PasswordReset{ get; set; }
        private User()
        {
        }

        public User(string email)
        {
            Id = Guid.NewGuid();
            Email = email.ToLowerInvariant();
            EmailConfirmation = Guid.NewGuid().ToString();
        }

        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Empty password.");
            }
            Password = password;
        }
    }
}