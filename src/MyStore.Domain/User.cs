﻿using System;

namespace MyStore.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        private User()
        {
        }

        public User(string email, string role = "user")
        {
            Id = Guid.NewGuid();
            Email = email.ToLowerInvariant();
            Role = role.ToLowerInvariant();
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