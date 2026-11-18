using Microsoft.AspNetCore.Mvc;
using EventRegistration.Models;
using EventRegistration.Data;

namespace EventRegistration.Controllers;

public class ParticipantsController : Controller
{
    private readonly AppDbContext _context;

    public ParticipantsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Participants (Список всех участников)
    public IActionResult Index()
    {
        var participants = _context.Participants.ToList();
        return View(participants);
    }

    // GET: /Participants/Create (Форма добавления)
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Participants/Create (Сохранение участника)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Participant newParticipant)
    {
        if (ModelState.IsValid)
        {
            _context.Participants.Add(newParticipant);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(newParticipant);
    }
}