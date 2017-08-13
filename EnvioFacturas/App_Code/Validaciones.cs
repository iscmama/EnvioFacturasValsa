using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de Validaciones
/// </summary>
public class Validaciones
{
	public Validaciones()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public Boolean EsMaster(String IdUsuario)
    {
        Boolean SiEs = false;
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            //Checo si es cuenta master
            String QueryCheca = "SELECT CAST(Master AS INT) FROM Usuarios WHERE Id_Usuario = '" + IdUsuario + "';";
            SqlCommand ComandoCheca = new SqlCommand(QueryCheca, conn, Transaccion);
            SqlDataReader LectorCheca = ComandoCheca.ExecuteReader();
            int Master = 0;
            if (LectorCheca.HasRows)
            {
                while (LectorCheca.Read())
                {
                    Master = LectorCheca.GetInt32(0);
                }
            }
            LectorCheca.Close();

            //Checo la condicion del master
            if (Master == 1)
            {
                SiEs = true;
            }
            else
            {
                SiEs = false;
            }

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Transaccion.Rollback();
            conn.Close();
        }

        return SiEs;
    }
}