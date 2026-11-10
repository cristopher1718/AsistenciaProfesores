namespace Entities.DBEntities;
public partial class HistorialCodigo
{
    public int IdHistorial { get; set; }

    public int IDProfesor { get; set; }

    public string CodigoInstitucional { get; set; } = null!;

    public DateOnly FechaAsignacion { get; set; }

    public DateOnly? FechaRetiro { get; set; }

    public virtual Profesore IDProfesorNavigation { get; set; } = null!;
}
