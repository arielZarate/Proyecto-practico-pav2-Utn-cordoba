//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Datos
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public partial class Departamento
    {
        public Departamento()
        {
            this.Empleado = new HashSet<Empleado>();
        }
    
        public int id_dpto { get; set; }
        public string nombre_dpto { get; set; }



       [JsonIgnore]
        //le saque el virtual
        public ICollection<Empleado> Empleado { get; set; }
    }
}
