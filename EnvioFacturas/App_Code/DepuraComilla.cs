using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de DepuraComilla
/// </summary>
public class DepuraComilla
{
	public DepuraComilla()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public String Depurar(String Texto)
    {
        try
        {
            Texto = Texto.Replace("'", "''").Trim().ToUpper();
            return Texto;
        }
        catch
        {
            return Texto;
        }
    }

    public String DepurarSinMayus(String Texto)
    {
        try
        {
            Texto = Texto.Replace("'", "''").Trim();
            return Texto;
        }
        catch
        {
            return Texto;
        }
    }
}