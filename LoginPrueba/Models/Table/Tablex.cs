
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;


namespace LoginPrueba.Controllers

{
    public class Tablex
    {

        static bool HasNonNullSearchValue(List<Columns> columns)
        {
            return columns.Any(column => column.search?.value != null);
        }
        [HttpPost("Tables")]
        public JsonResult Tables(
         string busqueda,
         List<search> search = null,
         List<Columns> columns = null,
         List<Order> order = null,
         int start = 0,
         int length = 10,
         List<object> resultado = null)
        {
            try
            {
                var totales = resultado.Count;
                var tblempty = true;

                if (columns != null && HasNonNullSearchValue(columns))
                {
                    foreach (var col in columns)
                    {
                        if (col.search.value != null)
                        {
                            var partes = col.search.value.Contains(",")
                                ? col.search.value.Split(',')
                                : new string[] { col.search.value };

                            resultado = resultado
                                .Where(e => partes.Contains(
                                    e.GetType().GetProperty(col.name).GetValue(e)?.ToString()
                                ))
                                .ToList();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(busqueda))
                {
                    foreach (var col in columns)
                    {
                        List<object> tb = new List<object>();



                        if (col.name!="Operaciones"){
                             tb = resultado.Where(e =>
                                   e.GetType().GetProperty(col.name)
                                    .GetValue(e)?
                                    .ToString()
                                    .IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0
                               ).ToList();
                        }
                       

                        if (tb != null && tb.Count > 0)
                        {
                            tblempty = false;
                            resultado = tb;
                            break;
                        }
                    }

                    if (tblempty) {
                        
                        return new JsonResult(new
                        {
                            data = new List<object>(),
                            recordsFiltered = 0,
                            recordsTotal = 0
                        });
                    }

                }
                if (resultado != null)
                {
                    if (order == null)
                    {
                        return new JsonResult(new
                        {
                            order,
                            data = resultado.Skip(start).Take(length),
                            recordsFiltered = resultado.Count,
                            recordsTotal = totales
                        });
                    }

                    resultado = (order[0].dir == "desc")
                        ? resultado.OrderByDescending(e => e.GetType().GetProperty(order[0].column_name).GetValue(e)).ToList()
                        : resultado.OrderBy(e => e.GetType().GetProperty(order[0].column_name).GetValue(e)).ToList();

                 
                }
                return new JsonResult(new
                {
                   // draw = "1" ,
                    order,
                    data = resultado.Skip(start).Take(length),
                    recordsFiltered = resultado.Count,
                    recordsTotal = totales
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al consultar la información", detail = ex.Message });
            }
        }
    }
}

