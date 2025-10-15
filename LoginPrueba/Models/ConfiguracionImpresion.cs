using System;
using System.Collections.Generic;

namespace LoginPrueba.Models;

public partial class ConfiguracionImpresion
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public string TamañoHoja { get; set; } = null!;

    public int TamañoLetra { get; set; }

    public string TipoLetra { get; set; } = null!;

    public byte[]? ImagenFondo { get; set; }

    public virtual Usuarios? Usuario { get; set; }
}
