namespace Entities.DBEntities;

public partial class AusenciasMensuale
{
    public int IdAusencia { get; set; }

    public int IdProfesor { get; set; }

    public DateOnly FechaAusencia { get; set; }

    public int LeccionesJustificadas { get; set; }

    public int LeccionesInjustificadas { get; set; }

    public DateOnly FechaRegistroAplicacion { get; set; }

    public virtual Profesore IdProfesorNavigation { get; set; } = null!;
}
