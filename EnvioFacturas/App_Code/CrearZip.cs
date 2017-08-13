using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ionic.Zip;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de CrearZip
/// </summary>
public class CrearZip
{
	public CrearZip()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public String Crea(String Lista)
    {
        String Retorno;
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;
        try
        {
            String Nombre = NombrarArchivo();

            using (ZipFile zip = new ZipFile())
            {
                conn.Open();
                Transaccion = conn.BeginTransaction();

                String Query = "SELECT Ruta_PDF, Ruta_XML FROM Facturas WHERE Id_Factura IN(" + Lista + ");";
                SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
                SqlDataReader Lector = Comando.ExecuteReader();
                if(Lector.HasRows)
                {
                    while(Lector.Read())
                    {
                        zip.AddFile(Lector.GetString(0), "");
                        zip.AddFile(Lector.GetString(1), "");
                    }
                }
                Lector.Close();

                Transaccion.Commit();
                conn.Close();

                zip.Save(HttpContext.Current.Server.MapPath("Zip/" + Nombre + ".zip"));
                //zip.Save(@"C:\inetpub\wwwroot\EnvioFacturas\Zip\" + Nombre + ".zip");
            }

            Retorno = "1," + Nombre;
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