using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de IniciarSesion
/// </summary>
public class IniciarSesion
{
	public IniciarSesion()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public List<int> Iniciar(String Usuario, String Contraseña)
    {
        List<int> Lista = new List<int>();
        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

			String Query = "SELECT Id_Usuario, CAST(Administrador AS INT), CAST(Master AS INT) FROM Usuarios WHERE Usuario = '" + DepCom.Depurar(Usuario) + "' AND Password = '" + DepCom.Depurar(Contraseña) + "' AND Activo = 1;";
			SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
			SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    Lista.Add(Lector.GetInt32(0));
                    Lista.Add(Lector.GetInt32(1));
                    Lista.Add(Lector.GetInt32(2));
                }
            }
            Lector.Close();




            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            try
            {
                Transaccion.Rollback();
                conn.Close();
            }
            catch
            {

            }            
            Lista = null;
        }

        return Lista;
    }
}