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
                        perio.Ingreso - perio.Ahorro - SUM(g.Monto) AS PresupuestoRestante
                    FROM dbo.GASTO AS g
                    INNER JOIN dbo.PERIODO AS perio ON g.PeriodoId = perio.PeriodoId
                    WHERE perio.Mes = @mes AND perio.Anio = @anio
                    GROUP BY perio.Ingreso, perio.Ahorro
                )
                SELECT
                    g.GastoId,
                    cat.CategoriaId,
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


        public static DataTable ObtenerGastos(int mes, int anio, string categoria)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            DataTable db = new DataTable();

            string sSQL = @"
                ;WITH TotalGeneral AS (
                    SELECT 
                        perio.Ingreso - perio.Ahorro - SUM(g.Monto) AS PresupuestoRestante
                    FROM dbo.GASTO AS g
                    INNER JOIN dbo.PERIODO AS perio ON g.PeriodoId = perio.PeriodoId
                    WHERE perio.Mes = @mes AND perio.Anio = @anio
                    GROUP BY perio.Ingreso, perio.Ahorro
                )
                SELECT
                    g.GastoId,
                    cat.CategoriaId,
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
                        AND (@categoria IS NULL OR cat.Nombre = @categoria)
                ORDER BY g.fecha DESC, g.GastoId DESC;";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@anio", anio);
                    cmd.Parameters.AddWithValue("@categoria", string.IsNullOrEmpty(categoria) ? (object)DBNull.Value : categoria);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cn.Open();
                        da.Fill(db);
                    }
                }
            }

            return db;
        }



        public static bool EliminarGasto(string sSQL, Dictionary<string, object> dic) 
        {
            int value = 0;
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@gastoid", dic["gastoid"]);
                    cn.Open();
                    value = cmd.ExecuteNonQuery();
                }
            }
            if (value > 0)
            {
                return true;
            }
            else {
                return false;
            
            }
        }

        public static void ActualizarGasto(int gastoId, int periodoId, int categoriaId, decimal monto, DateTime fecha, string comentario)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            string sSQL = @"UPDATE GASTO 
                     SET PeriodoId = @PeriodoId, 
                         CategoriaId = @CategoriaId, 
                         monto = @monto, 
                         fecha = @fecha, 
                         comentario = @comentario
                     WHERE GastoId = @gastoId";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@PeriodoId", periodoId);
                    cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);
                    cmd.Parameters.AddWithValue("@monto", monto);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@comentario",
                    string.IsNullOrWhiteSpace(comentario) ? (object)DBNull.Value : comentario);
                    cmd.Parameters.AddWithValue("@gastoId", gastoId);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }





    }
}