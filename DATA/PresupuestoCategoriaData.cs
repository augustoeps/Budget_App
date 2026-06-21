using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Presupuesto.DATA
{
    public class PresupuestoCategoriaData
    {
        public static DataTable ObtenerCategoriasPorMes(int periodo)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            DataTable db = new DataTable();

            string sSQL = @"SELECT c.CategoriaId, c.Nombre, 
                       c.Icono, 
                       COALESCE(pc.MontoAsignado, 0) as MontoAsignado,
                       COALESCE(pc.MontoAsignado, 0) - COALESCE(SUM(g.Monto), 0) as Sobrante
                FROM CATEGORIA c
                LEFT JOIN PRESUPUESTO_CATEGORIA pc ON c.CategoriaId = pc.CategoriaId AND pc.PeriodoId = @PeriodoId
                LEFT JOIN GASTO g ON g.CategoriaId = pc.CategoriaId AND g.PeriodoId = pc.PeriodoId
                WHERE c.Activa = 1
                GROUP BY
                   c.CategoriaId,
                    c.Nombre,
                    c.Icono,
                    pc.MontoAsignado,
                    c.OrdenVisualizacion
                ORDER BY c.OrdenVisualizacion;";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@PeriodoId", periodo);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cn.Open();
                        da.Fill(db);
                    }
                }
            }

            return db;
        }

        public static void InsertarPresupuesto(int periodoId, int categoriaId, decimal monto)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            string sSQL = @"INSERT INTO presupuesto_categoria
                        (PeriodoId,CategoriaId,MontoAsignado)
                        VALUES(@periodoId,@categoriaId,@monto)";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.AddWithValue("@periodoId", periodoId);
                    cmd.Parameters.AddWithValue("@categoriaId", categoriaId);
                    cmd.Parameters.AddWithValue("@monto", monto);


                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        //hace exacamente lo mismo que el metodo de arriba(InsertarPresupuesto) pero como este metodo se llama en un bucle 
        //el anterior cada llamada abria y cerraba la conexion y asi podia pasar 100veces solo para guardar 100 categoris
        //ahora con el metoodo de abajo solo se abre y cierra la conexion 1 vez (optimizamos)
        public static void GuardarPresupuestos(int periodoId, List<(int categoriaId, decimal monto)> datos)
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                SqlTransaction tx = cn.BeginTransaction();

                try
                {
                    foreach (var item in datos)
                    {
                        using (SqlCommand cmd = new SqlCommand(@"
                        IF EXISTS (SELECT 1 FROM PRESUPUESTO_CATEGORIA 
                                   WHERE PeriodoId = @periodoId AND CategoriaId = @categoriaId)
                            UPDATE PRESUPUESTO_CATEGORIA 
                            SET MontoAsignado = @monto
                            WHERE PeriodoId = @periodoId AND CategoriaId = @categoriaId
                        ELSE
                            INSERT INTO PRESUPUESTO_CATEGORIA (PeriodoId, CategoriaId, MontoAsignado)
                            VALUES (@periodoId, @categoriaId, @monto)", cn, tx))
                                        {
                                            cmd.Parameters.Add("@periodoId", SqlDbType.Int).Value = periodoId;
                                            cmd.Parameters.Add("@categoriaId", SqlDbType.Int).Value = item.categoriaId;
                                            cmd.Parameters.Add("@monto", SqlDbType.Decimal).Value = item.monto;

                                            cmd.ExecuteNonQuery();
                                        }       
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }














    }
}