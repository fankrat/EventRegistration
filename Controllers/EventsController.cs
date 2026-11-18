using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventRegistration.Models;
using EventRegistration.Data;

namespace EventRegistration.Controllers;

public class EventsController : Controller
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Events
    public IActionResult Index()
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

        var events = _context.Events.ToList();
        return View(events);
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
        var ev = _context.Events.FirstOrDefault(m => m.Id == Id);
        if (ev == null) return NotFound();

        if (ParticipantId == 0)
        {
            TempData["Error"] = "Пожалуйста, выберите участника.";
            ViewBag.Participants = _context.Participants.ToList();
            return View(ev);
        }

        var activeRegistrationsCount = _context.Registrations
            .Count(r => r.EventId == Id && r.Status == "Активна");
            
        if (activeRegistrationsCount >= ev.Capacity)
        {
            TempData["Error"] = "Свободных мест нет! Регистрация невозможна.";
            ViewBag.Participants = _context.Participants.ToList();
            return View(ev);
        }

        var isAlreadyRegistered = _context.Registrations
            .Any(r => r.EventId == Id && r.ParticipantId == ParticipantId && r.Status == "Активна");
            
        if (isAlreadyRegistered)
        {
            TempData["Error"] = "Этот участник уже зарегистрирован на данное мероприятие.";
            ViewBag.Participants = _context.Participants.ToList();
            return View(ev);
        }

        var newRegistration = new Registration
        {
            EventId = Id,
            ParticipantId = ParticipantId,
            Status = "Активна",
            CreatedAt = DateTime.Now
        };

        _context.Registrations.Add(newRegistration);
        _context.SaveChanges();

        TempData["Success"] = "Участник успешно зарегистрирован!";
        return RedirectToAction(nameof(Details), new { id = Id });
    }

    // POST: /Events/CancelRegistration/5 (ОТМЕНА РЕГИСТРАЦИИ)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelRegistration(int id) // id - это Id самой регистрации
    {
        // Находим регистрацию в базе
        var registration = _context.Registrations
            .Include(r => r.Event)
            .FirstOrDefault(r => r.Id == id);
            
        if (registration == null) return NotFound();

        // Меняем статус на "Отменена". Место автоматически освобождается благодаря нашей логике подсчета!
        registration.Status = "Отменена";
        _context.SaveChanges();

        TempData["Success"] = "Регистрация успешно отменена. Место освобождено.";
        
        // Возвращаемся на страницу подробностей этого мероприятия
        return RedirectToAction(nameof(Details), new { id = registration.EventId });
    }
}