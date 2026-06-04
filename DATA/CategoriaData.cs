using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Presupuesto.DATA
{
    public class CategoriaData
    {

        public static DataTable ObtenerCategorias()
        {
            string conexion = ConfigurationManager.ConnectionStrings["Presupuesto"].ConnectionString;

            DataTable db = new DataTable();

            string sSQL = @"SELECT * from Categoria WHERE Activa = @activa ORDER BY OrdenVisualizacion ASC";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, cn))
                {
                    cmd.Parameters.Add("@activa", SqlDbType.Int).Value = 1;
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