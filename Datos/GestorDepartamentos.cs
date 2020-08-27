
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
    public class GestorDepartamentos
    {

        public static IEnumerable<Departamento> Buscar(string Nombre, bool? Activo, int numeroPagina, out int RegistrosTotal)
        {

            //ref Entity Framework

            using (PymesEntities db = new PymesEntities())     //el using asegura el db.dispose() que libera la conexion de la base
            {
                IQueryable<Departamento> consulta = db.Departamento;
                // aplicar filtros
                //ref LinQ
                //Expresiones lambda, metodos de extension
                if (!string.IsNullOrEmpty(Nombre))
                    consulta = consulta.Where(x => x.nombre_dpto.ToUpper().Contains(Nombre.ToUpper()));    // equivale al like '%TextoBuscar%'
                //if (Activo != null)
                //    consulta = consulta.Where(x => x.Activo == Activo);
                RegistrosTotal = consulta.Count();

                // ref EF; consultas paginadas
                int RegistroDesde = (numeroPagina - 1) * 10;
                var Lista = consulta.OrderBy(x => x.nombre_dpto).Skip(RegistroDesde).Take(10).ToList(); // la instruccion sql recien se ejecuta cuando hacemos ToList()
                return Lista;
            }

        }



        public static Departamento BuscarPorId(int Id)
        {
            using (PymesEntities db = new PymesEntities())
            {
                return db.Departamento.Find(Id);
            }
        }



        //************************************************************************************



        public static void Grabar(Departamento dpto)
        {
            //validar campos
            string erroresValidacion = "";
            if (string.IsNullOrEmpty(dpto.nombre_dpto))
                erroresValidacion += "Descripcion de dpto es un dato requerido ";
            //if (dpto.Precio == null || dpto.Precio == 0)
            //    erroresValidacion += "Precio es un dato requerido; ";
            if (!string.IsNullOrEmpty(erroresValidacion))
                throw new Exception(erroresValidacion);

            //grabar registro
            using (PymesEntities db = new PymesEntities())
            {
                try
                {
                    if (dpto.id_dpto > 0)
                    {
                        db.Entry(dpto).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Departamento.Add(dpto);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("UK_Nombre_Dpto"))
                        throw new ApplicationException("Ya existe otro Departamento con ese Nombre");
                    else
                        throw;
                }
            }
        }




        public static void DeleteDpto(int Id)
        {
            using (PymesEntities db = new PymesEntities())
            {
                //ref Entity Framework; ejecutar codigo sql directo
                db.Database.ExecuteSqlCommand("Delete from Departamento where id_dpto = @Id",
                    new SqlParameter("@Id", Id)
                    );
            }
        }



        //public static void ActivarDesactivar(int IdArticulo)
        //{
        //    using (PymesEntities db = new PymesEntities())
        //    {
        //        //ref Entity Framework; ejecutar codigo sql directo
        //        db.Database.ExecuteSqlCommand("Update Departamento set Activo = case when ISNULL(activo,1)=1 then 0 else 1 end  where IdArticulo = @IdArticulo",
        //            new SqlParameter("@IdArticulo", IdArticulo)
        //            );
        //    }
        //}


        public static Departamento ADOBuscarPorId(int id)
        {
            //ref ADO; Recuperar cadena de conexión de web.config
            string CadenaConexion = System.Configuration.ConfigurationManager.ConnectionStrings["Pymes"].ConnectionString;
            //ref ADO; objetos conexion, comando, parameters y datareader
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = CadenaConexion;
            Departamento dpto = null;
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "select * from Departamento d where d.id_dpto = @Id";
                cmd.Parameters.Add(new SqlParameter("@Id", id));
                SqlDataReader dr = cmd.ExecuteReader();
                // con el resultado cargar una entidad
                //ref ADO; generar un objeto entidad
                if (dr.Read())
                {
                    dpto = new Departamento();
                    dpto.id_dpto = (int)dr["Id"];
                    dpto.nombre_dpto = dr["nombre"].ToString();
                    //if (dr["Precio"] != DBNull.Value)
                    //    dpto.Precio = (decimal)dr["Precio"];
                    //if (dr["CodigoDeBarra"] != DBNull.Value)
                    //    dpto.CodigoDeBarra = dr["CodigoDeBarra"].ToString();
                    //if (dr["Id"] != DBNull.Value)
                    //    dpto.IdArticuloFamilia = (int)dr["IdArticuloFamilia"];
                    //if (dr["Stock"] != DBNull.Value)
                    //    dpto.Stock = (int)dr["Stock"];
                    //if (dr["Activo"] != DBNull.Value)
                    //    dpto.Activo = (bool)dr["Activo"];
                    //if (dr["FechaAlta"] != DBNull.Value)
                    //    dpto.FechaAlta = (DateTime)dr["FechaAlta"];
                }
                dr.Close();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
            return dpto;
        }
    }
}

