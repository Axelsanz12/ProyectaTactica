Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports Directiones
Imports Entidad
Imports Entidad.Ventas

Public Class ClienteDAO

    ' Obtiene todos los clientes
    Public Function ObtenerTodos() As List(Of Cliente)
        Dim lista As New List(Of Cliente)()
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("SELECT * FROM Clientes")
        acceso.ejecutarLectura()

        While acceso.Leer.Read()
            Dim c As New Cliente With {
                .ClienteID = acceso.Leer("ClienteID"),
                .Nombre = acceso.Leer("Nombre"),
                .Apellido = acceso.Leer("Apellido"),
                .Email = acceso.Leer("Email"),
                .Telefono = acceso.Leer("Telefono"),
                .Direccion = If(IsDBNull(acceso.Leer("Direccion")), "", acceso.Leer("Direccion").ToString()),
                .FechaAlta = acceso.Leer("FechaAlta")
            }
            lista.Add(c)
        End While

        acceso.cerrarConexion()
        Return lista
    End Function

    ' Inserta un nuevo cliente
    Public Sub Insertar(c As Cliente)
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("INSERT INTO Clientes (Nombre, Apellido, Email, Telefono, Direccion, FechaAlta) " &
                             "VALUES (@Nombre, @Apellido, @Email, @Telefono, @Direccion, @FechaAlta)")

        acceso.setearParametros("@Nombre", c.Nombre)
        acceso.setearParametros("@Apellido", c.Apellido)
        acceso.setearParametros("@Email", c.Email)
        acceso.setearParametros("@Telefono", c.Telefono)
        acceso.setearParametros("@Direccion", c.Direccion)
        acceso.setearParametros("@FechaAlta", c.FechaAlta)

        acceso.ejecutarAccion()
    End Sub

    ' Modifica un cliente existente
    Public Sub Modificar(c As Cliente)
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("UPDATE Clientes SET Nombre=@Nombre, Apellido=@Apellido, Email=@Email, Telefono=@Telefono, Direccion=@Direccion " &
                             "WHERE ClienteID=@ClienteID")

        acceso.setearParametros("@ClienteID", c.ClienteID)
        acceso.setearParametros("@Nombre", c.Nombre)
        acceso.setearParametros("@Apellido", c.Apellido)
        acceso.setearParametros("@Email", c.Email)
        acceso.setearParametros("@Telefono", c.Telefono)
        acceso.setearParametros("@Direccion", c.Direccion)

        acceso.ejecutarAccion()
    End Sub

    ' Elimina un cliente por ID
    Public Sub Eliminar(id As Integer)
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("DELETE FROM Clientes WHERE ClienteID = @id")
        acceso.setearParametros("@id", id)

        acceso.ejecutarAccion()
    End Sub

    ' Obtiene todas las ventas con el nombre del cliente
    Public Function ObtenerVentas() As List(Of Venta)
        Dim lista As New List(Of Venta)()
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("
            SELECT v.ID, v.IDCliente, v.Fecha, v.Total, c.Nombre 
            FROM Ventas v 
            INNER JOIN Clientes c ON v.IDCliente = c.ClienteID 
            ORDER BY c.Nombre")

        acceso.ejecutarLectura()

        While acceso.Leer.Read()
            Dim v As New Venta With {
                .ID = acceso.Leer("ID"),
                .IDCliente = acceso.Leer("IDCliente"),
                .Fecha = acceso.Leer("Fecha"),
                .Total = acceso.Leer("Total"),
                .NombreCliente = acceso.Leer("Nombre").ToString()
            }
            lista.Add(v)
        End While

        acceso.cerrarConexion()
        Return lista
    End Function

    ' Obtiene todos los productos
    Public Function ObtenerProductos() As List(Of Producto)
        Dim lista As New List(Of Producto)()
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("SELECT * FROM Productos")
        acceso.ejecutarLectura()

        While acceso.Leer.Read()
            Dim p As New Producto With {
                .ProductoID = Convert.ToInt32(acceso.Leer("ProductoID")),
                .Nombre = acceso.Leer("Nombre").ToString(),
                .Descripcion = acceso.Leer("Descripcion").ToString(),
                .Precio = Convert.ToDecimal(acceso.Leer("Precio")),
                .Stock = Convert.ToInt32(acceso.Leer("Stock")),
                .FechaAlta = Convert.ToDateTime(acceso.Leer("FechaAlta"))
            }
            lista.Add(p)
        End While

        acceso.cerrarConexion()
        Return lista
    End Function

    ' Inserta un producto nuevo
    Public Sub InsertarProducto(p As Producto)
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, FechaAlta) " &
                             "VALUES (@Nombre, @Descripcion, @Precio, @Stock, @FechaAlta)")

        acceso.setearParametros("@Nombre", p.Nombre)
        acceso.setearParametros("@Descripcion", p.Descripcion)
        acceso.setearParametros("@Precio", p.Precio)
        acceso.setearParametros("@Stock", p.Stock)
        acceso.setearParametros("@FechaAlta", p.FechaAlta)

        acceso.ejecutarAccion()
    End Sub

    ' Modifica un producto existente
    Public Sub ModificarProducto(p As Producto)
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("UPDATE Productos SET Nombre=@Nombre, Descripcion=@Descripcion, Precio=@Precio, Stock=@Stock " &
                             "WHERE ProductoID=@ProductoID")

        acceso.setearParametros("@ProductoID", p.ProductoID)
        acceso.setearParametros("@Nombre", p.Nombre)
        acceso.setearParametros("@Descripcion", p.Descripcion)
        acceso.setearParametros("@Precio", p.Precio)
        acceso.setearParametros("@Stock", p.Stock)

        acceso.ejecutarAccion()
    End Sub

    ' Elimina un producto por ID
    Public Sub EliminarProducto(id As Integer)
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("DELETE FROM Productos WHERE ProductoID = @id")
        acceso.setearParametros("@id", id)

        acceso.ejecutarAccion()
    End Sub

    ' Obtiene una venta por ID
    Public Function ObtenerVentaPorID(idVenta As Integer) As Venta
        Dim venta As Venta = Nothing
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("SELECT * FROM Ventas WHERE ID = @ID")
        acceso.setearParametros("@ID", idVenta)
        acceso.ejecutarLectura()

        If acceso.Leer.Read() Then
            venta = New Venta With {
                .ID = acceso.Leer("ID"),
                .IDCliente = acceso.Leer("IDCliente"),
                .Fecha = acceso.Leer("Fecha"),
                .Total = acceso.Leer("Total")
            }
        End If

        acceso.cerrarConexion()
        Return venta
    End Function

    ' Obtiene los items de una venta
    Public Function ObtenerItemsPorVenta(idVenta As Integer) As List(Of VentaItem)
        Dim lista As New List(Of VentaItem)()
        Dim acceso As New AcessoDatos()

        acceso.setearConsulta("SELECT * FROM VentasItems WHERE IDVenta = @IDVenta")
        acceso.setearParametros("@IDVenta", idVenta)
        acceso.ejecutarLectura()

        While acceso.Leer.Read()
            Dim item As New VentaItem With {
                .ID = acceso.Leer("ID"),
                .IDVenta = acceso.Leer("IDVenta"),
                .IDProducto = acceso.Leer("IDProducto"),
                .PrecioUnitario = acceso.Leer("PrecioUnitario"),
                .Cantidad = acceso.Leer("Cantidad"),
                .PrecioTotal = acceso.Leer("PrecioTotal")
            }
            lista.Add(item)
        End While

        acceso.cerrarConexion()
        Return lista
    End Function

    ' Modifica una venta y sus items
    Public Sub ModificarVenta(venta As Venta, items As List(Of VentaItem))
        Dim acceso As New AcessoDatos()

        ' Actualiza datos generales de la venta
        acceso.setearConsulta("UPDATE Ventas SET IDCliente = @IDCliente, Fecha = @Fecha, Total = @Total WHERE ID = @ID")
        acceso.setearParametros("@IDCliente", venta.IDCliente)
        acceso.setearParametros("@Fecha", venta.Fecha)
        acceso.setearParametros("@Total", venta.Total)
        acceso.setearParametros("@ID", venta.ID)
        acceso.ejecutarAccion()

        ' Elimina items anteriores
        Dim accDelete As New AcessoDatos()
        accDelete.setearConsulta("DELETE FROM VentasItems WHERE IDVenta = @IDVenta")
        accDelete.setearParametros("@IDVenta", venta.ID)
        accDelete.ejecutarAccion()

        ' Inserta items nuevos
        For Each item In items
            Dim accItem As New AcessoDatos()
            accItem.setearConsulta("INSERT INTO VentasItems (IDVenta, IDProducto, PrecioUnitario, Cantidad, PrecioTotal) " &
                                  "VALUES (@IDVenta, @IDProducto, @PrecioUnitario, @Cantidad, @PrecioTotal)")
            accItem.setearParametros("@IDVenta", venta.ID)
            accItem.setearParametros("@IDProducto", item.IDProducto)
            accItem.setearParametros("@PrecioUnitario", item.PrecioUnitario)
            accItem.setearParametros("@Cantidad", item.Cantidad)
            accItem.setearParametros("@PrecioTotal", item.PrecioTotal)
            accItem.ejecutarAccion()
        Next
    End Sub

    ' Elimina una venta y sus items
    Public Sub EliminarVenta(idVenta As Integer)
        Dim accDeleteItems As New AcessoDatos()
        accDeleteItems.setearConsulta("DELETE FROM VentasItems WHERE IDVenta = @IDVenta")
        accDeleteItems.setearParametros("@IDVenta", idVenta)
        accDeleteItems.ejecutarAccion()

        Dim accDeleteVenta As New AcessoDatos()
        accDeleteVenta.setearConsulta("DELETE FROM Ventas WHERE ID = @ID")
        accDeleteVenta.setearParametros("@ID", idVenta)
        accDeleteVenta.ejecutarAccion()
    End Sub

End Class
