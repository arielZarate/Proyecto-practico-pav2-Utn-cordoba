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
using Datos;

namespace paginaWeb_pav2
{
    public class DepartamentosController : ApiController
    {
        private PymesEntities db = new PymesEntities();

         //GET api/Departamentos
        public IQueryable<Departamento> GetDepartamento()
        {
            return db.Departamento;
        }



       

           // GET api/Departamentos/5
        [ResponseType(typeof(Departamento))]
        public IHttpActionResult GetDepartamento(int id)
        {
          
            Departamento departamento = db.Departamento.Find(id);

            //Departamento departamento = Datos.GestorDepartamentos.BuscarPorId(id);

            if (departamento == null)
            {
                return NotFound();
            }

            return Ok(departamento);
        }



        //***********************





        //GET: api/Articulos
        //public IHttpActionResult GetDepartamento(string Nombre = "", bool? Activo = null, int numeroPagina = 1)
        //{
        //    //ref webapi parametros;
        //    //ref webapi tipos de retorno de los metodos; cambiamos la devolucion generica del metodo: IQueryable<Articulos> por IHttpActionResult para poder devolver tambien RegistrosTotal
        //    int RegistrosTotal;
        //    //ref c#  var
        //    var Lista = Datos.GestorDepartamentos.Buscar(Nombre, Activo, numeroPagina, out RegistrosTotal);
        //    return Ok(new { Lista = Lista, RegistrosTotal = RegistrosTotal });
        //}












        // PUT api/Departamentos/5
        public IHttpActionResult PutDepartamento(int id, Departamento departamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != departamento.id_dpto)
            {
                return BadRequest();
            }

            //db.Entry(departamento).State = EntityState.Modified;

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!DepartamentoExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            Datos.GestorDepartamentos.Grabar(departamento);

            return StatusCode(HttpStatusCode.NoContent);
        }


    
   

        //*************************************************
        // POST api/Departamentos
        [ResponseType(typeof(Departamento))]
        public IHttpActionResult PostDepartamento(Departamento departamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Datos.GestorDepartamentos.Grabar(departamento);
            db.Departamento.Add(departamento);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = departamento.id_dpto }, departamento);
        }



            //******************************************************************





        // DELETE api/Departamentos/5
        [ResponseType(typeof(Departamento))]
        public IHttpActionResult DeleteDepartamento(int id)
        {
            Departamento departamento = db.Departamento.Find(id);
            if (departamento == null)
            {
                return NotFound();
            }

            db.Departamento.Remove(departamento);
            db.SaveChanges();

            return Ok(departamento);
            //Datos.GestorDepartamentos.DeleteDpto(id);
            //return StatusCode(HttpStatusCode.NoContent);

        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DepartamentoExists(int id)
        {
            return db.Departamento.Count(e => e.id_dpto == id) > 0;
        }
    }
}







   
