using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Models;

public class Event
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Дата обязательна")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Место обязательно")]
    [Display(Name = "Место")]
    public string Place { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Количество мест должно быть больше 0")]
    [Display(Name = "Всего мест")]
    public int Capacity { get; set; }

    // Связь: у одного мероприятия может быть много регистраций
    public List<Registration>? Registrations { get; set; }
}