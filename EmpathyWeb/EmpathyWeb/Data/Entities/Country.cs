using System.ComponentModel.DataAnnotations;

namespace EmpathyWeb.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name ="País")]
        [MaxLength(50, ErrorMessage ="El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage ="El campo {0} es obligatorio")]
        public string Name { get; set; }

        //Un país tiene muchos estados
        public ICollection<State> States { get; set;}

        //Propiedad de lectura
        [Display(Name="Departamentos")]
        public int StatesNumber => States == null ? 0 : States.Count;
    }
}
