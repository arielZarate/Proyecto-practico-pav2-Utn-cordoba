using Datos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;


namespace paginaWeb_pav2
{
    public class EmpleadosController : ApiController
    {
        private PymesEntities db = new PymesEntities();

        //GET api/Empleados
        //public IQueryable<Empleado> GetEmpleado()
        //{
        //    return db.Empleado;
        //}



        //ref webapi; por defecto se busca el metodo de request (get, post, etc) segun comienze el nombre de la accion y sus parametros 
        //GET: api/Articulos
        public IHttpActionResult GetEmpleados(string Nombre = "", bool? Activo = null, int numeroPagina = 1)
        {
            //ref webapi parametros;
            //ref webapi tipos de retorno de los metodos; cambiamos la devolucion generica del metodo: IQueryable<Articulos> por IHttpActionResult para poder devolver tambien RegistrosTotal
            int RegistrosTotal;
            //ref c#  var
            var Lista = Datos.GestorEmpleados.Buscar(Nombre, Activo, numeroPagina, out RegistrosTotal);
            return Ok(new { Lista = Lista, RegistrosTotal = RegistrosTotal });
        }



        // GET api/Empleado/5

        [ResponseType(typeof(Empleado))]
        public IHttpActionResult GetEmpleados(int id)
        {

            Empleado empleado = Datos.GestorEmpleados.BuscarPorId(id);

            //Departamento departamento = db.Departamento.Find(id);
            if (empleado == null)
            {
                return NotFound();// status 404
            }

            return Ok(empleado);
        }


        // PUT: api/Articulos/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutArticulos(int id, Empleado empleado)
        {
            if (!ModelState.IsValid)  //ref DataAnnotations; validar en el servidor ??
            {
                return BadRequest(ModelState);
            }

            if (id != empleado.id_emple)
            {
                return BadRequest();
            }

            Datos.GestorEmpleados.Grabar(empleado);

            return StatusCode(HttpStatusCode.NoContent);
        }



        // POST: api/Articulos
        [ResponseType(typeof(Empleado))]
        public IHttpActionResult PostArticulos(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Datos.GestorEmpleados.Grabar(empleado);

            return CreatedAtRoute("DefaultApi", new { id = empleado.id_emple }, empleado);
        }



        [ResponseType(typeof(Empleado))]
        public IHttpActionResult DeleteEmpleado(int id)
        {
            //new ApplicationException("error en base");   //ref??? throw no genera error dentro de webapi, continua normalmente?

            //ref EntityFramework baja logica
            Datos.GestorEmpleados.ActivarDesactivar(id);
            return StatusCode(HttpStatusCode.NoContent);
        }


        //ojo con este metodo 


        //[ResponseType(typeof(Departamento))]
        //public IHttpActionResult DeleteEmpleado(int id)
        //{
        //Empleado empleado = db.Empleado.Find(id);
        //    if (empleado == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Empleado.Remove(empleado);
        //    db.SaveChanges();

        //    return Ok(empleado);
           

        //}








        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //private bool EmpleadoExists(int id)
        //{
        //    return db.Empleado.Count(e => e.id_emple == id) > 0;
        //}
    }
}