using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Presupuesto.DATA
{
    public class GastoData
    {

        public static void InsertarGasto(int PeriodoId, int CategoriaId, decimal monto, DateTime fecha, string comentario)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            string sSQL = @"INSERT INTO GASTO (PeriodoId, CategoriaId, monto, fecha, comentario) 
                    VALUES (@PeriodoId, @CategoriaId, @monto, @fecha, @comentario)";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@PeriodoId", PeriodoId);
                    cmd.Parameters.AddWithValue("@CategoriaId", CategoriaId);
                    cmd.Parameters.AddWithValue("@monto", monto);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@comentario",
                        string.IsNullOrWhiteSpace(comentario) ? (object)DBNull.Value : comentario);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}