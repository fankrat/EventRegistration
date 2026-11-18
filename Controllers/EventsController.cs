using Microsoft.AspNetCore.Mvc;
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

        var ev = _context.Events.FirstOrDefault(m => m.Id == id);
        if (ev == null) return NotFound();

        return View(ev);
    }

    // GET: /Events/Edit/5 (показывает форму редактирования)
    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();

        var ev = _context.Events.Find(id);
        if (ev == null) return NotFound();

        return View(ev);
    }

    // POST: /Events/Edit/5 (сохраняет изменения)
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
}