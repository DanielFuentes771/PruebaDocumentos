using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LoginPrueba.Models;

public partial class LoginPruebasContext : DbContext
{
    public LoginPruebasContext()
    {
    }

    public LoginPruebasContext(DbContextOptions<LoginPruebasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConfiguracionImpresion> ConfiguracionImpresions { get; set; }

    public virtual DbSet<Documento> Documentos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<UsuarioPermiso> UsuarioPermisos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DANIEL_FUENTES;Database=LoginPruebas;User Id=sa;Password=PruebaLaboral2025;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfiguracionImpresion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Configur__3214EC07D9833156");

            entity.ToTable("ConfiguracionImpresion");

            entity.Property(e => e.TamañoHoja)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TipoLetra).HasMaxLength(50);

            entity.HasOne(d => d.Usuario).WithMany(p => p.ConfiguracionImpresions)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Configura__Usuar__412EB0B6");
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3214EC07FBFABB2B");

            entity.Property(e => e.contrato).HasMaxLength(50);
            entity.Property(e => e.fechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.nombre).HasMaxLength(100);
            entity.Property(e => e.saldo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.telefono).HasMaxLength(20);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Documentos)
                .HasForeignKey(d => d.usuarioId)
                .HasConstraintName("FK__Documento__Usuar__440B1D61");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permisos__3214EC074554B0AD");

            entity.Property(e => e.NombreModulo).HasMaxLength(100);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Usuarios__3214EC07D93ED223");

            entity.HasIndex(e => e.usuario, "UQ__Usuarios__E3237CF7041AAFA2").IsUnique();

            entity.Property(e => e.contraseña).HasMaxLength(255);
            entity.Property(e => e.estatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.usuario)
                .HasMaxLength(50)
                .HasColumnName("Usuario");
        });

        modelBuilder.Entity<UsuarioPermiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsuarioP__3214EC0712F4C53B");

            entity.HasOne(d => d.Permiso).WithMany(p => p.UsuarioPermisos)
                .HasForeignKey(d => d.PermisoId)
                .HasConstraintName("FK__UsuarioPe__Permi__3E52440B");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioPermisos)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__UsuarioPe__Usuar__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
