Imports System.Data.SqlClient
Imports Directiones

Public Class AcessoDatos
    Private comando As SqlCommand
    Private lector As SqlDataReader

    Public ReadOnly Property Leer() As SqlDataReader
        Get
            Return lector
        End Get
    End Property

    Public Sub setearConsulta(consulta As String)
        comando = New SqlCommand(consulta)
        comando.CommandType = CommandType.Text
    End Sub

    Public Sub ejecutarLectura()
        Dim conexion As SqlConnection = Directiones.Conexion.ObtenerConexion()
        comando.Connection = conexion
        Try
            conexion.Open()
            lector = comando.ExecuteReader(CommandBehavior.CloseConnection)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub ejecutarAccion()
        Using conexion As SqlConnection = Directiones.Conexion.ObtenerConexion()
            comando.Connection = conexion
            Try
                conexion.Open()
                comando.ExecuteNonQuery()
            Catch ex As Exception
                Throw ex
            End Try
        End Using
    End Sub

    Public Sub setearParametros(nombre As String, valor As Object)
        If comando Is Nothing Then
            Throw New InvalidOperationException("Primero debe llamar a setearConsulta para inicializar el comando.")
        End If
        comando.Parameters.AddWithValue(nombre, valor)
    End Sub

    Public Sub cerrarConexion()
        If lector IsNot Nothing Then
            lector.Close()
        End If
    End Sub

    Public Function ejecutarEscalar() As Object
        Using conexion As SqlConnection = Directiones.Conexion.ObtenerConexion()
            comando.Connection = conexion
            Try
                conexion.Open()
                Return comando.ExecuteScalar()
            Catch ex As Exception
                Throw ex
            End Try
        End Using
    End Function

End Class
