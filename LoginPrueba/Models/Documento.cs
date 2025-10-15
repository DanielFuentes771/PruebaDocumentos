using System;
using System.Collections.Generic;

namespace LoginPrueba.Models;

public partial class Documento
{
    public int Id { get; set; }

    public string nombre { get; set; } = null!;

    public string contrato { get; set; } = null!;

    public decimal saldo { get; set; }

    public DateOnly fecha { get; set; }

    public string telefono { get; set; } = null!;

    public int? usuarioId { get; set; }

    public DateTime? fechaCreacion { get; set; }

    public virtual Usuarios? Usuario { get; set; }
}
