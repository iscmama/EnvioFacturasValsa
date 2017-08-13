using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de MiCuenta
/// </summary>
public class MiCuenta
{
	public MiCuenta()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public String ModificarContraseña(List<String> Lista)
    {
        String Error = "";

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            //Primero checo que ñla cotnreaseña sea cporrecta
            Boolean Existe = false;
            String QueryCheca = "SELECT Id_Usuario FROM Usuarios WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(Lista[0]) + "' AND Password = '" + DepCom.DepurarSinMayus(Lista[1]) + "';";
            SqlCommand ComandoCheca = new SqlCommand(QueryCheca, conn, Transaccion);
            SqlDataReader LectorCheca = ComandoCheca.ExecuteReader();
            if(LectorCheca.HasRows)
            {
                Existe = true;
            }
            LectorCheca.Close();

            if(Existe)
            {
                //Si es correcta, inserto
                String Query = "UPDATE Usuarios SET Password = '" + DepCom.DepurarSinMayus(Lista[2]) + "' WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(Lista[0]) + "';";
                SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
                Comando.ExecuteNonQuery();
            }
            else
            {
                Error = "Contraseña incorrecta";
            }

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Error = "Error ocurrido durante la operacion";
            Transaccion.Rollback();
            conn.Close();
        }

        return Error;
    }
}