using Microsoft.AspNetCore.Mvc;
using EventRegistration.Models;

namespace EventRegistration.Controllers;

public class EventsController : Controller
{
    // Учебные данные (пока в памяти)
    private static readonly List<Event> _events = new()
    {
        new Event { Id = 1, Title = "Тестовое мероприятие", Date = new DateTime(2026, 12, 1), Place = "Актовый зал", Capacity = 5 },
        new Event { Id = 2, Title = "Лекция по C#", Date = new DateTime(2026, 12, 10), Place = "Аудитория 305", Capacity = 20 },
        new Event { Id = 3, Title = "Спортивный турнир", Date = new DateTime(2026, 12, 15), Place = "Спортзал", Capacity = 10 }
    };

    // GET: /Events
    public IActionResult Index()
    {
        return View(_events);
    }
}