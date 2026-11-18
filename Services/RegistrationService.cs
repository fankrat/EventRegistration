using EventRegistration.Data;
using EventRegistration.Models;

namespace EventRegistration.Services;

public class RegistrationService
{
    private readonly AppDbContext _context;

    // Внедряем базу данных
    public RegistrationService(AppDbContext context)
    {
        _context = context;
    }

    // Метод регистрации: возвращает результат (успех/ошибка) и сообщение
    public (bool IsSuccess, string ErrorMessage) Register(int eventId, int participantId)
    {
        var ev = _context.Events.Find(eventId);
        if (ev == null) return (false, "Мероприятие не найдено.");

        if (participantId == 0) return (false, "Пожалуйста, выберите участника.");

        // Проверка: есть ли свободные места
        var activeRegistrationsCount = _context.Registrations
            .Count(r => r.EventId == eventId && r.Status == "Активна");
            
        if (activeRegistrationsCount >= ev.Capacity)
            return (false, "Свободных мест нет! Регистрация невозможна.");

        // Проверка: не зарегистрирован ли уже
        var isAlreadyRegistered = _context.Registrations
            .Any(r => r.EventId == eventId && r.ParticipantId == participantId && r.Status == "Активна");
            
        if (isAlreadyRegistered)
            return (false, "Этот участник уже зарегистрирован на данное мероприятие.");

        // Сохранение
        var newRegistration = new Registration
        {
            EventId = eventId,
            ParticipantId = participantId,
            Status = "Активна",
            CreatedAt = DateTime.Now
        };

        _context.Registrations.Add(newRegistration);
        _context.SaveChanges();

        return (true, "Участник успешно зарегистрирован!");
    }

    // Метод отмены регистрации
    public bool Cancel(int registrationId)
    {
        var registration = _context.Registrations.Find(registrationId);
        if (registration == null) return false;

        // Меняем статус, чтобы место освободилось
        registration.Status = "Отменена";
        _context.SaveChanges();
        
        return true;
    }
}