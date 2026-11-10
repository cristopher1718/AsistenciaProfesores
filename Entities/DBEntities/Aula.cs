namespace Entities.DBEntities;

public partial class Aula
{
    public int IdAula { get; set; }

    public string NumeroAula { get; set; } = null!;

    public virtual ICollection<UbicacionProfesore> UbicacionProfesores { get; set; } = new List<UbicacionProfesore>();
}
