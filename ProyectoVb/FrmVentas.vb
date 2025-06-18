Imports Directiones
Imports Entidad
Imports Entidad.Ventas

Public Class FrmVentas
    Private dao As New ClienteDAO()
    Private listaProductos As List(Of Producto)
    Private ventaItems As New List(Of VentaItem)

    Private Sub FrmVentas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarClientes()
        CargarProductos()
        CargarVentas()
        InicializarGrilla()
        LimpiarTodo()
    End Sub

    Private bool As Boolean = False
    Private Sub CargarClientes()
        If bool Then Return
        cmbClientes.DataSource = dao.ObtenerTodos()
        cmbClientes.DisplayMember = "Nombre"
        cmbClientes.ValueMember = "ClienteID"
    End Sub

    Private Sub CargarProductos()
        listaProductos = dao.ObtenerProductos()
        cmbProductos.DataSource = listaProductos
        cmbProductos.DisplayMember = "Nombre"
        cmbProductos.ValueMember = "ProductoID"
    End Sub

    Private cargandoCombo As Boolean = False
    Private Sub CargarVentas()
        cargandoCombo = True

        ' Limpiar ComboBox
        cmbVentas.DataSource = Nothing
        cmbVentas.Items.Clear()

        ' Obtener todas las ventas sin duplicados
        Dim listaVentasOriginal As List(Of Venta) = dao.ObtenerVentas()

        ' Eliminar duplicados por ID (puede pasar si la consulta o inserción está mal)
        Dim listaSinDuplicados = listaVentasOriginal _
        .GroupBy(Function(v) v.ID) _
        .Select(Function(g) g.First()) _
        .ToList()

        ' Asignar la lista al ComboBox
        cmbVentas.DataSource = listaSinDuplicados
        cmbVentas.DisplayMember = "NombreCliente"
        cmbVentas.ValueMember = "ID"
        cmbVentas.SelectedIndex = -1

        cargandoCombo = False
    End Sub


    Private Sub InicializarGrilla()
        dgvItems.Columns.Clear()
        dgvItems.Columns.Add("Producto", "Producto")
        dgvItems.Columns.Add("Cantidad", "Cantidad")
        dgvItems.Columns.Add("PrecioUnitario", "Precio Unitario")
        dgvItems.Columns.Add("PrecioTotal", "Total")
    End Sub

    Private Sub btnAgregarItem_Click(sender As Object, e As EventArgs) Handles btnAgregarItem.Click
        If String.IsNullOrWhiteSpace(txtCantidad.Text) OrElse Not Integer.TryParse(txtCantidad.Text, Nothing) Then
            MessageBox.Show("Ingrese una cantidad válida.")
            Return
        End If

        Dim producto As Producto = DirectCast(cmbProductos.SelectedItem, Producto)
        Dim cantidad As Integer = Integer.Parse(txtCantidad.Text)

        Dim item As New VentaItem With {
            .IDProducto = producto.ProductoID,
            .Cantidad = cantidad,
            .PrecioUnitario = producto.Precio,
            .PrecioTotal = producto.Precio * cantidad
        }

        ventaItems.Add(item)
        dgvItems.Rows.Add(producto.Nombre, cantidad, producto.Precio, item.PrecioTotal)
        CalcularTotalGeneral()
    End Sub

    Private Sub CalcularTotalGeneral()
        Dim total As Decimal = ventaItems.Sum(Function(i) i.PrecioTotal)
        lblTotal.Text = "Total: $" & total.ToString("F2")
    End Sub

    Private Sub btnGuardarVenta_Click(sender As Object, e As EventArgs) Handles btnGuardarVenta.Click
        btnGuardarVenta.Enabled = False

        Try
            If ventaItems.Count = 0 Then
                MessageBox.Show("Debe agregar al menos un producto.")
                Return
            End If

            Dim venta As New Venta With {
            .IDCliente = Convert.ToInt32(cmbClientes.SelectedValue),
            .Fecha = dtpFecha.Value,
            .Total = ventaItems.Sum(Function(i) i.PrecioTotal)
        }

            GuardarVenta(venta, ventaItems)
            MessageBox.Show("Venta guardada exitosamente.")
            LimpiarTodo()
            CargarVentas()

        Catch ex As Exception
            MessageBox.Show("Error al guardar la venta: " & ex.Message)
        Finally
            btnGuardarVenta.Enabled = True
        End Try
    End Sub


    Private Sub GuardarVenta(venta As Venta, items As List(Of VentaItem))
        Dim acceso As New AcessoDatos()
        acceso.setearConsulta("INSERT INTO Ventas (IDCliente, Fecha, Total) VALUES (@IDCliente, @Fecha, @Total); SELECT SCOPE_IDENTITY();")
        acceso.setearParametros("@IDCliente", venta.IDCliente)
        acceso.setearParametros("@Fecha", venta.Fecha)
        acceso.setearParametros("@Total", venta.Total)

        Dim resultado = acceso.ejecutarEscalar()
        venta.ID = Convert.ToInt32(resultado)

        For Each item In items
            Dim accItem As New AcessoDatos()
            accItem.setearConsulta("INSERT INTO VentasItems (IDVenta, IDProducto, PrecioUnitario, Cantidad, PrecioTotal) VALUES (@IDVenta, @IDProducto, @PrecioUnitario, @Cantidad, @PrecioTotal)")
            accItem.setearParametros("@IDVenta", venta.ID)
            accItem.setearParametros("@IDProducto", item.IDProducto)
            accItem.setearParametros("@PrecioUnitario", item.PrecioUnitario)
            accItem.setearParametros("@Cantidad", item.Cantidad)
            accItem.setearParametros("@PrecioTotal", item.PrecioTotal)
            accItem.ejecutarAccion()
        Next
    End Sub


    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If ventaItems.Count = 0 Then
            MessageBox.Show("Debe agregar al menos un producto para modificar la venta.")
            Return
        End If

        Dim idVenta As Integer = ObtenerIDVentaSeleccionada()
        If idVenta = 0 Then
            MessageBox.Show("Seleccione una venta válida para modificar.")
            Return
        End If

        Dim ventaModificada As New Venta With {
            .ID = idVenta,
            .IDCliente = Convert.ToInt32(cmbClientes.SelectedValue),
            .Fecha = dtpFecha.Value,
            .Total = ventaItems.Sum(Function(i) i.PrecioTotal)
        }

        Try
            ModificarVenta(ventaModificada, ventaItems)
            MessageBox.Show("Venta modificada correctamente.")
            LimpiarTodo()
            CargarVentas()
        Catch ex As Exception
            MessageBox.Show("Error al modificar la venta: " & ex.Message)
        End Try
    End Sub

    Private Sub ModificarVenta(venta As Venta, items As List(Of VentaItem))
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("UPDATE Ventas SET IDCliente = @IDCliente, Fecha = @Fecha, Total = @Total WHERE ID = @ID")
        acceso.setearParametros("@IDCliente", venta.IDCliente)
        acceso.setearParametros("@Fecha", venta.Fecha)
        acceso.setearParametros("@Total", venta.Total)
        acceso.setearParametros("@ID", venta.ID)
        acceso.ejecutarAccion()

        Dim accDelete As New AcessoDatos()
        accDelete.setearConsulta("DELETE FROM VentasItems WHERE IDVenta = @IDVenta")
        accDelete.setearParametros("@IDVenta", venta.ID)
        accDelete.ejecutarAccion()

        For Each item In items
            Dim accItem As New AcessoDatos()
            accItem.setearConsulta("INSERT INTO VentasItems (IDVenta, IDProducto, PrecioUnitario, Cantidad, PrecioTotal) VALUES (@IDVenta, @IDProducto, @PrecioUnitario, @Cantidad, @PrecioTotal)")
            accItem.setearParametros("@IDVenta", venta.ID)
            accItem.setearParametros("@IDProducto", item.IDProducto)
            accItem.setearParametros("@PrecioUnitario", item.PrecioUnitario)
            accItem.setearParametros("@Cantidad", item.Cantidad)
            accItem.setearParametros("@PrecioTotal", item.PrecioTotal)
            accItem.ejecutarAccion()
        Next
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        Dim idVenta As Integer = ObtenerIDVentaSeleccionada()
        If idVenta = 0 Then
            MessageBox.Show("Seleccione una venta válida para eliminar.")
            Return
        End If

        Dim resultado = MessageBox.Show("¿Está seguro que desea eliminar la venta seleccionada?", "Confirmar eliminación", MessageBoxButtons.YesNo)
        If resultado = DialogResult.Yes Then
            Try
                EliminarVenta(idVenta)
                MessageBox.Show("Venta eliminada correctamente.")
                LimpiarTodo()
                CargarVentas()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar la venta: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub EliminarVenta(idVenta As Integer)
        Dim accDeleteItems As New AcessoDatos()
        accDeleteItems.setearConsulta("DELETE FROM VentasItems WHERE IDVenta = @IDVenta")
        accDeleteItems.setearParametros("@IDVenta", idVenta)
        accDeleteItems.ejecutarAccion()

        Dim accDeleteVenta As New AcessoDatos()
        accDeleteVenta.setearConsulta("DELETE FROM Ventas WHERE ID = @ID")
        accDeleteVenta.setearParametros("@ID", idVenta)
        accDeleteVenta.ejecutarAccion()
    End Sub

    Private Function ObtenerIDVentaSeleccionada() As Integer
        If cmbVentas.SelectedItem IsNot Nothing AndAlso cmbVentas.SelectedValue IsNot Nothing Then
            Return Convert.ToInt32(cmbVentas.SelectedValue)
        Else
            Return 0
        End If
    End Function

    Private Sub cmbVentas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbVentas.SelectedIndexChanged
        If cargandoCombo Then Return
        Dim idVenta As Integer = ObtenerIDVentaSeleccionada()
        If idVenta > 0 Then
            Dim ventaSeleccionada As Venta = dao.ObtenerVentaPorID(idVenta)
            If ventaSeleccionada IsNot Nothing Then
                cmbClientes.SelectedValue = ventaSeleccionada.IDCliente
                dtpFecha.Value = ventaSeleccionada.Fecha

                ventaItems = dao.ObtenerItemsPorVenta(idVenta)
                dgvItems.Rows.Clear()
                For Each item In ventaItems
                    Dim prod = listaProductos.Find(Function(p) p.ProductoID = item.IDProducto)
                    dgvItems.Rows.Add(prod.Nombre, item.Cantidad, item.PrecioUnitario, item.PrecioTotal)
                Next
                CalcularTotalGeneral()
            End If
        Else
            LimpiarTodo()
        End If
    End Sub

    Private Sub LimpiarTodo()
        ventaItems.Clear()
        dgvItems.Rows.Clear()
        txtCantidad.Text = ""
        lblTotal.Text = "Total: $0.00"
        cmbClientes.SelectedIndex = -1
        cmbProductos.SelectedIndex = -1
        dtpFecha.Value = DateTime.Now
        cmbVentas.SelectedIndex = -1
    End Sub

    Private Sub btnEliminarItems_Click(sender As Object, e As EventArgs) Handles btnEliminarItems.Click
        If dgvItems.SelectedRows.Count = 0 Then
            MessageBox.Show("Seleccione al menos un ítem para eliminar.")
            Return
        End If

        Dim indicesEliminar As New List(Of Integer)
        For Each row As DataGridViewRow In dgvItems.SelectedRows
            indicesEliminar.Add(row.Index)
        Next

        indicesEliminar.Sort()
        indicesEliminar.Reverse()

        For Each i In indicesEliminar
            ventaItems.RemoveAt(i)
            dgvItems.Rows.RemoveAt(i)
        Next

        CalcularTotalGeneral()
    End Sub

End Class
