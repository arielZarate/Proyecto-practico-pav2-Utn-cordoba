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
    
    public partial class Empleado
    {
        public int id_emple { get; set; }
        public string nom_emple { get; set; }
        public string ape_emple { get; set; }
        public decimal sueldo { get; set; }
        public System.DateTime fecha_alta { get; set; }
        public int id_departamento { get; set; }
        public Boolean activo { get; set; }

       
        [JsonIgnore]

        //le saque el virtual
        public  Departamento Departamento { get; set; }
    }
}
