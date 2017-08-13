using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de Correos
/// </summary>
public class Correos
{
	public Correos()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public List<String> ObtenerInformacion()
    {
        List<String> Lista = new List<String>();

        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT Correo, Contraseña, Host, Puerto, CAST(SSL AS VARCHAR), Texto FROM Correo;";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    Lista.Add(Lector.GetString(0));
                    Lista.Add(Lector.GetString(1));
                    Lista.Add(Lector.GetString(2));
                    Lista.Add(Lector.GetString(3));
                    Lista.Add(Lector.GetString(4));
                    Lista.Add(Lector.GetString(5));
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

    public Boolean ModificarCorreo(List<String> Lista)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String SSL = "0";
            if(Lista[4] != null)
            {
                SSL = "1";
            }
            String Query = "UPDATE Correo SET Correo = '" + DepCom.DepurarSinMayus(Lista[0]) + "', Contraseña = '" + DepCom.DepurarSinMayus(Lista[1]) + "', Host = '" + DepCom.DepurarSinMayus(Lista[2]) + "', Puerto = '" + DepCom.DepurarSinMayus(Lista[3]) + "', SSL = '" + SSL + "', Texto = '" + DepCom.DepurarSinMayus(Lista[5]) + "';";
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