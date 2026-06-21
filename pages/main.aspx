<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Presupuesto.pages.main" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <link href="../css/main.css" rel="stylesheet" />

    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Presupuesto</title>

    <%-- Bootstrap 5 CSS --%>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    
    <%-- Tabler Icons --%>
    <link href="https://cdn.jsdelivr.net/npm/@tabler/icons-webfont@latest/tabler-icons.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    
</head>

<form id="form1" runat="server">

<div class="layout">

<aside class="aside">
  <div class="aside-brand">
    <div class="aside-brand-icon">
      <i class="ti ti-wallet"></i>
    </div>
    <div>
      <p class="aside-brand-name">Mi Presupuesto</p>
      <p class="aside-brand-year">2025</p>
    </div>
  </div>

    <asp:LinkButton ID="lnkAbrirModalEditar" runat="server"
    style="display:none"
    OnClick="lnkAbrirModalEditar_Click">
</asp:LinkButton>

  <div class="aside-ingreso">
    <p class="aside-ingreso-label">Sueldo mensual</p>
    <asp:Label ID="lblIngreso" runat="server"
    CssClass="aside-ingreso-valor"
    style="cursor:pointer; border-bottom: 1px dashed #475569;"
    ToolTip="Clic para editar">
    </asp:Label>

<asp:TextBox ID="txtIngresoEditar" runat="server"
    CssClass="aside-ingreso-valor"
    style="display:none; background:transparent; border:none; 
           border-bottom: 1px solid #818cf8; color:#f1f5f9; 
           width:120px; outline:none;">
</asp:TextBox>
  </div>

   <asp:HiddenField ID="hfAnio" runat="server" Value="" />
   <asp:HiddenField ID="hfMesSeleccionado" runat="server" Value="" />
    <asp:HiddenField ID="hfNombreMes" runat="server" Value="" />
    <asp:HiddenField ID="hfIngresoNuevo" runat="server" Value="" />
    <asp:HiddenField ID="hfPresupuesto" runat="server" Value="" />
    <asp:HiddenField ID="hfGastoIdEliminar" runat="server" Value="" />
    <asp:HiddenField ID="hfEditarGasto" runat="server" Value="" />    

  <div class="aside-section-label" style="display:flex; align-items:center; justify-content:space-between;">
    <span>Períodos</span>
    <asp:DropDownList ID="ddlAnio" runat="server" 
        AutoPostBack="true"
        OnSelectedIndexChanged="ddlAnio_SelectedIndexChanged"
        CssClass="ddl-anio">
    </asp:DropDownList>
</div>

  <nav class="aside-nav">
    <asp:Repeater ID="rptMeses" runat="server">
        <ItemTemplate>
            <asp:LinkButton 
                ID="lnkMes" 
                runat="server"
                CssClass="periodo-item"
                CommandArgument='<%# Eval("MesNumero") + "|" + Eval("Anio") %>'
                OnCommand="lnkMes_Command">
                <div class="periodo-icon">
                    <i class="ti ti-calendar-month"></i>
                </div>
                <div class="periodo-texto">
                    <p class="periodo-mes"><%# Eval("Nombre") %></p>
                    <p class="periodo-anio"><%# Eval("Anio") %></p>
                </div>

                </asp:LinkButton>

        </ItemTemplate>
    </asp:Repeater>
</nav>

  <div class="aside-footer">
    <a href="#" class="btn-nuevo-periodo">
      <i class="ti ti-plus"></i>
      <span>Nuevo períodoo</span>
    </a>
  </div>
