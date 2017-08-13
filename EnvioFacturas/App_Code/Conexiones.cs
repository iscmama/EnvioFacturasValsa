using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// Descripción breve de Conexiones
/// </summary>
public class Conexiones
{
	public Conexiones()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public static String CONEXION = ConfigurationManager.ConnectionStrings["dbEnvioFacturas"].ConnectionString;
}