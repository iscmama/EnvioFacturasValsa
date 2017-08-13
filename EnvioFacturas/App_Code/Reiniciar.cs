using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

/// <summary>
/// Descripción breve de Reiniciar
/// </summary>
public class Reiniciar
{
	public Reiniciar()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public String Reinicia()
    {
        String Error = "";
        try
        {
            Process[] myProcesses;
            // Regresa un arreglo con todas las instancias que coincidan con el nombre de aplicacion
            myProcesses = Process.GetProcessesByName("FacturasSandvik");
            foreach (Process myProcess in myProcesses)
            {
                myProcess.Kill();
            }
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\inetpub\wwwroot\EnvioFacturas\Program\FacturasSandvik.exe";
            proc.Start();
        }
        catch(Exception ex)
        {
            Error = ex.Message;
        }
        return Error;
    }
}