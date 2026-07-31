using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Domain.Common
{
    /// <summary>
    /// Базовый класс всех сущеностей
    /// содержит общие поля, одинаковые для всех
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
