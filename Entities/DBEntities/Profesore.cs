namespace Entities.DBEntities;

public partial class Profesore
{
    public int IdProfesor { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string Apellido2 { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public int IdEspecialidad { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<AsistenciaDiarium> AsistenciaDiaria { get; set; } = new List<AsistenciaDiarium>();

    public virtual ICollection<AusenciasMensuale> AusenciasMensuales { get; set; } = new List<AusenciasMensuale>();

    public virtual ICollection<HistorialCodigo> HistorialCodigos { get; set; } = new List<HistorialCodigo>();

    public virtual HorariosEntradum? HorariosEntradum { get; set; }

    public virtual Especialidade IdEspecialidadNavigation { get; set; } = null!;

    public virtual ICollection<UbicacionProfesore> UbicacionProfesores { get; set; } = new List<UbicacionProfesore>();
}
