using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    public interface ITokenHasher
    {
        string Hash(string token);
    }
}