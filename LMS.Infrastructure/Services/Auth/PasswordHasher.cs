using LMS.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Services.Auth
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12;

        public string Hash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

        public bool Verify(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
