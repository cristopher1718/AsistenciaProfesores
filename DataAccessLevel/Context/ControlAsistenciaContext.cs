using System;
using System.Collections.Generic;
using Entities.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLevel.Context;

public partial class ControlAsistenciaContext : DbContext
{
    public ControlAsistenciaContext(){}

    public ControlAsistenciaContext(DbContextOptions<ControlAsistenciaContext> options):base(options){}

    public virtual DbSet<AsistenciaDiarium> AsistenciaDiaria { get; set; }

    public virtual DbSet<Aula> Aulas { get; set; }

    public virtual DbSet<AusenciasMensuale> AusenciasMensuales { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<HistorialCodigo> HistorialCodigos { get; set; }

    public virtual DbSet<HorariosEntradum> HorariosEntrada { get; set; }

    public virtual DbSet<Profesore> Profesores { get; set; }

    public virtual DbSet<UbicacionProfesore> UbicacionProfesores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AsistenciaDiarium>(entity =>
        {
            entity.HasKey(e => e.IdAsistencia).HasName("PK__Asistenc__3956DEE67AE76C56");

            entity.Property(e => e.TipoTardia)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProfesorNavigation).WithMany(p => p.AsistenciaDiaria)
                .HasForeignKey(d => d.IdProfesor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsistenciaDiaria_Profesores");
        });

        modelBuilder.Entity<Aula>(entity =>
        {
            entity.HasKey(e => e.IdAula).HasName("PK__Aulas__2C89573C552BE1A4");

            entity.Property(e => e.NumeroAula)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AusenciasMensuale>(entity =>
        {
            entity.HasKey(e => e.IdAusencia).HasName("PK__Ausencia__0B5BA414DE1FE8E1");

            entity.HasOne(d => d.IdProfesorNavigation).WithMany(p => p.AusenciasMensuales)
                .HasForeignKey(d => d.IdProfesor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusenciasMensuales_Profesores");
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.IdEspecialidad).HasName("PK__Especial__693FA0AFF94A0A13");

            entity.Property(e => e.NombreEspecialidad)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistorialCodigo>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__Historia__9CC7DBB4188925A6");

            entity.Property(e => e.CodigoInstitucional)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IDProfesorNavigation).WithMany(p => p.HistorialCodigos)
                .HasForeignKey(d => d.IDProfesor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialCodigos_Profesores");
        });

        modelBuilder.Entity<HorariosEntradum>(entity =>
        {
            entity.HasKey(e => e.IdHorario).HasName("PK__Horarios__1539229BE4A40D8C");

            entity.HasIndex(e => e.IdProfesor, "UQ__Horarios__C377C3A0DE00C58B").IsUnique();

            entity.HasOne(d => d.IdProfesorNavigation).WithOne(p => p.HorariosEntradum)
                .HasForeignKey<HorariosEntradum>(d => d.IdProfesor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HorariosEntrada_Profesores");
        });

        modelBuilder.Entity<Profesore>(entity =>
        {
            entity.HasKey(e => e.IdProfesor).HasName("PK__Profesor__C377C3A1AE50D019");

            entity.HasIndex(e => e.Cedula, "UQ__Profesor__B4ADFE38464154A6").IsUnique();

            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Cedula)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEspecialidadNavigation).WithMany(p => p.Profesores)
                .HasForeignKey(d => d.IdEspecialidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Profesores_Especialidades");
        });

        modelBuilder.Entity<UbicacionProfesore>(entity =>
        {
            entity.HasKey(e => e.IdUbicacion).HasName("PK__Ubicacio__778CAB1D6897E906");

            entity.HasOne(d => d.IdAulaNavigation).WithMany(p => p.UbicacionProfesores)
                .HasForeignKey(d => d.IdAula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ubicacion_Aulas");

            entity.HasOne(d => d.IdProfesorNavigation).WithMany(p => p.UbicacionProfesores)
                .HasForeignKey(d => d.IdProfesor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ubicacion_Profesores");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__5B65BF972E631FFD");

            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuarios__6B0F5AE00E517B92").IsUnique();

            entity.Property(e => e.Contrasenna)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
