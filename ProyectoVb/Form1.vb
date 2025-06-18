Imports Directiones
Imports Entidad

Public Class Form1
    Dim dao As New ClienteDAO()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarClientes()
    End Sub

    Private Sub CargarClientes()
        Try
            Dim lista = dao.ObtenerTodos()
            dgvClientes.DataSource = Nothing
            dgvClientes.DataSource = lista
        Catch ex As Exception
            MessageBox.Show("Error al cargar clientes: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Try
            Dim nuevo As New Cliente With {
                .Nombre = txtNombre.Text.Trim(),
                .Apellido = txtApellido.Text.Trim(),
                .Telefono = txtTelefono.Text.Trim(),
                .Email = txtEmail.Text.Trim(),
                .Direccion = txtDireccion.Text.Trim(),
                .FechaAlta = dtpFecha.Value
            }

            dao.Insertar(nuevo)
            MessageBox.Show("Cliente agregado correctamente.")
            CargarClientes()
            LimpiarCampos()
        Catch ex As Exception
            MessageBox.Show("Error al agregar cliente: " & ex.Message)
        End Try
    End Sub

    Private Sub LimpiarCampos()
        txtNombre.Text = ""
        txtApellido.Text = ""
        txtTelefono.Text = ""
        txtEmail.Text = ""
        txtDireccion.Text = ""
        dtpFecha.Value = DateTime.Now
    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If dgvClientes.CurrentRow Is Nothing Then
            MessageBox.Show("Seleccioná un cliente para modificar.")
            Return
        End If

        Try
            Dim cliente As New Cliente With {
                .ClienteID = Convert.ToInt32(dgvClientes.CurrentRow.Cells("ClienteID").Value),
                .Nombre = txtNombre.Text.Trim(),
                .Apellido = txtApellido.Text.Trim(),
                .Telefono = txtTelefono.Text.Trim(),
                .Email = txtEmail.Text.Trim(),
                .Direccion = txtDireccion.Text.Trim(),
                .FechaAlta = dtpFecha.Value
            }

            dao.Modificar(cliente)
            MessageBox.Show("Cliente modificado.")
            CargarClientes()
            LimpiarCampos()
        Catch ex As Exception
            MessageBox.Show("Error al modificar: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click
        If dgvClientes.CurrentRow Is Nothing Then
            MessageBox.Show("Seleccioná un cliente para eliminar.")
            Return
        End If

        Dim id As Integer = Convert.ToInt32(dgvClientes.CurrentRow.Cells("ClienteID").Value)
        If MessageBox.Show("¿Estás seguro de eliminar el cliente?", "Confirmar", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                dao.Eliminar(id)
                MessageBox.Show("Cliente eliminado.")
                CargarClientes()
                LimpiarCampos()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub dgvClientes_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvClientes.CellContentClick
        If e.RowIndex >= 0 Then
            Dim fila As DataGridViewRow = dgvClientes.Rows(e.RowIndex)
            txtNombre.Text = fila.Cells("Nombre").Value.ToString()
            txtApellido.Text = fila.Cells("Apellido").Value.ToString()
            txtTelefono.Text = fila.Cells("Telefono").Value.ToString()
            txtEmail.Text = fila.Cells("Email").Value.ToString()
            txtDireccion.Text = fila.Cells("Direccion").Value.ToString()
            dtpFecha.Value = Convert.ToDateTime(fila.Cells("FechaAlta").Value)
        End If
    End Sub

    Private Sub btnVerventas_Click(sender As Object, e As EventArgs) Handles btnVerventas.Click
        Try
            Dim listaVentas = dao.ObtenerVentas()
            dgvClientes.DataSource = Nothing
            dgvClientes.DataSource = listaVentas
        Catch ex As Exception
            MessageBox.Show("Error al cargar ventas: " & ex.Message)
        End Try


    End Sub

    Private Sub btnVentas_Click(sender As Object, e As EventArgs) Handles btnVentas.Click
        Dim frm As New FrmVentas()
        frm.ShowDialog()
    End Sub

    Private Sub btnProductos_Click(sender As Object, e As EventArgs) Handles btnProductos.Click
        Dim frm As New FrmProductos()
        frm.ShowDialog()
    End Sub

    Private Sub txtBuscar_TextChanged(sender As Object, e As EventArgs) Handles txtBuscar.TextChanged
        Try
            Dim criterio As String = txtBuscar.Text.Trim().ToLower()
            Dim lista = dao.ObtenerTodos()
            Dim listaFiltrada = lista.Where(Function(c)
                                                Return (c.Nombre.ToLower().Contains(criterio) OrElse
                                                    c.Apellido.ToLower().Contains(criterio) OrElse
                                                    c.Email.ToLower().Contains(criterio) OrElse
                                                    c.Telefono.ToLower().Contains(criterio))
                                            End Function).ToList()

            dgvClientes.DataSource = Nothing
            dgvClientes.DataSource = listaFiltrada

        Catch ex As Exception
            MessageBox.Show("Error al buscar clientes: " & ex.Message)
        End Try
    End Sub
End Class
