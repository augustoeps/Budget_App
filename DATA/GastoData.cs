using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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


        public static DataTable ObtenerGastos(int mes, int anio)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            DataTable db = new DataTable();

            string sSQL = @"
                ;WITH TotalGeneral AS (
                    SELECT 
                        perio.Ingreso - SUM(g.Monto) AS PresupuestoRestante
                    FROM dbo.GASTO AS g
                    INNER JOIN dbo.PERIODO AS perio ON g.PeriodoId = perio.PeriodoId
                    WHERE perio.Mes = @mes AND perio.Anio = @anio
                    GROUP BY perio.Ingreso
                )
                SELECT
                    g.GastoId,
                    cat.Nombre,
                    g.Monto,
                    g.Fecha,
                    g.Comentario,
                    pre.MontoAsignado,
                    SUM(g.Monto) OVER (PARTITION BY cat.Nombre ORDER BY g.Fecha, g.GastoId  
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS totalCategoria,
                    pre.MontoAsignado - SUM(g.Monto) OVER (PARTITION BY cat.Nombre ORDER BY g.Fecha, g.GastoId 
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CategoriaRestante,
                    tg.PresupuestoRestante
                FROM dbo.GASTO AS g
                INNER JOIN dbo.CATEGORIA AS cat ON g.CategoriaId = cat.CategoriaId
                INNER JOIN dbo.PERIODO AS p ON g.PeriodoId = p.PeriodoId
                INNER JOIN dbo.PRESUPUESTO_CATEGORIA AS pre ON pre.PeriodoId = p.PeriodoId 
                    AND pre.CategoriaId = g.CategoriaId
                CROSS JOIN TotalGeneral AS tg
                WHERE p.Mes = @mes AND p.Anio = @anio
                ORDER BY g.fecha DESC, g.GastoId DESC;";

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

    }
}