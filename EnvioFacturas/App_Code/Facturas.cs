using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Descripción breve de Facturas
/// </summary>
public class Facturas
{
	public Facturas()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public String EliminarAcuse(ObjetoAcuse Datos)
    {
        String Error = "";
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            //Primero checo que exista
            Boolean Existe = false;
            String Query = "SELECT Id_Factura FROM Acuses WHERE Id_Factura = '" + Datos.IdFactura + "' AND Numero = '" + Datos.NumeroString + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    Existe = true;
                }
            }
            Lector.Close();

            if(Existe)
            {
                Query = "UPDATE Acuses SET Activo = 0, Usuario_Elimina = '" + Datos.IdUsuario + "' WHERE Id_Factura = '" + Datos.IdFactura + "' AND Numero = '" + Datos.NumeroString + "';";
                Comando = new SqlCommand(Query, conn, Transaccion);
                Comando.ExecuteNonQuery();

                //Modificamos numeros
                Query = "UPDATE Acuses SET Numero = '1' WHERE Id_Factura = '" + Datos.IdFactura + "' AND Activo = '1';";
                Comando = new SqlCommand(Query, conn, Transaccion);
                Comando.ExecuteNonQuery();
            }
            else
            {
                Error = "No Existe el acuse a eliminar";
            }
            

            Transaccion.Commit();
            conn.Close();
        }
        catch(Exception ex)
        {
            try
            {
                Transaccion.Rollback();
                conn.Close();
            }
            catch
            {
            }
            Error = ex.Message;
        }
        return Error;
    }

    public List<ObjetoAcuse> ListarAcuses(int IdFactura)
    {
        List<ObjetoAcuse> Lista = new List<ObjetoAcuse>();

        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            int Numero = 1;
            String Query = "SELECT Archivo, Numero FROM Acuses WHERE Id_Factura = '" + IdFactura + "' AND Activo = '1';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while (Lector.Read())
                {
                    ObjetoAcuse ObjAcu = new ObjetoAcuse();
                    ObjAcu.Archivo = Lector.GetString(0);
                    ObjAcu.Numero = Lector.GetInt32(1);
                    Lista.Add(ObjAcu);
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
                Lista = null;
            }
            Lista = null;
        }

        return Lista;
    }

    public Boolean SubirAcuse(ObjetoAcuse Datos)
    {
        Boolean Correcto = true;
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            int Numero = 1;
            String Query = "SELECT COALESCE(MAX(Numero), 0) + 1 FROM Acuses WHERE Id_factura = '" + Datos.IdFactura + "' AND Activo = 1;";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    Numero = Lector.GetInt32(0);
                }
            }
            Lector.Close();

            Query = "INSERT INTO Acuses(Id_Factura, Archivo, Usuario_Sube, Numero) VALUES('" + Datos.IdFactura + "', '" + Datos.Archivo + "', '" + Datos.IdUsuario + "', '"+ Numero + "');";
            Comando = new SqlCommand(Query, conn, Transaccion);
            Comando.ExecuteNonQuery();

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
                Correcto = false;
            }
            Correcto = false;
        }
        return Correcto;
    }

    public List<String> ObtenerResultados(String IdCliente)
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "SELECT Id_Factura, Empresa.Razon_Social AS Emisor, Serie, Folio, Nota, Tipo_Comprobante, CONCAT(CONVERT(VARCHAR, Fecha_Emision, 103), ' ', CONVERT(VARCHAR, Fecha_Emision, 108)) , Condiciones_Pago, Metodo_Pago, Moneda, CAST(Tipo_Cambio AS VARCHAR), CAST(Sub_Total AS VARCHAR), CAST(IVA AS VARCHAR), CAST(Total AS VARCHAR) FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa WHERE Facturas.Id_Cliente = '" + DepCom.Depurar(IdCliente) + "';";
            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            SqlDataReader LectorObtener = ComandoObtener.ExecuteReader();
            if (LectorObtener.HasRows)
            {
                while (LectorObtener.Read())
                {
                    Lista.Add(LectorObtener.GetInt32(0).ToString());
                    Lista.Add(LectorObtener.GetString(1).ToUpper());
                    Lista.Add(LectorObtener.GetString(2).ToUpper());
                    Lista.Add(LectorObtener.GetString(3).ToUpper());
                    Lista.Add(LectorObtener.GetString(4).ToUpper());
                    Lista.Add(LectorObtener.GetString(5).ToUpper());
                    Lista.Add(LectorObtener.GetString(6).ToUpper());
                    Lista.Add(LectorObtener.GetString(7).ToUpper());
                    Lista.Add(LectorObtener.GetString(8).ToUpper());
                    Lista.Add(LectorObtener.GetString(9).ToUpper());
                    Lista.Add(LectorObtener.GetString(10).ToUpper());
                    Lista.Add(LectorObtener.GetString(11).ToUpper());
                    Lista.Add(LectorObtener.GetString(12).ToUpper());
                    Lista.Add(LectorObtener.GetString(13).ToUpper());
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

    public List<String> ObtenerRFC(String IdCliente)
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "SELECT RFC, (SELECT TOP(1) Razon_Social FROM Facturas WHERE Facturas.Id_Cliente = Clientes.Id_Cliente GROUP BY Razon_Social ORDER BY COUNT(*) DESC) FROM Clientes WHERE Clientes.Id_Cliente = '" + DepCom.Depurar(IdCliente) + "';";
            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            SqlDataReader LectorObtener = ComandoObtener.ExecuteReader();
            if (LectorObtener.HasRows)
            {
                while (LectorObtener.Read())
                {
                    Lista.Add(LectorObtener.GetString(0).ToUpper());
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

    public String Redireccionar(String PDFOXML, String IdFactura)
    {
        String Resultado = "";

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT REPLACE(Ruta_" + DepCom.Depurar(PDFOXML) + ", (SELECT Ruta FROM RutaProyecto), '') FROM Facturas WHERE Id_Factura = '" + DepCom.Depurar(IdFactura) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                while(Lector.Read())
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
            Transaccion.Rollback();
            conn.Close();
        }

        return Resultado;
    }

    public String ObtenerNota(String IdFactura)
    {
        String Resultado = "";

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT Nota FROM Facturas WHERE Id_Factura = '" + DepCom.Depurar(IdFactura) + "';";
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
            Transaccion.Rollback();
            conn.Close();
        }

        return Resultado;
    }

    public Boolean ActualizarNota(String Nota, String IdFactura)
    {
        Boolean Correcto = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "UPDATE Facturas SET Nota = '" + DepCom.Depurar(Nota) + "' WHERE Id_Factura = '" + DepCom.Depurar(IdFactura) + "';";
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

    public Boolean HayCorreo(String IdCliente, String Numero)
    {
        Boolean SiHay = false;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT Id_Correo FROM CorreosCliente WHERE Id_Cliente = '" + DepCom.Depurar(IdCliente) + "' AND Activo = 1 AND Numero_Cliente = '" + DepCom.Depurar(Numero) + "';";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if (Lector.HasRows)
            {
                SiHay = true;
            }
            Lector.Close();

            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            SiHay = false;
            String Query = "SELECT Id_Correo FROM CorreosCliente WHERE Id_Cliente = '" + DepCom.Depurar(IdCliente) + "' AND Activo = 1;";
        }

        return SiHay; ;
    }

    public List<String> DistintosTiposComprobantes()
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "SELECT DISTINCT(UPPER(Tipo_Comprobante)) FROM Facturas;";
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

    public List<String> DistintosStatus()
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "SELECT UPPER(Status) FROM Facturas GROUP BY Status ORDER BY Status;";
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

    public List<String> DistintosPaises()
    {
        List<String> Lista = new List<String>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String QueryObtener = "SELECT UPPER(País) FROM Facturas GROUP BY UPPER(País) ORDER BY UPPER(País);";
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

    public List<ObjetoFactura> ObtenerResultadosEspecificos(DatosEnviar Datos, String IdUsuario)
    {
        List<ObjetoFactura> Lista = new List<ObjetoFactura>();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();

        try
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            Transaccion = conn.BeginTransaction();

            //Checamos si hay serie
            String Condiciones = " WHERE 1 = 1";
            if(Datos.Serie != null)
            {
                if(Datos.Serie.Trim().Length != 0)
                {
                    Condiciones += " AND Serie = '" + DepCom.Depurar(Datos.Serie.Trim()) + "'";
                }
            }
            //Checo si hay folio
            if (Datos.Folio != null)
            {
                if (Datos.Folio.Trim().Length != 0)
                {
                    Condiciones += " AND Folio = '" + DepCom.Depurar(Datos.Folio.Trim()) + "'";
                }
            }
            //Ahora el tipo de comprobante
            if (Datos.TipoComprobante != null)
            {
                if (Datos.TipoComprobante.Trim().Length != 0 && Datos.TipoComprobante.Trim() != "0")
                {
                    Condiciones += " AND Tipo_Comprobante = '" + DepCom.Depurar(Datos.TipoComprobante.Trim()) + "'";
                }
            }

            //el status
            if (Datos.Status != null)
            {
                if (Datos.Status.Trim().Length != 0)
                {
                    Condiciones += " AND Status = '" + DepCom.Depurar(Datos.Status.Trim()) + "'";
                }
            }

            //El inicio
            if (Datos.Inicio != null)
            {
                if (Datos.Inicio.Trim().Length != 0)
                {
                    Condiciones += " AND Fecha_Emision >= '" + DepCom.Depurar(Datos.Inicio.Trim()) + "'";
                }
            }

            //Ahora el fin
            if (Datos.Fin != null)
            {
                if (Datos.Fin.Trim().Length != 0)
                {
                    Condiciones += " AND Fecha_Emision < DATEADD(DAY, 1, '" + DepCom.Depurar(Datos.Fin.Trim()) + "')";
                }
            }

            //El pais
            if (Datos.Pais != null)
            {
                if (Datos.Pais.Trim().Length != 0)
                {
                    Condiciones += " AND País = '" + DepCom.Depurar(Datos.Pais.Trim()) + "'";
                }
            }

            //Ahora el receptor
            String Receptor = "";
            if (Datos.Receptor != null)
            {
                if (Datos.Receptor.Trim().Length != 0 && Datos.Receptor.Trim() != "0")
                {
                    //Buscamos el rfc para ver que no sea extranjero
                    String QueryExtra = "SELECT TOP(1) Facturas.Id_Cliente, RFC FROM Facturas INNER JOIN Clientes ON Clientes.Id_Cliente = Facturas.Id_Cliente WHERE UPPER(Razon_Social) = '" + DepCom.Depurar(Datos.Receptor.Trim()) + "' GROUP BY Facturas.Id_Cliente, RFC;";
                    SqlCommand ComandoExtra = new SqlCommand(QueryExtra, conn, Transaccion);
                    SqlDataReader LectorExtra = ComandoExtra.ExecuteReader();
                    int IdClienteTemp = -1;
                    String RFCTemp = "";
                    if(LectorExtra.HasRows)
                    {
                        while(LectorExtra.Read())
                        {
                            IdClienteTemp = LectorExtra.GetInt32(0);
                            RFCTemp = LectorExtra.GetString(1);
                        }
                    }
                    LectorExtra.Close();

                    if (RFCTemp.EndsWith("010101000"))
                    {
                        Receptor = " AND Facturas.Razon_Social = '" + DepCom.Depurar(Datos.Receptor.Trim()) + "'";
                    }
                    else
                    {
                        Receptor = " AND Facturas.Id_Cliente = '" + IdClienteTemp + "'";
                    }
                }
            }


            if(Receptor.Length == 0)
            {
                //Si no hay receptor, entonces checo sus posibles
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

                //Si es master, no hago ni merga
                //Si es admin, agrego un dato nuevo
                if(Master == 1)
                {
                    Receptor = "";
                }
                else
                {
                    Receptor = " AND Facturas.Id_Empresa = (SELECT Usuarios.Id_Empresa FROM Usuarios WHERE Usuarios.Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "')";
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
                    for (int i = 0; i < ArregloRFC.Length; i++ )
                    {
                        if(RFC.Length == 0)
                        {
                            RFC = "'" + ArregloRFC[i] + "'";
                        }
                        else
                        {
                            RFC += ",'" + ArregloRFC[i] + "'";
                        }
                    }

                    Receptor = " AND Facturas.Serie IN (" + RFC + ")";
                }
            }

            Condiciones += Receptor + ";";


            String QueryObtener = "SELECT Id_Factura, Empresa.Razon_Social AS Emisor, Facturas.Razon_Social, Serie, Folio, Tipo_Comprobante, CONCAT(CONVERT(VARCHAR, Fecha_Emision, 103), ' ', CONVERT(VARCHAR, Fecha_Emision, 108)) , Condiciones_Pago, Metodo_Pago, Moneda, CAST(Tipo_Cambio AS VARCHAR), CAST(Sub_Total AS VARCHAR), CAST(IVA AS VARCHAR), CAST(Total AS VARCHAR), CAST(Facturas.Id_Cliente AS VARCHAR), Nota, Numero_Cliente, País, Enviada FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa INNER JOIN Clientes ON Clientes.Id_Cliente = Facturas.Id_Cliente" + Condiciones;
            SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
            SqlDataReader LectorObtener = ComandoObtener.ExecuteReader();
            if (LectorObtener.HasRows)
            {
                while (LectorObtener.Read())
                {
                    ObjetoFactura ObjFac = new ObjetoFactura();
                    ObjFac.IdFactura = LectorObtener.GetInt32(0);
                    ObjFac.Emisor = LectorObtener.GetString(1);
                    ObjFac.RazonSocial = LectorObtener.GetString(2);
                    ObjFac.Serie = LectorObtener.GetString(3);
                    ObjFac.Folio = LectorObtener.GetString(4);
                    ObjFac.TipoComprobante = LectorObtener.GetString(5);
                    ObjFac.FechaEmision = LectorObtener.GetString(6);
                    ObjFac.CondicionesPago = LectorObtener.GetString(7);
                    ObjFac.MetodoPago = LectorObtener.GetString(8);
                    ObjFac.Moneda = LectorObtener.GetString(9);
                    ObjFac.TipoCambio = LectorObtener.GetString(10);
                    ObjFac.SubTotal = LectorObtener.GetString(11);
                    ObjFac.IVA = LectorObtener.GetString(12);
                    ObjFac.Total = LectorObtener.GetString(13);
                    ObjFac.IdCliente = LectorObtener.GetString(14);
                    ObjFac.Nota = LectorObtener.GetString(15);
                    ObjFac.NumeroCliente = LectorObtener.GetString(16);
                    ObjFac.Pais = LectorObtener.GetString(17);
                    ObjFac.Enviada = LectorObtener.GetInt32(18);
                    Lista.Add(ObjFac);
                }
            }
            LectorObtener.Close();

            Transaccion.Commit();
        }
        catch
        {
            Transaccion.Rollback();
        }
        finally
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
        }

        return Lista;
    }

    public String FechaInicial()
    {
        String Retorno = "2016-01-01";
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();

            String Query = "SELECT  REPLACE(CONVERT(VARCHAR, GETDATE(), 102), '.', '-');";
            SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
            SqlDataReader Lector = Comando.ExecuteReader();
            if(Lector.HasRows)
            {
                while(Lector.Read())
                {
                    Retorno = Lector.GetString(0);
                }
            }
            Lector.Close();
            Transaccion.Commit();
            conn.Close();
        }
        catch
        {
            Retorno = "2016-01-01";
            try
            {
                Transaccion.Rollback();
                conn.Close();
            }
            catch
            {

            }
        }

        return Retorno;
    }
}