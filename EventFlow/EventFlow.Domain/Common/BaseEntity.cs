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
        public BaseEntity(Guid id, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
        }

        public Guid Id { get; init; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public void SetTimestamps(DateTime date)
        {
            CreatedAt = date;
            UpdatedAt = date;
        }
        public void MarkAsUpdated(DateTime date)
        {
            UpdatedAt = date;
        }
    }
}
