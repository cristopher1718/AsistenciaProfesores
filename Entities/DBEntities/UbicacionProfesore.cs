namespace Entities.DBEntities;
public partial class UbicacionProfesore
{
    public int IdUbicacion { get; set; }

    public int IdProfesor { get; set; }

    public int IdAula { get; set; }

    public int DiaSemana { get; set; }

    public bool Leccion_1 { get; set; }

    public bool Leccion_2 { get; set; }

    public bool Leccion_3 { get; set; }

    public bool Leccion_4 { get; set; }

    public bool Leccion_5 { get; set; }

    public bool Leccion_6 { get; set; }

    public bool Leccion_7 { get; set; }

    public bool Leccion_8 { get; set; }

    public bool Leccion_9 { get; set; }

    public bool Leccion_10 { get; set; }

    public bool Leccion_11 { get; set; }

    public bool Leccion_12 { get; set; }

    public virtual Aula IdAulaNavigation { get; set; } = null!;

    public virtual Profesore IdProfesorNavigation { get; set; } = null!;
}
