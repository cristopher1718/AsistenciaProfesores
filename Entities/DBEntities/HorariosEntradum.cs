namespace Entities.DBEntities;

public partial class HorariosEntradum
{
    public int IdHorario { get; set; }

    public int IdProfesor { get; set; }

    public TimeOnly? EntradaLunes { get; set; }

    public TimeOnly? EntradaMartes { get; set; }

    public TimeOnly? EntradaMiercoles { get; set; }

    public TimeOnly? EntradaJueves { get; set; }

    public TimeOnly? EntradaViernes { get; set; }

    public virtual Profesore IdProfesorNavigation { get; set; } = null!;
}