</aside>

   <main class="main-content">
  <div class="container-fluid p-3">
    <div class="row g-4">

      <!-- ══ IZQUIERDA: Presupuesto restante ══ -->
      <div class="col-6">
        <div class="panel-card">
          <p class="panel-label">Presupuesto restante</p>
          <asp:Label ID="lblPresupuestoRestante" runat="server"
              CssClass="panel-valor"
              Text="0,00 €">
          </asp:Label>
          <button type="button" class="btn-añadir" 
                  onclick="abrirModalGasto()">
            <i class="ti ti-plus" aria-hidden="true"></i>
            Añadir gasto
          </button>
        </div>
      </div>

      <!-- ══ DERECHA: Categorías ══ -->
      <div class="col-6">
        <div class="panel-card panel-card-categorias">
          
          <%-- Header fijo --%>
          <p class="panel-label">Categorías</p>

          <%-- Zona con scroll --%>
          <div class="categorias-scroll">
            <asp:Repeater ID="rptCategorias" runat="server">
              <ItemTemplate>
                <div class="categoria-fila">
                  <div class="categoria-nombre">
                    <i class="ti <%# Eval("Icono") %>" aria-hidden="true"></i>
                    <span><%# Eval("Nombre") %></span>
                  </div>
                  <div class="categoria-montos">
                    <div class="monto-bloque">
                      <p class="monto-label">Inicial</p>
                      <asp:TextBox ID="txtMonto" runat="server"
                          CssClass="monto-textbox"
                          Text='<%#  string.Format("{0:N2}", Eval("MontoAsignado")) %>'>
                      </asp:TextBox>
                    </div>
                    <div class="monto-bloque">
                      <p class="monto-label">Restante</p>
                      <asp:Label 
                            ID="label1" 
                            runat="server"
                            CssClass='<%# "monto-textbox " + ObtenerClaseRestante(Eval("Sobrante"), Eval("MontoAsignado")) %>'
                            Text='<%# string.Format("{0:N2}", Eval("Sobrante")) %>'>
                       </asp:Label>
                    </div>
                  </div>
                  <asp:HiddenField ID="hfCategoriaId" runat="server"
                      Value='<%# Eval("CategoriaId") %>' />
                </div>
              </ItemTemplate>
            </asp:Repeater>
          </div>

          <%-- Botón fijo abajo --%>
          <div class="categorias-footer">
            <asp:Button ID="btnGuardarCategorias" runat="server"
                Text="Guardar categorías"
                CssClass="btn-guardar-categorias"
                OnClick="btnGuardarCategorias_Click" />
          </div>

        </div>
      </div>

    </div>
  </div>
  <div class="gastos-seccion">
    <div class="tabla-gastos-wrapper">
       <div class="tabla-gastos-header">
            <span>Categoría</span>
            <span>Fecha</span>
            <span>Comentario</span>
            <span>Monto</span>
            <span>Categoria Restante</span>
            <span>Opciones</span>
        </div>

        <div class="tabla-gastos-scroll">
            <asp:Repeater ID="RptGastos" runat="server">
                <ItemTemplate>
                    <div class="tabla-gastos-fila">
                        <span class="gasto-categoria"><%# Eval("Nombre") %></span>
                        <span class="gasto-fecha"><%# Convert.ToDateTime(Eval("Fecha")).ToString("dd/MM/yyyy") %></span>
                        <span class="gasto-comentario"><%# Eval("Comentario") ?? "-" %></span>
                        <span class="gasto-monto"><%# string.Format("{0:N2} €", Eval("Monto")) %></span>
                        <span class="gasto-monto <%# ObtenerClaseRestante(Eval("CategoriaRestante"), Eval("MontoAsignado")) %>">
                            <%# string.Format("{0:N2} €", Eval("CategoriaRestante")) %>
                        </span>
                        <span class="gasto-opciones">
                        <button type="button"
                            class="btn-icono btn-editar-gasto"
                            onclick="abrirModalEditarGasto(this)"
                            data-gastoid='<%# Eval("GastoId") %>'
                            data-categoriaid='<%# Eval("CategoriaId")%>'
                            data-categoria='<%# Eval("Nombre") %>'
                            data-monto='<%# Convert.ToDecimal(Eval("Monto")).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) %>'
                            data-comentario='<%# Eval("Comentario") == DBNull.Value ? "" : Eval("Comentario") %>'
                            data-fecha='<%# Convert.ToDateTime(Eval("Fecha")).ToString("yyyy-MM-dd") %>'
                            title="Editar">
                            <i class="ti ti-edit"></i>
                        </button>

                        <button type="button" 
                            class="btn-icono btn-eliminar-gasto"
                            onclick="abrirModalConfirmarEliminar(this)"
                            data-gastoid='<%# Eval("GastoId") %>'
                            data-categoria='<%# Eval("Nombre") %>'
                            data-monto='<%# string.Format("{0:N2}", Eval("Monto")) %>'
                            title="Eliminar">
                            <i class="ti ti-trash"></i>
                        </button>
                        </span>
                        <asp:HiddenField ID="HiddenFieldTablaGasto" runat="server"
                            Value='<%# Eval("GastoId") %>' />
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlSinGastos" runat="server" Visible="false" CssClass="mensaje-vacio">
                Aún no existen gastos en este mes.
            </asp:Panel>
        </div>
    </div>

    <div class="filtro-categoria">
    <p class="panel-label">Filtrar por categoría</p>
    <asp:DropDownList ID="ddlFiltrarGastos" runat="server" 
        CssClass="filtro-categoria-ddl">
    </asp:DropDownList>
    <asp:Button runat="server" ID="Button1"
        CssClass="btn-filtrar-gastos"
        Text="Filtrar" OnClick="FiltrarGastos" />
