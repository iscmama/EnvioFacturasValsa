using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// Descripción breve de MandarCorreo
/// </summary>
public class MandarCorreo
{
	public MandarCorreo()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public Boolean Manda(String IdFactura)
    {
        Boolean Mando = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();


            //Primero obtengo las credenciales
            String Correo = "";
            String Contraseña = "";
            String Host = "";
            String Puerto = "";
            Boolean SSL = false;
            String QueryCredenciales = "SELECT Correo, Contraseña, Host, Puerto, SSL FROM Correo WHERE Activo = 1;";
            SqlCommand ComandoCredenciales = new SqlCommand(QueryCredenciales, conn, Transaccion);
            SqlDataReader LectorCredenciales = ComandoCredenciales.ExecuteReader();
            if(LectorCredenciales.HasRows)
            {
                while(LectorCredenciales.Read())
                {
                    Correo = LectorCredenciales.GetString(0);
                    Contraseña = LectorCredenciales.GetString(1);
                    Host = LectorCredenciales.GetString(2);
                    Puerto = LectorCredenciales.GetString(3);
                    SSL = LectorCredenciales.GetBoolean(4);
                }
            }
            else
            {
                Mando = false;
            }
            LectorCredenciales.Close();

            if(Mando)
            {
                String XML = "";
                MailMessage email = new MailMessage();
                //Ahora obtengo los correos del cliente
                String QueryCorreos = "SELECT Correo, Ruta_XML FROM Facturas INNER JOIN CorreosCliente ON CorreosCliente.Id_Cliente = Facturas.Id_Cliente WHERE Id_Factura = '" + DepCom.Depurar(IdFactura) + "' AND CorreosCliente.Activo = 1  AND CorreosCliente.Numero_Cliente = (SELECT Numero_Cliente FROM Facturas WHERE Facturas.Id_Factura = '" + DepCom.Depurar(IdFactura) + "');";
                SqlCommand ComandoCorreos = new SqlCommand(QueryCorreos, conn, Transaccion);
                SqlDataReader LectorCorreos = ComandoCorreos.ExecuteReader();
                if(LectorCorreos.HasRows)
                {
                    while(LectorCorreos.Read())
                    {
                        email.To.Add(new MailAddress(LectorCorreos.GetString(0)));
                        XML = LectorCorreos.GetString(1);
                    }
                }
                LectorCorreos.Close();

                String Texto = "";
                String QueryTexto = "SELECT Texto FROM Correo;";
                SqlCommand ComandoTexto = new SqlCommand(QueryTexto, conn, Transaccion);
                SqlDataReader LectorTexto = ComandoTexto.ExecuteReader();
                if(LectorTexto.HasRows)
                {
                    while(LectorTexto.Read())
                    {
                        Texto = LectorTexto.GetString(0);
                    }
                }
                LectorTexto.Close();

                String[] ArregloFactura = XML.Split('\\');
                                
                email.From = new MailAddress(Correo);
                email.Subject = "Reenvio de Factura " + ArregloFactura[ArregloFactura.Length - 1].Replace(".xml", "");
                //Adjunto los archivos
                //email.Attachments.Add(new Attachment(XML));
                //email.Attachments.Add(new Attachment(XML.Replace(".xml", ".pdf")));
                email.Body = Texto;
                email.IsBodyHtml = false;
                email.Priority = MailPriority.Normal;

                //Definimos credenciales
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Host;
                smtp.Port = Convert.ToInt32(Puerto);
                smtp.EnableSsl = SSL;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(Correo, Contraseña);

                smtp.Send(email);

                //eliminamos el objeto
                smtp.Dispose();

                //Ahora marcamos como enviado
                String Query = "UPDATE Facturas SET Enviada = 1 where Id_Factura = '" +  IdFactura + "';";
                SqlCommand Comando = new SqlCommand(Query, conn);
                conn.Open();
                Comando.ExecuteNonQuery();
                conn.Close();
            }

            Transaccion.Commit();
            conn.Close();
        }
        catch(Exception ex)
        {
            Mando = false;
            Transaccion.Rollback();
            conn.Close();
        }

