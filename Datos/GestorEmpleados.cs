
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Datos
{
    public class GestorEmpleados
    {

        public static IEnumerable<Empleado> Buscar(string Nombre, bool? Activo, int numeroPagina, out int RegistrosTotal)
        {

            //ref Entity Framework

            using (PymesEntities db = new PymesEntities())     //el using asegura el db.dispose() que libera la conexion de la base
            {
                IQueryable<Empleado> consulta = db.Empleado;
                // aplicar filtros
                //ref LinQ
                //Expresiones lambda, metodos de extension
                if (!string.IsNullOrEmpty(Nombre))
                    consulta = consulta.Where(x => x.nom_emple.ToUpper().Contains(Nombre.ToUpper()));    // equivale al like '%TextoBuscar%'
                if (Activo != null)
                    consulta = consulta.Where(x => x.activo == Activo);
                RegistrosTotal = consulta.Count();

                // ref EF; consultas paginadas
                int RegistroDesde = (numeroPagina - 1) * 10;
                var Lista = consulta.OrderBy(x => x.nom_emple).Skip(RegistroDesde).Take(10).ToList(); // la instruccion sql recien se ejecuta cuando hacemos ToList()
                return Lista;
            }

        }




        public static Empleado BuscarPorId(int sId)
        {
            using (PymesEntities db = new PymesEntities())
            {
                return db.Empleado.Find(sId);
            }
        }






        public static void Grabar(Empleado emple)
        {
            //validar campos
            string erroresValidacion = "";
            if (string.IsNullOrEmpty(emple.nom_emple))
                erroresValidacion += "Nombre de empleado es un dato requerido ";
            if (string.IsNullOrEmpty(emple.ape_emple))
                erroresValidacion += "Apellido de empleado es un dato requerido ";
            if (emple.sueldo<=0)
                erroresValidacion += "Sueldo es un dato requerido ";
            if (string.IsNullOrEmpty((emple.fecha_alta).ToString()))
                erroresValidacion += "Verifique la fecha ";
                if (string.IsNullOrEmpty(emple.id_departamento.ToString()))
                    erroresValidacion += "Verifique el departamento";
            if (!string.IsNullOrEmpty(erroresValidacion))
                throw new Exception(erroresValidacion);

            //grabar registro
            using (PymesEntities db = new PymesEntities())
            {
                try
                {
                    if (emple.id_emple > 0)
                    {
                        db.Entry(emple).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Empleado.Add(emple);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("UK_Nombre"))
                    
                        throw new ApplicationException("Ya existe otro Trabajador con ese Nombre");
                    
                    else
                        throw;
                }
            }
        }


      

        public static void ActivarDesactivar(int  Id)
        {
            using (PymesEntities db = new PymesEntities())
            {
                //ref Entity Framework; ejecutar codigo sql directo
                db.Database.ExecuteSqlCommand("Update Empleado set activo = case when ISNULL(activo,1)=1 then 0 else 1 end  where id_emple = @IdEmpleado",
                    new SqlParameter("@IdEmpleado", Id)
                    );
            }
        }

      



        //public static Empleado ADOBuscarPorId(int IdArticulo)
        //{
        //    //ref ADO; Recuperar cadena de conexión de web.config
        //    string CadenaConexion = System.Configuration.ConfigurationManager.ConnectionStrings["PymesAdo"].ConnectionString;
        //    //ref ADO; objetos conexion, comando, parameters y datareader
        //    SqlConnection cn = new SqlConnection();
        //    cn.ConnectionString = CadenaConexion;
        //    Empleado gilTrabajador = null;
        //    try
        //    {
        //        cn.Open();
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = cn;
        //        cmd.CommandText = "select * from Articulos c where c.idArticulo = @IdArticulo";
        //        cmd.Parameters.Add(new SqlParameter("@IdArticulo", IdArticulo));
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        // con el resultado cargar una entidad
        //        //ref ADO; generar un objeto entidad
        //        if (dr.Read())
        //        {
        //            gilTrabajador = new Articulos();
        //            gilTrabajador.IdArticulo = (int)dr["IdArticulo"];
        //            gilTrabajador.Nombre = dr["nombre"].ToString();
        //            if (dr["Precio"] != DBNull.Value)
        //                gilTrabajador.Precio = (decimal)dr["Precio"];
        //            if (dr["CodigoDeBarra"] != DBNull.Value)
        //                gilTrabajador.CodigoDeBarra = dr["CodigoDeBarra"].ToString();
        //            if (dr["IdArticuloFamilia"] != DBNull.Value)
        //                gilTrabajador.IdArticuloFamilia = (int)dr["IdArticuloFamilia"];
        //            if (dr["Stock"] != DBNull.Value)
        //                gilTrabajador.Stock = (int)dr["Stock"];
        //            if (dr["Activo"] != DBNull.Value)
        //                gilTrabajador.Activo = (bool)dr["Activo"];
        //            if (dr["FechaAlta"] != DBNull.Value)
        //                gilTrabajador.FechaAlta = (DateTime)dr["FechaAlta"];
        //        }
        //        dr.Close();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally
        //    {
        //        if (cn.State == ConnectionState.Open)
        //            cn.Close();
        //    }
        //    return gilTrabajador;
        //}

   
    }
}