</div>
</div>
        

</main>
    

 </div>

        <!-- MODAL PARA CREAR NUEVO MES Y ANIO SIN PRESUPUESTO-->

<div class="modal fade" id="modalNuevoPresupuesto" tabindex="-1" 
     data-bs-backdrop="static" data-bs-keyboard="false">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">

            <div class="modal-header border-0 pb-0">
                <h5 class="modal-title">
                    <i class="ti ti-calendar-exclamation me-2 text-warning"></i>
                    Sin presupuesto
                </h5>
            </div>

            <div class="modal-body">
                <p class="text-muted mb-4">
                    El mes de <strong>
                        <asp:Label ID="lblMesModal"  runat="server"></asp:Label>
                    </strong> de <strong>
                        <asp:Label ID="lblAnioModal" runat="server"></asp:Label>
                    </strong> aún no tiene presupuesto asignado.
                </p>

                <div class="mb-3">
                    <label class="form-label fw-500">Ingreso del mes</label>
                    <div class="input-group">
                        <span class="input-group-text">€</span>
                        <asp:TextBox ID="txtIngreso" runat="server"
                            CssClass="form-control"
                            placeholder="0,00"
                            TextMode="Number">
                        </asp:TextBox>
                    </div>
                    <label class="form-label fw-500">Ahorro del mes</label>
                    <div class="input-group">
                        <span class="input-group-text">€</span>
                        <asp:TextBox ID="TxtAhorro" runat="server"
                            CssClass="form-control"
                            placeholder="0,00"
                            TextMode="Number">
                        </asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="modal-footer border-0 pt-0">
                <button type="button" class="btn btn-outline-secondary" 
                        data-bs-dismiss="modal">
                    Cancelar
                </button>
                <asp:Button ID="btnGuardarPresupuesto" runat="server"
                    Text="Guardar"
                    CssClass="btn btn-primary"
                    OnClick="btnGuardarPresupuesto_Click" />
            </div>

        </div>
    </div>
</div>

    <!-- MODAL PARA MODIFICAR PRESUPUESTO-->
    <div class="modal fade" id="modalEditarPresupuesto" tabindex="-1"
     data-bs-backdrop="static" data-bs-keyboard="false">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">

            <div class="modal-header border-0 pb-0">
                <h5 class="modal-title">
                    <i class="ti ti-edit me-2 text-info"></i>
                    Cambiar presupuesto
                </h5>
            </div>

            <div class="modal-body">
                <p class="text-muted">
                    ¿Estás seguro que deseas cambiar el presupuesto de
                    <strong><asp:Label ID="lblMesEditar"  runat="server"></asp:Label></strong> de
                    <strong><asp:Label ID="lblAnioEditar" runat="server"></asp:Label></strong> de
                    <strong><asp:Label ID="lblIngresoAnterior" runat="server"></asp:Label> €</strong> a
                    <strong><asp:Label ID="lblIngresoNuevo"    runat="server"></asp:Label> €</strong>?
                </p>
            </div>

            <div class="modal-footer border-0 pt-0">
                <button type="button" class="btn btn-outline-secondary"
                        id="btnCancelarEditar"
                        data-bs-dismiss="modal">
                    Cancelar
                </button>
                <asp:Button ID="btnConfirmarEditar" runat="server"
                    Text="Confirmar"
                    CssClass="btn btn-primary"
                    OnClick="btnConfirmarEditar_Click" />
            </div>

        </div>
    </div>
