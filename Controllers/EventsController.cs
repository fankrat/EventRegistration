using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventRegistration.Models;
using EventRegistration.Data;
using EventRegistration.Services;

namespace EventRegistration.Controllers;

public class EventsController : Controller
{
    private readonly AppDbContext _context;
    private readonly RegistrationService _registrationService;

    // Внедряем и базу данных, и наш новый Сервис
    public EventsController(AppDbContext context, RegistrationService registrationService)
    {
        _context = context;
        _registrationService = registrationService;
    }

    // GET: /Events (с поиском)
    public IActionResult Index(string searchString)
    {
        if (!_context.Events.Any())
        {
            _context.Events.AddRange(
                new Event { Title = "Тестовое мероприятие", Date = new DateTime(2026, 12, 1), Place = "Актовый зал", Capacity = 5 },
                new Event { Title = "Лекция по C#", Date = new DateTime(2026, 12, 10), Place = "Аудитория 305", Capacity = 20 },
                new Event { Title = "Спортивный турнир", Date = new DateTime(2026, 12, 15), Place = "Спортзал", Capacity = 10 }
            );
            _context.SaveChanges();
        }

        // Берем все мероприятия
        var events = _context.Events.AsQueryable();

        // Если ввели текст в поиск, фильтруем по названию
        if (!string.IsNullOrEmpty(searchString))
        {
            events = events.Where(e => e.Title.Contains(searchString));
        }

        // Передаем в представление найденный текст, чтобы он не пропадал из строки поиска
        ViewData["CurrentFilter"] = searchString;

        return View(events.ToList());
    }

    // GET: /Events/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Events/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Event newEvent)
    {
        if (ModelState.IsValid)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(newEvent);
    }

    // GET: /Events/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null) return NotFound();

        var ev = _context.Events
            .Include(e => e.Registrations!)
            .ThenInclude(r => r.Participant)
            .FirstOrDefault(m => m.Id == id);
            
        if (ev == null) return NotFound();

        return View(ev);
    }

    // GET: /Events/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();

        var ev = _context.Events.Find(id);
        if (ev == null) return NotFound();

        return View(ev);
    }

    // POST: /Events/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Event newEvent)
    {
        if (id != newEvent.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Events.Update(newEvent);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(newEvent);
    }

    // GET: /Events/Register/5
    public IActionResult Register(int? id)
    {
        if (id == null) return NotFound();

        var ev = _context.Events.FirstOrDefault(m => m.Id == id);
        if (ev == null) return NotFound();

        ViewBag.Participants = _context.Participants.ToList();
        return View(ev);
    }

    // POST: /Events/Register/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(int Id, int ParticipantId)
    {
        // Вся логика теперь в Сервисе! Контроллер просто вызывает его.
        var result = _registrationService.Register(Id, ParticipantId);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            var ev = _context.Events.FirstOrDefault(m => m.Id == Id);
            ViewBag.Participants = _context.Participants.ToList();
            return View(ev);
        }

        TempData["Success"] = result.ErrorMessage; // Тут лежит текст успеха
        return RedirectToAction(nameof(Details), new { id = Id });
    }

    // POST: /Events/CancelRegistration/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelRegistration(int id) // id - это Id регистрации
    {
        // Опять же, вся работа с базой и статусами скрыта в Сервисе
        var success = _registrationService.Cancel(id);
        
        if (!success) return NotFound();

        TempData["Success"] = "Регистрация успешно отменена. Место освобождено.";
        
        // Нам нужно узнать ID мероприятия, чтобы вернуться на его страницу.
        // Достаём его из базы (это быстро).
        var registration = _context.Registrations.Find(id);
        
        return RedirectToAction(nameof(Details), new { id = registration.EventId });
    }
}