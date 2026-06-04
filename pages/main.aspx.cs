using Microsoft.Ajax.Utilities;
using Presupuesto.DATA;
using Presupuesto.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using static Presupuesto.Helper.PeriodoHelper;

namespace Presupuesto.pages
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarAnios();
                hfAnio.Value = DateTime.Now.Year.ToString();
                CargarMeses(DateTime.Now.Year);

                int mesActual = DateTime.Now.Month;
                int anioActual = DateTime.Now.Year;

                // 👇 Guardar mes actual en HiddenFields
                hfMesSeleccionado.Value = mesActual.ToString();
                hfNombreMes.Value = PeriodoHelper.GenerarMeses(anioActual)
                                        .First(m => m.MesNumero == mesActual).Nombre;

                //carga todos los datos de la pagina principal
                CargarDatosMes(mesActual, anioActual);
                CargarCategoriasDropdown();



            }

        }

        private void CargarAnios()
        {
            int anioActual = DateTime.Now.Year;

            // Mostramos 3 años atrás y 1 adelante
            for (int i = anioActual - 3; i <= anioActual + 1; i++)
            {
                ddlAnio.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

            // Seleccionar el año actual por defecto
            ddlAnio.SelectedValue = anioActual.ToString();
        }

        private void CargarMeses(int anio)
        {
            var meses = PeriodoHelper.GenerarMeses(anio);
            rptMeses.DataSource = meses;
            rptMeses.DataBind();
        }

        protected void ddlAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            int anioSeleccionado = int.Parse(ddlAnio.SelectedValue);

            // Actualizar el HiddenField con el año elegido
            hfAnio.Value = anioSeleccionado.ToString();

            // Recargar los meses con el nuevo año
            CargarMeses(anioSeleccionado);
        }

        protected void CargarCategoriasDropdown()
        {
            DataTable dt = CategoriaData.ObtenerCategorias();
            ddlCrearGasto.DataSource = dt;
            ddlCrearGasto.DataTextField = "Nombre";
            ddlCrearGasto.DataValueField = "CategoriaId";
            ddlCrearGasto.DataBind();
        }

        protected void lnkMes_Command(object sender, CommandEventArgs e)
        {
            //aqui llega  "4|2026", ya que envio desde el front : CommandArgument='<%# Eval("MesNumero") + "|" + Eval("Anio") %>' 
            string data = e.CommandArgument.ToString();

            string[] partes = data.Split('|');

            int mes = int.Parse(partes[0]);
            int Anio = int.Parse(partes[1]);

            DataTable mesAnio = PeriodoData.ObtenerMesyAnio(mes, Anio);
            DataTable presuestoCategoria = PresupuestoCategoriaData.ObtenerCategoriasPorMes(mes,Anio);
            if (mesAnio.Rows.Count > 0)
            {

                if (presuestoCategoria.Rows.Count > 0)
                {
                    rptCategorias.DataSource = presuestoCategoria;
                    rptCategorias.DataBind();


                }
                else {

                    //Entra aqui cuando es un mes nuevo que no tiene categoria asignadas en la tabla PresupustoCategoria
                    //Como en el front uso un repeater con "monto asignado" pero esta consulta no devuelve esa row se le añade
                    //esa row con valor 0, para asi no teneer que usar 2 repeater uno para cada categoria
                    DataTable categorias = CategoriaData.ObtenerCategorias();

                    categorias.Columns.Add("MontoAsignado", typeof(decimal));

                    foreach (DataRow row in categorias.Rows)
                    {
                        row["MontoAsignado"] = 0;
                    }

                    rptCategorias.DataSource = categorias;
                    rptCategorias.DataBind();

                }
                

            }
            else
            {
                //Aqui se muestra el modal si el mes seleccionado aun no tiene presupuesto(es decir no esta en la BD)
                System.Diagnostics.Debug.WriteLine("NO EXISTE EN LA BD");
                hfMesSeleccionado.Value = mes.ToString();
                hfNombreMes.Value = PeriodoHelper.GenerarMeses(Anio)
                        .First(m => m.MesNumero == mes).Nombre;

                lblMesModal.Text = hfNombreMes.Value;
                lblAnioModal.Text = Anio.ToString();

                // Abrir el modal desde servidor
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                "abrirModal",
                "var modal = new bootstrap.Modal(document.getElementById('modalNuevoPresupuesto')); modal.show();",
                true);

            }
            MostrarIngreso(mesAnio);
        }

        private void MostrarIngreso(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                decimal ingreso = Convert.ToDecimal(dt.Rows[0]["Ingreso"]);
                System.Diagnostics.Debug.WriteLine(ingreso);
                lblIngreso.Text = ingreso.ToString("N2") + " €";
            }
            else
            {
                lblIngreso.Text = "Sin configurar";
            }
        }

        protected void btnGuardarPresupuesto_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtIngreso.Text)) return;

            int mes = int.Parse(hfMesSeleccionado.Value);
            int anio = int.Parse(hfAnio.Value);
            decimal ingreso = decimal.Parse(txtIngreso.Text);

            PeriodoData.InsertarPeriodo(mes, anio, ingreso);

            txtIngreso.Text = string.Empty;

            ScriptManager.RegisterStartupScript(this, this.GetType(),
            "cerrarModal",
            "$('#modalNuevoPresupuesto').modal('hide');",
            true);

            DataTable dt = PeriodoData.ObtenerMesyAnio(mes, anio);
            MostrarIngreso(dt);
            CargarDatosMes(mes, anio);

        }
        private void AbrirModalEditar()
        {
            // Limpiar el texto del label antes de parsear
            string textoActual = lblIngreso.Text
                            .Replace(" €", "")
                            .Trim()
                            .Replace(".", "")     // quitar puntos de miles
                            .Replace(",", ".");    // coma decimal → punto

            decimal ingresoAnterior = decimal.Parse(textoActual,
                                        System.Globalization.CultureInfo.InvariantCulture);

            decimal ingresoNuevo = decimal.Parse(hfIngresoNuevo.Value,
                                        System.Globalization.CultureInfo.InvariantCulture);

            System.Diagnostics.Debug.WriteLine("ya aprobado viejo: " + ingresoAnterior);
            System.Diagnostics.Debug.WriteLine("ya aprobado nuevo: " + ingresoNuevo);


            lblMesEditar.Text = hfNombreMes.Value;
            lblAnioEditar.Text = hfAnio.Value;
            lblIngresoAnterior.Text = ingresoAnterior.ToString("N2");
            lblIngresoNuevo.Text = ingresoNuevo.ToString("N2");

            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "abrirModalEditar",
                "var modal = new bootstrap.Modal(document.getElementById('modalEditarPresupuesto')); modal.show();",
                true);
        }

        protected void btnConfirmarEditar_Click(object sender, EventArgs e)
        {
            int mes = int.Parse(hfMesSeleccionado.Value);
            int anio = int.Parse(hfAnio.Value);
            decimal ingreso = decimal.Parse(hfIngresoNuevo.Value,
                          System.Globalization.CultureInfo.InvariantCulture);

            PeriodoData.ActualizarIngreso(mes, anio, ingreso);

            DataTable dt = PeriodoData.ObtenerMesyAnio(mes, anio);
            MostrarIngreso(dt);
        }

        protected void lnkAbrirModalEditar_Click(object sender, EventArgs e)
        {
            AbrirModalEditar();
        }

        protected void ObtenerCategoria() {

            DataTable Categorias = CategoriaData.ObtenerCategorias();
            rptCategorias.DataSource = Categorias;
            rptCategorias.DataBind();


        }

        //guardo las modificaciones en categoria
        protected void btnGuardarCategorias_Click(object sender, EventArgs e)
        {
            decimal monto;
            int categoriaId;
            int mes = int.Parse(hfMesSeleccionado.Value);
            int anio = int.Parse(hfAnio.Value);

            //obtengo el id del mes y año actual
            int periodo = Presupuesto.DATA.PeriodoData.ObtenerPeriodoId(mes, anio);

            //optimizacion, en vez de hacer un insert por cada categoria ahora guardo los datos en una lista y
            //despues de salir del bucles solo ejecuto 1 vez la operacion de insertar
            List<(int categoriaId, decimal monto)> datos = new List<(int, decimal)>();

           

            foreach (RepeaterItem valor in rptCategorias.Items)
            {
                TextBox txtMonto = (TextBox)valor.FindControl("txtMonto");
                HiddenField hfCategoriaId = (HiddenField)valor.FindControl("hfCategoriaId");

                categoriaId = int.Parse(hfCategoriaId.Value);
                monto = decimal.Parse(txtMonto.Text);
                //añado cada valor a la lista datos 
                datos.Add((categoriaId, monto));
            }

            //Solo se ejecuta una vez porque optimizamos
            Presupuesto.DATA.PresupuestoCategoriaData.GuardarPresupuestos(periodo, datos);

        }

        private void CargarDatosMes(int mes, int anio)
        {
            DataTable mesAnio = PeriodoData.ObtenerMesyAnio(mes, anio);
            DataTable presupuestoCategoria = PresupuestoCategoriaData.ObtenerCategoriasPorMes(mes, anio);

            if (mesAnio.Rows.Count > 0)
            {
                if (presupuestoCategoria.Rows.Count > 0)
                {
                    rptCategorias.DataSource = presupuestoCategoria;
                    rptCategorias.DataBind();
                }
                else
                {
                    // Mes existente pero sin categorías asignadas
                    DataTable categorias = CategoriaData.ObtenerCategorias();

                    categorias.Columns.Add("MontoAsignado", typeof(decimal));

                    foreach (DataRow row in categorias.Rows)
                    {
                        row["MontoAsignado"] = 0;
                    }

                    rptCategorias.DataSource = categorias;
                    rptCategorias.DataBind();
                }
            }

            MostrarIngreso(mesAnio);
        }

        protected void btnGuardarGasto_Click(Object sender, EventArgs e) {

            
            int categoria = int.Parse(ddlCrearGasto.SelectedValue);
            decimal gasto = decimal.Parse(txtMontoGasto.Text);
            string comentario = txtComentarioGasto.Text;

            DateTime fecha;
            if (!DateTime.TryParse(txtFechaGasto.Text, out fecha))
            {
                fecha = DateTime.Today;
            }
            fecha = fecha.Date;

            //funcion de insertar en gasto


            int mes = int.Parse(hfMesSeleccionado.Value);
            int anio = int.Parse(hfAnio.Value);
            int PeriodoID = DATA.PeriodoData.ObtenerPeriodoId(mes, anio);

            System.Diagnostics.Debug.WriteLine("PeriodoId: " + PeriodoID);
            System.Diagnostics.Debug.WriteLine("CategoriaId: " + categoria);
            
            DATA.GastoData.InsertarGasto(PeriodoID, categoria, gasto, fecha, comentario);




        }


    }




}