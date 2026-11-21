using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace EventFlow.Application.Commands.EventCommands
{
    public class UploadEventImageCommandHandle(
        IEventRepository eventRepository,
        IOrganizerRepository organizerRepository,
        IWebHostEnvironment webHostEnvironment,
        ILogger<UploadEventImageCommandHandle> logger)
        : IRequestHandler<UploadEventImageCommand, Result>
    {
        private readonly string _uploadFolder = "images/events";

        public async Task<Result> Handle(UploadEventImageCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Загрузка изображения для мероприятия {EventId}", request.EventId);

            // Проверяем, что файл передан
            if (request.Image == null || request.Image.Length == 0)
                return Result.Failure("Файл изображения не указан", 400);

            // Ограничиваем размер файла (10 MB)
            const long maxFileSize = 10 * 1024 * 1024;
            if (request.Image.Length > maxFileSize)
                return Result.Failure("Размер изображения не должен превышать 10 МБ", 400);

            // Проверяем тип файла
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(request.Image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return Result.Failure("Допустимые форматы: jpg, jpeg, png, gif, webp", 400);

            // Проверяем, что пользователь — организатор этого мероприятия
            var organizer = await organizerRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (organizer == null)
                return Result.Failure("Пользователь не является организатором", 401);

            var eventEntity = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (eventEntity == null)
                return Result.Failure("Мероприятие не найдено", 404);

            if (eventEntity.OrganizerId != organizer.Id)
                return Result.Failure("У вас нет прав для изменения этого мероприятия", 403);

            // Сохраняем файл
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, _uploadFolder, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);

            var imageUrl = $"/{_uploadFolder}/{fileName}";

            // Обновляем ImageUrl в базе
            eventEntity.ImageUrl = imageUrl;
            await eventRepository.UpdateAsync(eventEntity, cancellationToken);
            await eventRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Изображение успешно загружено для мероприятия {EventId}: {ImageUrl}", request.EventId, imageUrl);
            return Result.Success(message: "Изображение успешно загружено");
        }
    }
}
