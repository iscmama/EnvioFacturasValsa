using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de Clientes
/// </summary>
public class Clientes
{
    public Clientes()
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }

    public List<ObjetoClientes> ObtenerResultados(String IdUsuario)
    {
        List<ObjetoClientes> Lista = new List<ObjetoClientes>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            //Checo si es cuenta master Y ADMINISTRADOR
            String QueryCheca = "SELECT CAST(Master AS INT), CAST(Administrador AS INT) FROM Usuarios WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "';";
            SqlCommand ComandoCheca = new SqlCommand(QueryCheca, conn, Transaccion);
            SqlDataReader LectorCheca = ComandoCheca.ExecuteReader();
            int Master = 0;
            int Admin = 0;
            if (LectorCheca.HasRows)
            {
                while (LectorCheca.Read())
                {
                    Master = LectorCheca.GetInt32(0);
                    Admin = LectorCheca.GetInt32(1);
                }
            }
            LectorCheca.Close();

            
            String Condiciones = "";
            if (Master == 1)
            {
                Condiciones += "";
                //QueryObtener = "SELECT Clientes.RFC, Facturas.Razon_Social FROM FACTURAS INNER JOIN Clientes ON Clientes.Id_Cliente = Facturas.Id_Cliente GROUP BY Razon_Social, Clientes.RFC ORDER BY Razon_Social;";
            }
            else if (Admin == 1)
            {
                Condiciones += " AND A.Id_Empresa = (SELECT Id_Empresa FROM Usuarios WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "')";
            }
            else
            {
                //Obtengo RFC de acceso
                String RFC = "";
                String QueryAcceso = "SELECT RFC_Permisos FROM Usuarios WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "';";
                SqlCommand ComandoAcceso = new SqlCommand(QueryAcceso, conn, Transaccion);
                SqlDataReader LectorAcceso = ComandoAcceso.ExecuteReader();
                if (LectorAcceso.HasRows)
                {
                    while (LectorAcceso.Read())
                    {
                        RFC = LectorAcceso.GetString(0);
                    }
                }
                LectorAcceso.Close();

                String[] ArregloRFC = RFC.Split(',');

                RFC = "";
                for (int i = 0; i < ArregloRFC.Length; i++)
                {
                    if (RFC.Length == 0)
                    {
                        RFC = "'" + ArregloRFC[i] + "'";
                    }
                    else
                    {
                        RFC += ",'" + ArregloRFC[i] + "'";
                    }
                }
                RFC = RFC.ToUpper();

                Condiciones += " AND A.Serie IN(" + RFC + ")";
            }

            String QueryObtener = "SELECT B.RFC, (SELECT TOP(1) UPPER(C.Razon_Social) FROM Facturas AS C WHERE C.Id_Cliente = B.Id_Cliente GROUP BY C.Razon_Social ORDER BY COUNT(*) DESC) AS Razon_Social FROM Facturas AS A INNER JOIN Clientes AS B ON B.Id_Cliente = A.Id_Cliente WHERE B.RFC NOT LIKE '%010101000'" + Condiciones + " GROUP BY B.Id_Cliente, B.RFC UNION SELECT B.RFC, UPPER(A.Razon_Social) FROM Facturas AS A INNER JOIN Clientes AS B ON B.Id_Cliente = A.Id_Cliente WHERE B.RFC LIKE '%010101000'" + Condiciones + " GROUP BY A.Razon_Social, B.RFC;";
            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            SqlDataReader LectorObtener = ComandoObtener.ExecuteReader();
            if (LectorObtener.HasRows)
            {
                while (LectorObtener.Read())
                {
                    ObjetoClientes ObjCli = new ObjetoClientes();
                    ObjCli.RazonSocial = LectorObtener.GetString(1);
                    ObjCli.RFC = LectorObtener.GetString(0);
                    Lista.Add(ObjCli);
                }
            }
            LectorObtener.Close();

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
    public List<String> ObtenerCorreos(String IdCliente, String Numero)
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "SELECT Id_Correo, Correo FROM CorreosCliente WHERE Activo = 1 AND Id_Cliente = '" + DepCom.Depurar(IdCliente) + "' AND Numero_Cliente = '" + DepCom.Depurar(Numero) + "';";
            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            SqlDataReader LectorObtener = ComandoObtener.ExecuteReader();
            if (LectorObtener.HasRows)
            {
                while (LectorObtener.Read())
                {
                    Lista.Add(LectorObtener.GetInt32(0).ToString());
                    Lista.Add(LectorObtener.GetString(1).ToUpper());
                }
            }
            LectorObtener.Close();

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

    public List<String> ObtenerSeries(String IdUsuario)
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            //Checo si es cuenta master Y ADMINISTRADOR
            String QueryCheca = "SELECT CAST(Master AS INT), CAST(Administrador AS INT) FROM Usuarios WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "';";
            SqlCommand ComandoCheca = new SqlCommand(QueryCheca, conn, Transaccion);
            SqlDataReader LectorCheca = ComandoCheca.ExecuteReader();
            int Master = 0;
            int Admin = 0;
            if (LectorCheca.HasRows)
            {
                while (LectorCheca.Read())
                {
                    Master = LectorCheca.GetInt32(0);
                    Admin = LectorCheca.GetInt32(1);
                }
            }
            LectorCheca.Close();

            String QueryObtener = "";

            if (Master == 1)
            {
                QueryObtener = "SELECT Serie FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa WHERE Empresa.Activo = 1 GROUP BY Serie ORDER BY Serie;";
            }
            else if (Admin == 1)
            {
                QueryObtener = "SELECT Serie FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa WHERE Empresa.Activo = 1 AND Facturas.Id_Empresa = (SELECT Id_Empresa FROM Usuarios WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "') GROUP BY Serie ORDER BY Serie;";
            }
            else
            {
                //Obtengo RFC de acceso
                String RFC = "";
                String QueryAcceso = "SELECT RFC_Permisos FROM Usuarios WHERE Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "';";
                SqlCommand ComandoAcceso = new SqlCommand(QueryAcceso, conn, Transaccion);
                SqlDataReader LectorAcceso = ComandoAcceso.ExecuteReader();
                if (LectorAcceso.HasRows)
                {
                    while (LectorAcceso.Read())
                    {
                        RFC = LectorAcceso.GetString(0);
                    }
                }
                LectorAcceso.Close();

                String[] ArregloRFC = RFC.Split(',');

                RFC = "";
                for (int i = 0; i < ArregloRFC.Length; i++)
                {
                    if (RFC.Length == 0)
                    {
                        RFC = "'" + ArregloRFC[i] + "'";
                    }
                    else
                    {
                        RFC += ",'" + ArregloRFC[i] + "'";
                    }
                }
                RFC = RFC.ToUpper();

                QueryObtener = "SELECT Serie FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa WHERE Empresa.Activo = 1 AND Facturas.Serie IN(" + RFC + ") GROUP BY Serie ORDER BY Serie;";
            }



            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            SqlDataReader LectorObtener = ComandoObtener.ExecuteReader();
            if (LectorObtener.HasRows)
            {
                while (LectorObtener.Read())
                {
                    Lista.Add(LectorObtener.GetString(0).ToUpper());
                }
            }
            LectorObtener.Close();

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

    public Boolean AgregarCorreo(String IdCliente, String Correo, String Numero)
    {
        Boolean Correcto = true;
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "INSERT INTO CorreosCliente(Id_Cliente, Correo, Numero_Cliente) VALUES('" + DepCom.Depurar(IdCliente) + "', '" + DepCom.Depurar(Correo) + "', '" + DepCom.Depurar(Numero) + "');";
            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            ComandoObtener.ExecuteNonQuery();

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

    public String NombreCorreo(String IdCorreo)
    {
        String Resultado = "";

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT Correo FROM CorreosCliente WHERE Id_Correo = '" + DepCom.Depurar(IdCorreo) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    Resultado = Lector.GetString(0);
                }
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Resultado = "";

            Transaccion.Rollback();
            conn.Close();
        }

        return Resultado;
    }

    public Boolean BorrarCorreo(String IdCorreo)
    {
        Boolean Correcto = true;
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "UPDATE CorreosCliente SET Activo = 0 WHERE Id_Correo = '" + DepCom.Depurar(IdCorreo) + "';";
            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            ComandoObtener.ExecuteNonQuery();

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