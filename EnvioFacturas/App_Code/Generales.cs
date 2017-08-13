using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de Generales
/// </summary>
public class Generales
{
	public Generales()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public String ObtenerNombre(String IdUsuario)
    {
        String Respuesta = "";
        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT UPPER(Nombre) FROM Usuarios WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    Respuesta = Lector.GetString(0);
                }
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Respuesta = "";
            Transaccion.Rollback();
            conn.Close();
        }

        return Respuesta;
    }

    public String ObtenerSerie(String IdUsuario)
    {
        String Respuesta = "";
        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT UPPER(RFC_Permisos) FROM Usuarios WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    Respuesta = Lector.GetString(0);
                }
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Respuesta = "";
            Transaccion.Rollback();
            conn.Close();
        }

        return Respuesta;
    }

    public String ObtenerEmpresa(String IdUsuario)
    {
        String Respuesta = "";
        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT UPPER(Razon_Social) FROM Usuarios INNER JOIN Empresa ON Empresa.Id_Empresa = Usuarios.Id_Empresa WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    Respuesta = Lector.GetString(0);
                }
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Respuesta = "";
            Transaccion.Rollback();
            conn.Close();
        }

        return Respuesta;
    }

    public Boolean EsAdministrador(String IdUsuario)
    {
        Boolean Respuesta = false;
        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT UPPER(Nombre) FROM Usuarios WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "' AND Administrador = '1' AND Activo = '1';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                Respuesta = true;
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Respuesta = false;
            Transaccion.Rollback();
            conn.Close();
        }

        return Respuesta;
    }

    public Boolean EsMaster(String IdUsuario)
    {
        Boolean Respuesta = false;
        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT UPPER(Nombre) FROM Usuarios WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "' AND Master = '1' AND Activo = '1';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                Respuesta = true;
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Respuesta = false;
            Transaccion.Rollback();
            conn.Close();
        }

        return Respuesta;
    }
}