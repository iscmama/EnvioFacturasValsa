using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de GestionCuentas
/// </summary>
public class GestionCuentas
{
	public GestionCuentas()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public List<String> ObtenerClientes(String IdUsuario)
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            //Checo si es cuenta master Y ADMINISTRADOR
            String QueryCheca = "SELECT CAST(Master AS INT), CAST(Administrador AS INT) FROM Usuarios WHERE Id_Usuario = '" + IdUsuario + "';";
            SqlCommand ComandoCheca = new SqlCommand(QueryCheca, conn, Transaccion);
            SqlDataReader LectorCheca = ComandoCheca.ExecuteReader();
            int Master = 0;
            int Admin = 0;
            if(LectorCheca.HasRows)
            {
                while(LectorCheca.Read())
                {
                    Master = LectorCheca.GetInt32(0);
                    Admin = LectorCheca.GetInt32(1);
                }
            }
            LectorCheca.Close();

            //Checo que realmente sea admin
            if(Admin == 1)
            {
                String Query = "";
                //Si realmente es admin, checo la condicion del master
                if(Master == 1)
                {
                    //Si es master, puedo checar todos los admin y las empresas
                    Query = "SELECT Id_Usuario, Nombre, Razon_Social FROM Usuarios INNER JOIN Empresa ON Empresa.Id_Empresa = Usuarios.Id_Empresa WHERE Id_Usuario <> '" + IdUsuario + "' AND Usuarios.Activo = '1';";
                }
                else
                {
                    //Si solo es admin, solo miro los que no sean admin y los de mi empresa
                    Query = "SELECT Id_Usuario, Nombre, Razon_Social FROM Usuarios INNER JOIN Empresa ON Empresa.Id_Empresa = Usuarios.Id_Empresa WHERE Id_Usuario <> '" + IdUsuario + "' AND Administrador <> 1 AND Usuarios.Id_Empresa = (SELECT Id_Empresa FROM Usuarios WHERE Id_Usuario = '" + IdUsuario + "' AND Usuarios.Activo = '1');";
                }
                SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
                SqlDataReader Lector = Comando.ExecuteReader();
                if(Lector.HasRows)
                {
                    while (Lector.Read())
                    {
                        Lista.Add(Lector.GetInt32(0).ToString());
                        Lista.Add(Lector.GetString(1));
                        Lista.Add(Lector.GetString(2));
                    }
                }
                Lector.Close();
            }



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

    public List<String> ObtenerEmpresas(String IdUsuario)
    {
        List<String> Lista = new List<String>();
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

            String Query = "";
            //Checo la condicion del master
            if (Master == 1)
            {
                //Si es master, puedo checar todos los admin y las empresas
                Query = "SELECT Id_Empresa, Razon_Social FROM Empresa ORDER BY Razon_Social;";
            }
            else
            {
                //Si solo es admin, solo miro los que no sean admin y los de mi empresa
                Query = "SELECT Id_Empresa, Razon_Social FROM Empresa WHERE Id_Empresa = (SELECT Id_Empresa FROM Usuarios WHERE Id_Usuario = '" + IdUsuario + "');";
            }
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    Lista.Add(Lector.GetInt32(0).ToString());
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

    public Boolean CrearCuenta(List<String> Lista)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "INSERT INTO Usuarios(Nombre, Usuario, Password, Administrador, Id_Empresa) VALUES('" + DepCom.Depurar(Lista[0]) + "', '" + DepCom.DepurarSinMayus(Lista[1]) + "', '" + DepCom.DepurarSinMayus(Lista[2]) + "', '" + DepCom.Depurar(Lista[3]) + "', '" + DepCom.Depurar(Lista[4]) + "');";
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

    public Boolean ExisteCuenta(String Usuario)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT Id_Usuario FROM Usuarios WHERE UPPER(Usuario) = '" + DepCom.Depurar(Usuario) + "' AND Activo = 1;";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(!Lector.HasRows)
            {
                Correcto = false;
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Correcto = true;
            Transaccion.Rollback();
            conn.Close();
        }
        return Correcto;
    }

    public Boolean EliminarCuenta(String IdUsuario)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "UPDATE Usuarios SET Activo = '0' WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "'";
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

    public Boolean CambiarContraseña(String IdUsuario, String Contraseña)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "UPDATE Usuarios SET Password = '" + DepCom.DepurarSinMayus(Contraseña) + "' WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "'";
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

    public List<String> ObtenerRFC(String IdUsuario)
    {
        List<String> Lista = new List<String>();
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

            String Query = "";
            //Checo la condicion del master
            if (Master == 1)
            {
                //Si es master, puedo checar todos los RFC
                Query = "SELECT Serie, Serie AS SerieDos FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa WHERE Empresa.Activo = 1 GROUP BY Serie ORDER BY Serie;";
            }
            else
            {
                //Si solo es admin, solo miro los que pertenezcan a su empresa
                Query = "SELECT Serie, Serie AS SerieDos FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa WHERE Empresa.Activo = 1 AND Empresa.Id_Empresa = (SELECT Id_Empresa FROM Usuarios WHERE Id_Usuario = '" + IdUsuario + "')  GROUP BY Serie ORDER BY Serie;";
            }
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

    public Boolean ModificarRFC(String IdUsuario, String RFC)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "UPDATE Usuarios SET RFC_Permisos = '" + DepCom.DepurarSinMayus(RFC) + "' WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "'";
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

    public String ObtenerRFCActual(String IdUsuario)
    {
        String RFC = "";

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT RFC_Permisos FROM Usuarios WHERE Id_Usuario = '" + DepCom.Depurar(IdUsuario) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    RFC = Lector.GetString(0);
                }
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            RFC = null;
            Transaccion.Rollback();
            conn.Close();
        }
        return RFC;
    }
}