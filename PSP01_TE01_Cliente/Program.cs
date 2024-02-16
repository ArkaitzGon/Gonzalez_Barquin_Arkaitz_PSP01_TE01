/**
 * Programa cliente
 * Recibe un enunciado
 * Mandamos una respuesta
 * Recibimos si hemos acertado, si hemos fallado volvemos a mandar una respuesta
 */

using System.Diagnostics;
using System.IO.Pipes;

namespace pipeCliente
{
    class PipeCliente
    {
        static void Main(string[] args)
        {
            Process p;

            // Lanzamos el proceso del servidor con el metodo que hemos creado
            LanzaServidor(out p);

            // Dejamos un tiempo prudencial de espera antes de hacer cualquier calculo
            Task.Delay(1000).Wait();

            // Preparamos el pipe del cliente para la conexion al pipe del servidor
            var cliente = new NamedPipeClientStream("PSP01_TE01");

            try
            {
                // Conectamos con el servidor
                cliente.Connect(1000);
                Console.WriteLine("Estableciendo conexion con el servidor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Se produjo un error al intentar conectar con el servidor: {ex.Message}");
            }
            
            Console.WriteLine("********************");
            Console.WriteLine("*Comienza el juego*");
            Console.WriteLine("********************");

            // Creamos los buffers
            StreamReader reader = new StreamReader(cliente);
            StreamWriter writer = new StreamWriter(cliente);

            
            // Para que el programa este continuamente escuchando y leyendo datos, creamos un bucle infinito
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("Resuelve el siguiente acertijo:\n");
                Console.WriteLine("*****************Acertijo********************");

                // Recibimos el enunciado
                var enunciado = reader.ReadLine();
                Console.WriteLine(enunciado);
                Console.WriteLine("*********************************************");
                Console.WriteLine("Indica una respuesta:");

                // Comprobamos si ha acertado, si no se ha acertado repetimos
                bool acierto = false;
                while (!acierto)
                {
                    // Mandamos la respuesta
                    writer.WriteLine(Console.ReadLine());
                    writer.Flush();

                    // Recibimos si hemos acertado o no
                    var respuesta = reader.ReadLine();
                    if (respuesta.Equals("OK"))
                    {
                        Console.WriteLine("¡Enhorabuena! Has acertado.");
                        Console.WriteLine("---------***-----------");
                        Console.WriteLine("************************");
                        Console.WriteLine("Jugar otra vez:");
                        Console.WriteLine("************************");
                        acierto = true;
                    }
                    else if (respuesta.Equals("KO"))
                    {
                        Console.WriteLine("No has acertado. Introduce otra respuesta:");
                    }
                }
                
            }
        }

        /**
         * Metodo que lanza el proceso del servidor
         * @return Process con el proceso del servidor
         */
        static Process LanzaServidor(out Process p1)
        {
            //Iniciamos un proceso con el servidor
            ProcessStartInfo info = new ProcessStartInfo(@"..\..\..\..\PSP01_TE01_Servidor\bin\Release\net8.0\win-x64\PSP01_TE01_Servidor.exe");
            
            info.CreateNoWindow = false;
            info.WindowStyle = ProcessWindowStyle.Normal;

            info.UseShellExecute = true;

            // Inicia el proceso del servidor 
            p1 = Process.Start(info);
            return p1;
        }
    }
}

