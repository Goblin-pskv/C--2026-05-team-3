using EventFlow.Application.Interfaces;
using EventFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Repositories;

/// <summary>
/// Реализация паттерна "Единица работы" (Unit of Work).
/// 
/// Зачем нужен:
/// 1. Группирует несколько операций в одну транзакцию
/// 2. Гарантирует атомарность: либо все успешно, либо ничего
/// 3. Централизованное управление транзакциями
/// 4. Избегаем дублирования кода транзакций в сервисах
/// 
/// Пример использования:
/// await using var uow = new UnitOfWork(context);
/// await uow.BeginTransactionAsync();
/// try {
///     await eventRepo.AddAsync(@event);
///     await registrationRepo.AddAsync(registration);
///     await uow.CommitAsync();
/// } catch {
///     await uow.RollbackAsync();
///     throw;
/// }
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly EventFlowDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(EventFlowDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Начать новую транзакцию.
    /// Все последующие операции будут выполняться в рамках этой транзакции.
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Зафиксировать транзакцию (COMMIT).
    /// Все изменения сохраняются в БД.
    /// </summary>
    public async Task CommitAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("Транзакция не была начата");

        try
        {
            await _transaction.CommitAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("Транзакция не была начата");

        try
        {
            await _transaction.RollbackAsync();
        }

        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Сохранить все изменения во всех репозиториях.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