        return Mando;
    }

    public String MandaCorreo(String IdFactura)
    {
        String Error = "";
        Boolean Mando = true;

        DepuraComilla DepCom = new DepuraComilla();
        SqlConnection conn = new SqlConnection(Conexiones.CONEXION);
        SqlTransaction Transaccion = null;

        try
        {
            conn.Open();
            Transaccion = conn.BeginTransaction();


            //Primero obtengo las credenciales
            String Correo = "";
            String Contraseña = "";
            String Host = "";
            String Puerto = "";
            Boolean SSL = false;
            String QueryCredenciales = "SELECT Correo, Contraseña, Host, Puerto, SSL FROM Correo WHERE Activo = 1;";
            SqlCommand ComandoCredenciales = new SqlCommand(QueryCredenciales, conn, Transaccion);
            SqlDataReader LectorCredenciales = ComandoCredenciales.ExecuteReader();
            if (LectorCredenciales.HasRows)
            {
                while (LectorCredenciales.Read())
                {
                    Correo = LectorCredenciales.GetString(0);
                    Contraseña = LectorCredenciales.GetString(1);
                    Host = LectorCredenciales.GetString(2);
                    Puerto = LectorCredenciales.GetString(3);
                    SSL = LectorCredenciales.GetBoolean(4);
                }
            }
            else
            {
                Mando = false;
            }
            LectorCredenciales.Close();

            if (Mando)
            {
                String XML = "";
                MailMessage email = new MailMessage();
                //Ahora obtengo los correos del cliente
                String QueryCorreos = "SELECT Correo, Ruta_XML FROM Facturas INNER JOIN CorreosCliente ON CorreosCliente.Id_Cliente = Facturas.Id_Cliente WHERE Id_Factura = '" + DepCom.Depurar(IdFactura) + "' AND CorreosCliente.Activo = 1  AND CorreosCliente.Numero_Cliente = (SELECT Numero_Cliente FROM Facturas WHERE Facturas.Id_Factura = '" + DepCom.Depurar(IdFactura) + "');";
                SqlCommand ComandoCorreos = new SqlCommand(QueryCorreos, conn, Transaccion);
                SqlDataReader LectorCorreos = ComandoCorreos.ExecuteReader();
                if (LectorCorreos.HasRows)
                {
                    while (LectorCorreos.Read())
                    {
                        email.To.Add(new MailAddress(LectorCorreos.GetString(0)));
                        XML = LectorCorreos.GetString(1);
                    }
                }
                LectorCorreos.Close();

                String Texto = "";
                String QueryTexto = "SELECT Texto FROM Correo;";
                SqlCommand ComandoTexto = new SqlCommand(QueryTexto, conn, Transaccion);
                SqlDataReader LectorTexto = ComandoTexto.ExecuteReader();
                if (LectorTexto.HasRows)
                {
                    while (LectorTexto.Read())
                    {
                        Texto = LectorTexto.GetString(0);
                    }
                }
                LectorTexto.Close();

                String[] ArregloFactura = XML.Split('\\');

                email.From = new MailAddress(Correo);
                email.Subject = "Reenvio de Factura " + ArregloFactura[ArregloFactura.Length - 1].Replace(".xml", "");
                //Adjunto los archivos
                //email.Attachments.Add(new Attachment(XML));
                //email.Attachments.Add(new Attachment(XML.Replace(".xml", ".pdf")));
                email.Body = Texto;
                email.IsBodyHtml = false;
                email.Priority = MailPriority.Normal;

                //Definimos credenciales
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Host;
                smtp.Port = Convert.ToInt32(Puerto);
                smtp.EnableSsl = SSL;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(Correo, Contraseña);

                smtp.Send(email);

                //eliminamos el objeto
                smtp.Dispose();

                //Ahora marcamos como enviado
                String Query = "UPDATE Facturas SET Enviada = 1 where Id_Factura = '" + IdFactura + "';";
                SqlCommand Comando = new SqlCommand(Query, conn, Transaccion);
                Comando.ExecuteNonQuery();
            }
            else
            {
                Error = "No Mando";
            }

            Transaccion.Commit();
            conn.Close();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            Mando = false;
            Transaccion.Rollback();
            conn.Close();
        }

        return Error;
    }

}