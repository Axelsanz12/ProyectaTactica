Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks


Public Class Conexion
    Public Shared Function ObtenerConexion() As SqlConnection
        Return New SqlConnection(ConfigurationManager.ConnectionStrings("conexionBD").ConnectionString)
    End Function
End Class
