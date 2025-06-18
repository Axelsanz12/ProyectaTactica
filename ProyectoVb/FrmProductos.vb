Imports Entidad
Imports Directiones

Public Class FrmProductos
    Private dao As New ClienteDAO()

    Private Sub FrmProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarProductos()
    End Sub

    Private Sub CargarProductos()
        Try
            Dim lista = dao.ObtenerProductos()
            dgvProductos.DataSource = Nothing
            dgvProductos.DataSource = lista
        Catch ex As Exception
            MessageBox.Show("Error al cargar productos: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Try
            Dim nuevo As New Producto With {
                .Nombre = txtNombre.Text.Trim(),
                .Descripcion = txtDescripcion.Text.Trim(),
                .Precio = Decimal.Parse(txtPrecio.Text),
                .Stock = Integer.Parse(txtStock.Text),
                .FechaAlta = dtpFecha.Value
            }

            dao.InsertarProducto(nuevo) ' 👈 Método específico para productos
            MessageBox.Show("Producto agregado correctamente.")
            CargarProductos()
            LimpiarCampos()
        Catch ex As Exception
            MessageBox.Show("Error al agregar producto: " & ex.Message)
        End Try
    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If dgvProductos.CurrentRow Is Nothing Then
            MessageBox.Show("Seleccioná un producto para modificar.")
            Return
        End If

        Try
            Dim producto As New Producto With {
                .ProductoID = Convert.ToInt32(dgvProductos.CurrentRow.Cells("ProductoID").Value),
                .Nombre = txtNombre.Text.Trim(),
                .Descripcion = txtDescripcion.Text.Trim(),
                .Precio = Decimal.Parse(txtPrecio.Text),
                .Stock = Integer.Parse(txtStock.Text),
                .FechaAlta = dtpFecha.Value
            }

            dao.ModificarProducto(producto) ' 👈 Método específico para productos
            MessageBox.Show("Producto modificado correctamente.")
            CargarProductos()
            LimpiarCampos()
        Catch ex As Exception
            MessageBox.Show("Error al modificar producto: " & ex.Message)
        End Try
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If dgvProductos.CurrentRow Is Nothing Then
            MessageBox.Show("Seleccioná un producto para eliminar.")
            Return
        End If

        Dim id As Integer = Convert.ToInt32(dgvProductos.CurrentRow.Cells("ProductoID").Value)
        If MessageBox.Show("¿Estás seguro de eliminar el producto?", "Confirmar", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                dao.EliminarProducto(id) ' 👈 Método específico para productos
                MessageBox.Show("Producto eliminado correctamente.")
                CargarProductos()
                LimpiarCampos()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar producto: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub LimpiarCampos()
        txtNombre.Text = ""
        txtDescripcion.Text = ""
        txtPrecio.Text = ""
        txtStock.Text = ""
        dtpFecha.Value = DateTime.Now
    End Sub

    Private Sub dgvProductos_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProductos.CellClick
        If e.RowIndex >= 0 Then
            Dim fila As DataGridViewRow = dgvProductos.Rows(e.RowIndex)
            txtNombre.Text = fila.Cells("Nombre").Value.ToString()
            txtDescripcion.Text = fila.Cells("Descripcion").Value.ToString()
            txtPrecio.Text = fila.Cells("Precio").Value.ToString()
            txtStock.Text = fila.Cells("Stock").Value.ToString()
            dtpFecha.Value = Convert.ToDateTime(fila.Cells("FechaAlta").Value)
        End If
    End Sub

    Private Sub txtBuscarProducto_TextChanged(sender As Object, e As EventArgs) Handles txtBuscarProducto.TextChanged
        Try
            Dim criterio As String = txtBuscarProducto.Text.Trim().ToLower()

            Dim lista = dao.ObtenerProductos()
            Dim listaFiltrada = lista.Where(Function(p)
                                                Return (p.Nombre.ToLower().Contains(criterio) OrElse
                                                    p.Descripcion.ToLower().Contains(criterio))
                                            End Function).ToList()

            dgvProductos.DataSource = Nothing
            dgvProductos.DataSource = listaFiltrada

        Catch ex As Exception
            MessageBox.Show("Error al buscar productos: " & ex.Message)
        End Try
    End Sub

End Class
