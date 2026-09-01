using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    public interface IValidationService
    {
        Task ValidateAsync<T>(T model);
    }
}
