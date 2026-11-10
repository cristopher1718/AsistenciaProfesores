namespace Entities.DBEntities;

public partial class Especialidade
{
    public int IdEspecialidad { get; set; }

    public string NombreEspecialidad { get; set; } = null!;

    public virtual ICollection<Profesore> Profesores { get; set; } = new List<Profesore>();
}
