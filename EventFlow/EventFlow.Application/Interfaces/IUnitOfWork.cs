using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Interfaces;

/// <summary>
/// Паттерн "Единица работы" (Unit of Work).
/// 
/// Зачем нужен:
/// 1. Группирует несколько операций в одну транзакцию
/// 2. Гарантирует атомарность: либо все операции успешны, либо ни одна
/// 3. Избегает дублирования SaveChangesAsync() в каждом репозитории
/// 4. Централизованное управление транзакциями
/// 
/// Пример использования:
/// await using var transaction = await _unitOfWork.BeginTransactionAsync();
/// try {
///     await _eventRepository.AddAsync(@event);
///     await _registrationRepository.AddAsync(registration);
///     await _unitOfWork.CommitAsync();
/// } catch {
///     await _unitOfWork.RollbackAsync();
///     throw;
/// }
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Сохранить все изменения во всех репозиториях.
    /// Выполняет COMMIT транзакции.
    /// </summary>
    /// <returns>Количество измененных записей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Начать новую транзакцию.
    /// Используется для группировки нескольких операций.
    /// </summary>
    /// <returns>Объект транзакции для управления</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Зафиксировать транзакцию (COMMIT).
    /// Все изменения сохраняются в БД.
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Откатить транзакцию (ROLLBACK).
    /// Все изменения отменяются.
    /// Вызывается при ошибке.
    /// </summary>
    Task RollbackAsync();
}
