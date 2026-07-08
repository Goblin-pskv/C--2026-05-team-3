using EventFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces
{
    /// <summary>
    /// Контракт для работы с пользователями в базе данных.
    /// 
    /// Определяет операции для регистрации, авторизации и управления
    /// профилями пользователей. Реализация — UserRepository в Infrastructure.
    /// 
    /// Особенности:
    /// - Поиск по Email (уникальный идентификатор для входа)
    /// - Проверка существования пользователя
    /// - Работа с хэшами паролей (никогда не храним пароли в открытом виде)
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Получить пользователя по уникальному идентификатору.
        /// </summary>
        /// <param name="id">UUID пользователя</param>
        /// <returns>Пользователь или null, если не найден</returns>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить пользователя по Email.
        /// Email — уникальный идентификатор для входа в систему.
        /// Используется при аутентификации.
        /// </summary>
        /// <param name="email">Email пользователя (в нижнем регистре)</param>
        /// <returns>Пользователь или null, если не найден</returns>
        /// <example>
        /// var user = await _repository.GetByEmailAsync("ivan@example.com");
        /// if (user == null) throw new DomainException("Пользователь не найден");
        /// </example>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Проверить, существует ли пользователь с указанным Email.
        /// Используется при регистрации, чтобы избежать дубликатов.
        /// </summary>
        /// <param name="email">Email для проверки</param>
        /// <returns>true если пользователь существует</returns>
        Task<bool> ExistsByEmailAsync(string email);

        /// <summary>
        /// Добавить нового пользователя в базу данных.
        /// После вызова нужно вызвать SaveChangesAsync().
        /// </summary>
        /// <param name="user">Сущность пользователя для добавления</param>
        Task AddAsync(User user);

        /// <summary>
        /// Обновить данные пользователя.
        /// Используется для изменения профиля, пароля и т.д.
        /// </summary>
        /// <param name="user">Пользователь с измененными данными</param>
        void Update(User user);

        /// <summary>
        /// Сохранить все изменения в базе данных.
        /// </summary>
        Task SaveChangesAsync();
    }
}
