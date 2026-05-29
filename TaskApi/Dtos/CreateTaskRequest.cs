using System.ComponentModel.DataAnnotations;

namespace TaskApi.Dtos;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "El titulo es obligatorio.")]
    [MinLength(1, ErrorMessage = "El titulo no puede estar vacio")]
    [MaxLength(200, ErrorMessage = "El titulo no puede tener mas de 200 caracteres")]
    public string Title { get; set; } = string.Empty;
}