</div>

        <!-- Modal Añadir Gasto -->
<div class="modal fade" id="modalAñadirGasto" tabindex="-1"
     data-bs-backdrop="static" data-bs-keyboard="false">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content modal-gasto-content">

      <div class="modal-header border-0 pb-0">
        <div style="display:flex; align-items:center; gap:10px;">
          <div style="width:34px;height:34px;border-radius:8px;background:rgba(99,102,241,0.1);display:flex;align-items:center;justify-content:center;">
            <i class="ti ti-receipt" style="font-size:17px;color:#6366f1;"></i>
          </div>
          <h5 class="modal-title" style="font-size:15px;font-weight:500;">Añadir gasto</h5>
        </div>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
      </div>

      <div class="modal-body pt-3">

        <div class="gasto-campo">
          <label class="gasto-label">Categoría</label>
          <div class="gasto-input-box">
            <i class="ti ti-tag"></i>
            <asp:DropDownList ID="ddlCrearGasto" runat="server" CssClass="form-control"></asp:DropDownList>
          </div>
        </div>

        <div class="gasto-campo">
          <label class="gasto-label">Monto</label>
          <div class="gasto-input-box">
            <span class="gasto-euro">€</span>
            <asp:TextBox ID="txtMontoGasto" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0.01" placeholder="0,00"></asp:TextBox>
          </div>
        </div>

        <div class="gasto-campo">
          <label class="gasto-label">Fecha</label>
          <div class="gasto-input-box">
            <i class="ti ti-calendar"></i>
            <asp:TextBox ID="txtFechaGasto" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
          </div>
        </div>

        <div class="gasto-campo">
          <label class="gasto-label">Comentario <span style="font-size:11px;color:#94a3b8;text-transform:none;font-weight:400;">(opcional)</span></label>
          <div class="gasto-input-box" style="align-items:flex-start;">
            <i class="ti ti-message" style="margin-top:1px;"></i>
            <asp:TextBox ID="txtComentarioGasto" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Añade una nota..."></asp:TextBox>
          </div>
        </div>

      </div>

      <div class="modal-footer border-0 pt-0">
        <button type="button" class="btn btn-outline-secondary btn-sm" data-bs-dismiss="modal">Cancelar</button>
        <asp:Button ID="btnGuardarGasto" runat="server" Text="Guardar gasto"
            CssClass="btn btn-primary btn-sm"
            OnClick="btnGuardarGasto_Click"
            OnClientClick="return validarMontoGasto();"/>
      </div>

    </div>
  </div>
</div>


        <!-- MODAL PARA CONFIRMAR ELIMINACION DE GASTO-->
    <div class="modal fade" id="modalEliminarGasto" tabindex="-1"
 data-bs-backdrop="static" data-bs-keyboard="false">
<div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">

        <div class="modal-header border-0 pb-0">
            <h5 class="modal-title">
                <i class="ti ti-trash me-2 text-danger"></i>
                Confirmar Eliminación
            </h5>
        </div>

        <div class="modal-body">
            <p class="text-muted">
                ¿Estás seguro que deseas eliminar el gasto de
                <strong><span id="spanCategoriaEliminar"></span></strong> de monto
                <strong><span id="spanMontoEliminar"></span> €</strong>
            </p>
        </div>

        <div class="modal-footer border-0 pt-0">
            <button type="button" class="btn btn-outline-secondary"
                    id="btnCancelarEliminar"
                    data-bs-dismiss="modal">
                Cancelar
            </button>
            <asp:Button ID="btnConfirmarEliminar" runat="server"
                Text="Confirmar"
                CssClass="btn btn-primary"
                
                OnClick="btnEliminarGasto_Click" />
        </div>

    </div>
</div>
</div>


