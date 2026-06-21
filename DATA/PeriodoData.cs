using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace Presupuesto.DATA
{
    public class PeriodoData
    {

        


        public static DataTable ObtenerListaPeriodos()
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;
            DataTable db = new DataTable();

            string sSQL = @"SELECT PeriodoId, Anio, Mes, Ingreso 
                    FROM PERIODO
                    ORDER BY Anio DESC, Mes DESC";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cn.Open();
                        da.Fill(db);
                    }
                }
            }


            return db;
        }

        public static DataTable ObtenerMesyAnio(int mes, int anio)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            DataTable db = new DataTable();

            string sSQL = @"SELECT * FROM PERIODO WHERE Mes = @mes AND Anio = @anio";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@anio", anio);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cn.Open();
                        da.Fill(db);
                    }
                }
            }

            return db;
        }

        public static void InsertarPeriodo(int mes, int anio, decimal ingreso,decimal ahorro)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            string sSQL = @"INSERT INTO PERIODO (Anio, Mes, Ingreso,Ahorro) 
                    VALUES (@anio, @mes, @ingreso,@ahorro)";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@anio", anio);
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@ingreso", ingreso);
                    cmd.Parameters.AddWithValue("@ahorro", ahorro);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ActualizarIngreso(int mes, int anio, decimal ingreso)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            string sSQL = @"UPDATE PERIODO 
                    SET Ingreso = @ingreso 
                    WHERE Mes = @mes AND Anio = @anio";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@ingreso", ingreso);
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@anio", anio);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int ObtenerPeriodoId(int mes, int anio)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            string sSQL = @"SELECT PeriodoId 
                    FROM PERIODO 
                    WHERE Mes = @mes AND Anio = @anio";

            using (SqlConnection cn = new SqlConnection(conexion))
            using (SqlCommand cmd = new SqlCommand(sSQL, cn))
            {
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@anio", anio);

                cn.Open();

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new Exception($"No existe periodo para Mes={mes}, Año={anio} para cuando se busca solo para obtener id");

                return Convert.ToInt32(result);
            }
        }




    }
}
