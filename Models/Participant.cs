using System.ComponentModel.DataAnnotations;

namespace EventRegistration.Models;

public class Participant
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ФИО обязательно")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон или Email обязателен")]
    [Display(Name = "Телефон/Email")]
    public string ContactInfo { get; set; } = string.Empty;
}