</form>




    <script>

    // Clic en el label → muestra el textbox
    document.getElementById('<%= lblIngreso.ClientID %>').addEventListener('click', function () {
        var lbl = document.getElementById('<%= lblIngreso.ClientID %>');
        var txt = document.getElementById('<%= txtIngresoEditar.ClientID %>');

        // Pasar el valor actual al textbox (quitar el " €")
        txt.value = lbl.innerText.replace(' €', '');

        console.log("viejo: " + lbl.innerText);
        console.log("nuevo: " + txt.value);

        lbl.style.display = 'none';
        txt.style.display = 'inline-block';
        txt.focus();
        txt.select();
    });



    // Al salir del textbox → restaurar label por ahora
        document.getElementById('<%= txtIngresoEditar.ClientID %>').addEventListener('blur', function () {
            var lbl = document.getElementById('<%= lblIngreso.ClientID %>');
            var txt = document.getElementById('<%= txtIngresoEditar.ClientID %>');
            var hfNuevo = document.getElementById('<%= hfIngresoNuevo.ClientID %>');

            var numNuevo = parseFloat(txt.value.replace(/\./g, '').replace(',', '.')).toFixed(2); // formato como se guarda en la bd 1200.00

            var valorViejo = lbl.innerText.replace(' €', '').trim();
            hfNuevo.value = formatearEuro(txt.value)  // formato de comparacion del label y texto agregado 1.200,00

            var valorNuevo = hfNuevo.value

         

        // Si no cambió nada o está vacío, restaurar sin modal
            if (valorNuevo === '' || valorNuevo === lbl.innerText.replace(' €', '').trim()) {
            txt.style.display = 'none';
            lbl.style.display = 'inline-block';
            console.log("entre en igualdad");
            return;
        }

        
            // Guardar el valor nuevo en el HiddenField
            hfNuevo.value = numNuevo;
            console.log("guarda en bd: " + hfNuevo.value);

        // Ocultar textbox y mostrar label mientras el modal está abierto
            txt.style.display  = 'none';
            lbl.style.display = 'inline-block';
        
            
            // Hacer postback para rellenar los labels del modal y abrirlo



            document.getElementById('<%= lnkAbrirModalEditar.ClientID %>').click();
            
        });

        function formatearEuro(valor) {
            if (valor == null || valor === '') return '';

            let str = valor.toString().trim();

            // Quitar puntos de miles y cambiar coma decimal a punto
            str = str.replace(/\./g, '').replace(',', '.');

            let num = Number(str);
            if (isNaN(num)) return '';

            // de-DE garantiza punto de miles y coma decimal en todos los navegadores
            return new Intl.NumberFormat('de-DE', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }).format(num);
        }

        function abrirModalGasto() {
            document.getElementById('<%= hfEditarGasto.ClientID %>').value = 0;

            document.getElementById('<%= ddlCrearGasto.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= txtMontoGasto.ClientID %>').value = "";
            document.getElementById('<%= txtFechaGasto.ClientID %>').value = "";
            document.getElementById('<%= txtComentarioGasto.ClientID %>').value = "";


            var modal = new bootstrap.Modal(document.getElementById('modalAñadirGasto'));
            modal.show();
        }
        function abrirModalConfirmarEliminar(boton) {
            // aquí "boton" es exactamente el mismo elemento que "this"
            // ahora podemos hacer boton.dataset.gastoid, boton.dataset.categoria, etc.

            var modal = new bootstrap.Modal(document.getElementById('modalEliminarGasto'));
            document.getElementById("spanCategoriaEliminar").textContent = boton.dataset.categoria;
            document.getElementById("spanMontoEliminar").textContent = boton.dataset.monto;
            document.getElementById('<%= hfGastoIdEliminar.ClientID %>').value = boton.dataset.gastoid;
            modal.show();
        }

        function abrirModalEditarGasto(boton) {
            var modal = new bootstrap.Modal(document.getElementById('modalAñadirGasto'));

            document.getElementById('<%= hfEditarGasto.ClientID %>').value = boton.dataset.gastoid;

            document.getElementById("<%= ddlCrearGasto.ClientID %>").value = boton.dataset.categoriaid;
            document.getElementById("<%= txtMontoGasto.ClientID %>").value = boton.dataset.monto;
            document.getElementById("<%= txtComentarioGasto.ClientID %>").value = boton.dataset.comentario;
            document.getElementById("<%= txtFechaGasto.ClientID %>").value = boton.dataset.fecha;

            modal.show();
       }
        function validarMontoGasto() {

            let value = parseFloat(document.getElementById("<%= txtMontoGasto.ClientID %>").value);

            if (value >= 0.01) {
                return true;
            }
            alert("El monto debe ser mayor a 0")
            return false;

        }







    </script>