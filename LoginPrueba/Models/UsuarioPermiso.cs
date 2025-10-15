using System;
using System.Collections.Generic;

namespace LoginPrueba.Models;

public partial class UsuarioPermiso
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public int? PermisoId { get; set; }

    public virtual Permiso? Permiso { get; set; }

    public virtual Usuarios? Usuario { get; set; }
}
