using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de Logs
/// </summary>
public class Logs
{
	public Logs()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public List<Variables> ObtenerLogs(Variables Vari)
    {
        List<Variables> Lista = new List<Variables>();

        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Filtro = "";
            if(Vari.Inicio != null)
            {
                if(Vari.Inicio.Length != 0)
                {
                    Filtro += " AND Fecha >= '" + Vari.Inicio + "'";
                }
            }
            if(Vari.Fin != null)
            {
                if(Vari.Fin.Length != 0)
                {
                    Filtro += " AND Fecha < DATEADD(DAY, 1, '" + Vari.Fin + "')";
                }
            }
            String Query = "SELECT CONCAT(REPLACE(CONVERT(VARCHAR, Fecha, 102), '.', '/'), ' ', CONVERT(VARCHAR, Fecha, 108)) AS Fecha, Modulo, Error FROM RegistroLogs WHERE 1 = 1 " + Filtro + ";";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    Variables Var = new Variables();
                    Var.Fecha = Lector.GetString(0);
                    Var.Modulo = Lector.GetString(1);
                    Var.Descripcion = Lector.GetString(2);
                    Lista.Add(Var);
                }
            }
Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Lista = null;
            try
            {
                Transaccion.Rollback();
                conn.Close();
            }
            catch
            {

            }
        }

        return Lista;
    }


    public String ObtenerLogsError(Variables Vari)
    {
String Error = "";
        List<Variables> Lista = new List<Variables>();

        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Filtro = "";
            if(Vari.Inicio != null)
            {
                if(Vari.Inicio.Length != 0)
                {
                    Filtro += " AND Fecha >= '" + Vari.Inicio + "'";
                }
            }
            if(Vari.Fin != null)
            {
                if(Vari.Fin.Length != 0)
                {
                    Filtro += " AND Fecha < DATEADD(DAY, 1, '" + Vari.Fin + "')";
                }
            }
            String Query = "SELECT CONCAT(REPLACE(CONVERT(VARCHAR, Fecha, 102), '.', '/'), ' ', CONVERT(VARCHAR, Fecha, 108)) AS Fecha, Modulo, Error FROM RegistroLogs WHERE 1 = 1 " + Filtro + ";";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    Variables Var = new Variables();
                    Var.Fecha = Lector.GetString(0);
                    Var.Modulo = Lector.GetString(1);
                    Var.Descripcion = Lector.GetString(2);
                    Lista.Add(Var);
                }
            }

            Transaccion.Commit();
            conn.Close();
        }
        catch(Exception ex)
        {
Error = ex.Message;
            Lista = null;
            try
            {
                Transaccion.Rollback();
                conn.Close();
            }
            catch
            {

            }
        }

        return Error;
    }
}