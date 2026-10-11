using EventFlow.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EventFlow.Infrastructure.Services
{
    public class Sha256TokenHasher : ITokenHasher
    {
        public string Hash(string token) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}