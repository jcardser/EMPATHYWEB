using System.ComponentModel.DataAnnotations;

namespace EmpathyWeb.Models
{
    public class StateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Departamento")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }
        public int CountryId { get; set; }
    }
}
