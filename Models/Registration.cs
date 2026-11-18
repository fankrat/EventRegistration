using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Models;

public class Registration
{
    public int Id { get; set; }

    // Связь с мероприятием
    [Required]
    [Display(Name = "Мероприятие")]
    public int EventId { get; set; }
    public Event? Event { get; set; }

    // Связь с участником
    [Required]
    [Display(Name = "Участник")]
    public int ParticipantId { get; set; }
    public Participant? Participant { get; set; }

    // Статус регистрации
    [Display(Name = "Статус")]
    public string Status { get; set; } = "Активна"; // Будет "Активна" или "Отменена"

    [Display(Name = "Дата регистрации")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}