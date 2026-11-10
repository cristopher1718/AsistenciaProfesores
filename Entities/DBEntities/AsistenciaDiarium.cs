namespace Entities.DBEntities;

public partial class AsistenciaDiarium
{
    public int IdAsistencia { get; set; }

    public int IdProfesor { get; set; }

    public DateOnly Fecha { get; set; }

    public TimeOnly HoraEntrada { get; set; }

    public TimeOnly HoraSalida { get; set; }

    public string TipoTardia { get; set; } = null!;

    public virtual Profesore IdProfesorNavigation { get; set; } = null!;
}
