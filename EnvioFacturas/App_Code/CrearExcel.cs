using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// Descripción breve de CrearExcel
/// </summary>
public class CrearExcel
{
	public CrearExcel()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public String CrearArchivo(List<String> ListaEntrada, String IdUsuario)
    {
        String Retorno = "";
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        DepuraComilla DepCom = new DepuraComilla();
        try
        {
            String ContenidoArchivo = "";
            //Nombro el archivo
            String NombreArchivo = this.NombrarArchivo();
            //Verifico si el archivo se nombro correctamente
            if (NombreArchivo.Length != 0)
            {
                //Iniciamos con los titulos
                ContenidoArchivo += "RFC Receptor";
                ContenidoArchivo += ",RFC Emisor";
                ContenidoArchivo += ",Razon Social del Receptor";
                ContenidoArchivo += ",N° Interno del Receptor";
                ContenidoArchivo += ",Serie";
                ContenidoArchivo += ",Folio";
                ContenidoArchivo += ",Tipo comprobante";
                ContenidoArchivo += ",Folio Fiscal UUID";
                ContenidoArchivo += ",Fecha de timbrado";
                ContenidoArchivo += ",Moneda";
                ContenidoArchivo += ",Total Bruto";
                ContenidoArchivo += ",Valor IVA";
                ContenidoArchivo += ",Total Neto";
                ContenidoArchivo += ",País";


                //Aqui inicio Querys
                conn.Open();
                Transaccion = conn.BeginTransaction();

                //Checamos si hay serie
                String Condiciones = " WHERE 1 = 1";
                if (ListaEntrada[0] != null)
                {
                    if (ListaEntrada[0].Trim().Length != 0)
                    {
                        Condiciones += " AND Serie = '" + DepCom.Depurar(ListaEntrada[0].Trim()) + "'";
                    }
                }
                //Checo si hay folio
                if (ListaEntrada[1] != null)
                {
                    if (ListaEntrada[1].Trim().Length != 0)
                    {
                        Condiciones += " AND Folio = '" + DepCom.Depurar(ListaEntrada[1].Trim()) + "'";
                    }
                }
                //Ahora el tipo de comprobante
                if (ListaEntrada[2] != null)
                {
                    if (ListaEntrada[2].Trim().Length != 0 && ListaEntrada[2].Trim() != "0")
                    {
                        Condiciones += " AND Tipo_Comprobante = '" + DepCom.Depurar(ListaEntrada[2].Trim()) + "'";
                    }
                }

                //el status
                if (ListaEntrada[6] != null)
                {
                    if (ListaEntrada[6].Trim().Length != 0)
                    {
                        Condiciones += " AND Status = '" + DepCom.Depurar(ListaEntrada[6].Trim()) + "'";
                    }
                }

                //El pais
                if (ListaEntrada[7] != null)
                {
                    if (ListaEntrada[7].Trim().Length != 0)
                    {
                        Condiciones += " AND País = '" + DepCom.Depurar(ListaEntrada[7].Trim()) + "'";
                    }
                }

                //El inicio
                if (ListaEntrada[3] != null)
                {
                    if (ListaEntrada[3].Trim().Length != 0)
                    {
                        Condiciones += " AND Fecha_Emision >= '" + DepCom.Depurar(ListaEntrada[3].Trim()) + "'";
                    }
                }

                //Ahora el fin
                if (ListaEntrada[4] != null)
                {
                    if (ListaEntrada[4].Trim().Length != 0)
                    {
                        Condiciones += " AND Fecha_Emision < DATEADD(DAY, 1, '" + DepCom.Depurar(ListaEntrada[4].Trim()) + "')";
                    }
                }

                //Ahora el receptor
                String Receptor = "";
                if (ListaEntrada[5] != null)
                {
                    if (ListaEntrada[5].Trim().Length != 0 && ListaEntrada[5].Trim() != "0")
                    {
                        Receptor = " AND Facturas.Razon_Social = '" + DepCom.Depurar(ListaEntrada[5].Trim()) + "'";
                    }
                }

                if (Receptor.Length == 0)
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
                    if (Master == 1)
                    {
                        Receptor = "";
                    }
                    else /*if (Admin == 1)
                    {
                        Receptor = " AND Facturas.Id_Empresa = (SELECT Usuarios.Id_Empresa FROM Usuarios WHERE Usuarios.Id_Usuario = '" + DepCom.DepurarSinMayus(IdUsuario) + "')";
                    }
                    else*/
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

                        Receptor = " AND Facturas.Serie IN (" + RFC + ")";
                    }
                }

                Condiciones += Receptor + ";";

                String QueryObtener = @"SELECT (SELECT TOP(1) Clientes.RFC FROM Facturas WHERE Facturas.Id_Cliente = Clientes.Id_Cliente GROUP BY Razon_Social ORDER BY COUNT(*) DESC) AS RFC_Receptor, Empresa.RFC AS RFC_Emisor, Facturas.Razon_Social, Facturas.Numero_Cliente, Serie, Folio, Tipo_Comprobante, REPLACE((REVERSE(SUBSTRING(REVERSE(Ruta_PDF),0,CHARINDEX('\',REVERSE(Ruta_PDF))))), '.PDF', '') AS UUID, CONCAT(CONVERT(VARCHAR, Fecha_Emision, 103), ' ', CONVERT(VARCHAR, Fecha_Emision, 108)) AS Fecha_Timbrado, Moneda, CAST(Sub_Total AS VARCHAR) AS SubTotal, CAST(IVA AS VARCHAR) AS IVA, CAST(Total AS VARCHAR) AS Total, País FROM Facturas INNER JOIN Empresa ON Empresa.Id_Empresa = Facturas.Id_Empresa INNER JOIN Clientes ON Clientes.Id_Cliente = Facturas.Id_Cliente" + Condiciones;
                SqlCommand ComandoObtener = new SqlCommand(QueryObtener, conn, Transaccion);
                SqlDataReader LectorObtener = ComandoObtener.ExecuteReader();
                if (LectorObtener.HasRows)
                {
                    while (LectorObtener.Read())
                    {
                        ContenidoArchivo += "\n" +LectorObtener.GetString(0);
                        ContenidoArchivo += "," + LectorObtener.GetString(1);
                        if (LectorObtener.GetString(2).Contains(","))
                        {
                            ContenidoArchivo += ",\"" + LectorObtener.GetString(2) + "\" ";
                        }
                        else
                        {
                            ContenidoArchivo += "," + LectorObtener.GetString(2);
                        }
                        ContenidoArchivo += "," + LectorObtener.GetString(3);
                        ContenidoArchivo += "," + LectorObtener.GetString(4);
                        ContenidoArchivo += "," + LectorObtener.GetString(5);
                        ContenidoArchivo += "," + LectorObtener.GetString(6);
                        ContenidoArchivo += "," + LectorObtener.GetString(7);
                        ContenidoArchivo += "," + LectorObtener.GetString(8);
                        if (LectorObtener.GetString(9).Contains(","))
                        {
                            ContenidoArchivo += ",\"" + LectorObtener.GetString(9) + "\"";
                        }
                        else
                        {
                            ContenidoArchivo += "," + LectorObtener.GetString(9);
                        }
                        ContenidoArchivo += "," + LectorObtener.GetString(10);
                        ContenidoArchivo += "," + LectorObtener.GetString(11);
                        ContenidoArchivo += "," + LectorObtener.GetString(12);
                        ContenidoArchivo += "," + LectorObtener.GetString(13);
                    }
                }
                LectorObtener.Close();

                Transaccion.Commit();
                conn.Close();

                //Guardo archivo
                var archivo = @"C:\inetpub\wwwroot\EnvioFacturas\Excel\" + NombreArchivo + ".csv";
                
                using (var fileStream = File.Create(archivo))
                {
                    var texto = new System.Text.UTF8Encoding(true).GetBytes(ContenidoArchivo);
                    fileStream.Write(texto, 0, texto.Length);
                    fileStream.Flush();
                }


                Retorno = "1," + NombreArchivo;
            }
            else
            {
                Retorno = "0,Error ocurrido al nombrar archivo";
            }
        }
        catch(Exception ex)
        {
            Retorno = "0," + ex.Message;
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

    private String NombrarArchivo()
    {
        String Retorno = "";
        try
        {
            Random Ran = new Random();
            String Posibles = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int LongitudMaxima = Posibles.Length;
            char Letra;
            int LongitudNuevaCadena = 20;
            for (int i = 0; i < LongitudNuevaCadena; i++)
            {
                Letra = Posibles[Ran.Next(LongitudMaxima)];
                Retorno += Letra.ToString();
            }
        }
        catch
        {
            Retorno = "";
        }
        return Retorno;
    }
}