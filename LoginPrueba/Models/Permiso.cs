using System;
using System.Collections.Generic;

namespace LoginPrueba.Models;

public partial class Permiso
{
    public int Id { get; set; }

    public string NombreModulo { get; set; } = null!;

    public virtual ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = new List<UsuarioPermiso>();
}
