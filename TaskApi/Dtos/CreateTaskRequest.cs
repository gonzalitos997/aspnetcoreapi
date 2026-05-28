using System.ComponentModel.DataAnnotations;

namespace TaskApi.Dtos;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "El titulo es obligatorio.")]
    [StringLength(
        100,
        MinimumLength = 3,
        ErrorMessage = "El titulo debe tener entre 3 y 100 caracteres."
    )]
    public string Title { get; set; } = string.Empty;
}
