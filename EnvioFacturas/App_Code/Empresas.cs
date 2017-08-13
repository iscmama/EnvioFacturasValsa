using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de Empresas
/// </summary>
public class Empresas
{
	public Empresas()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public List<String> ObtenerEmpresas()
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT Id_Empresa, UPPER(Razon_Social), UPPER(RFC) FROM Empresa WHERE Activo = 1 ORDER BY Razon_Social;";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    Lista.Add(Lector.GetInt32(0).ToString());
                    Lista.Add(Lector.GetString(1));
                    Lista.Add(Lector.GetString(2));
                }
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Transaccion.Rollback();
            conn.Close();
        }

        return Lista;
    }

    public List<String> InformacionEmpresa(String IdEmpresa)
    {
        DepuraComilla DepCom = new DepuraComilla();
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT UPPER(Razon_Social), UPPER(RFC) FROM Empresa WHERE Activo = 1 AND Id_Empresa = '" + DepCom.Depurar(IdEmpresa) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    Lista.Add(Lector.GetString(0));
                    Lista.Add(Lector.GetString(1));
                }
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Transaccion.Rollback();
            conn.Close();
        }

        return Lista;
    }

    public Boolean CrearEmpresa(List<String> Lista)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "INSERT INTO Empresa(Razon_Social, RFC) VALUES('" + DepCom.Depurar(Lista[0]) + "', '" + DepCom.DepurarSinMayus(Lista[1]) + "');";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            Comando.ExecuteNonQuery();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Correcto = false;
            Transaccion.Rollback();
            conn.Close();
        }
        return Correcto;
    }

    public Boolean EditarEmpresa(List<String> Lista)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "UPDATE Empresa SET Razon_Social = '" + DepCom.Depurar(Lista[0]) + "', RFC = '" + DepCom.Depurar(Lista[1]) + "' WHERE Id_Empresa = '" + DepCom.Depurar(Lista[2]) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            Comando.ExecuteNonQuery();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Correcto = false;
            Transaccion.Rollback();
            conn.Close();
        }
        return Correcto;
    }
}