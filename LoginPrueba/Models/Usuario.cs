using System;
using System.Collections.Generic;

namespace LoginPrueba.Models;

public partial class Usuarios
{
    public int id { get; set; }

    public string usuario { get; set; } = null!;

    public string? contraseña { get; set; } = null;

    public string estatus { get; set; } = null!;

    public TimeOnly horainicio { get; set; }

    public TimeOnly horafin { get; set; }

    public virtual ICollection<ConfiguracionImpresion> ConfiguracionImpresions { get; set; } = new List<ConfiguracionImpresion>();

    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    public virtual ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = new List<UsuarioPermiso>();
}
