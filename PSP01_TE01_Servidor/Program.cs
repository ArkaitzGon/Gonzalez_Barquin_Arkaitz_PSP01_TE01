/**
 * Programa Servidor
 * Carga un fichero xml de adivinanzas (el fichero debe estar en la carpeta del primer proyecto creado en este caso cliente. Tanto en release como en debug(net8.0))
 * Elige una al azar y envia el enunciado
 * Recibe la respuesta y comprueba si es correcta
 * Envia un ok si es correcto un KO si es incorrecto
 * si es incorrecta recibe otra respusta hasta que acierte
 */

using System.Diagnostics;
using System.IO.Pipes;
using System.Xml;


namespace pipeServidor
{
    class PipeServidor
    {
        static void Main(string[] args)
        { 
            try
            {
                // Cargamos el fichero con el metodo que hemos creado
                List<Adivinanza> lista = CargaFichero();
    
                // Creamos el pipe del servidor
                var server = new NamedPipeServerStream("PSP01_TE01");
                Console.WriteLine("Conexion a servidor establecida");

                // Esperamos a que el cliente contacte con el servidor
                server.WaitForConnection();
                

                // Creamos lo buffer
                StreamReader reader = new StreamReader(server);
                StreamWriter writer = new StreamWriter(server);
                Console.WriteLine("Pipe Servidor esperando datos.");

                
                while (true)
                {
                    Console.WriteLine("");
                    // Creamos el numero aleatorio y lo imprimimos
                    Random random = new Random();
                    int numeroAleatorio = random.Next(0, lista.Count);
                    Console.WriteLine(numeroAleatorio);

                    //Escribimos el enunciado en el writer
                    string enunciado = lista[numeroAleatorio].enunciado;
                    writer.WriteLine(enunciado);
                    Console.WriteLine("Enviado:P " + enunciado);
                    writer.Flush();

                    //Comprobamos si acierta
                    bool acierto = false;

                    while (acierto == false)
                    {
                        // Recibimos la respuesta del cliente
                        var respuesta = reader.ReadLine();
                        Console.WriteLine("Recibido:\n" + respuesta);
                        Console.WriteLine("RespuestaReal: " + lista[numeroAleatorio].respuesta);

                        // Comprobamos si las respuestas coinciden
                        if (respuesta.ToLower().Equals(lista[numeroAleatorio].respuesta.ToLower()))
                        {
                            // Mandamos un OK si acierta
                            writer.WriteLine("OK");
                            Console.WriteLine("Enviado:R OK");
                            
                            // Borramos adivinanza
                            lista.RemoveAt(numeroAleatorio);
                            Console.WriteLine("Borrando adivinanza de la lista: " + enunciado);
                            Console.WriteLine("Numero de adivinanzas disponibles en la lista: " + lista.Count());

                            acierto = true;
                            writer.Flush ();
                        }
                        else
                        {
                            // Mandamos un KO si no acierta
                            Console.WriteLine("Enviado:R KO");
                            writer.WriteLine("KO");
                            writer.Flush ();
                        }
                    }                 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0} Apagando el servidor por error", e.Message);
            }
            
        }
        /**
         * Metodo que carga las adivinanzas del XML
         * @return una list de adivinanzas
         */
        public static List<Adivinanza> CargaFichero()
        {
            List<Adivinanza> lista = new List<Adivinanza>();
            string posicion = "";
            string enunciado = "";
            string respuesta = "";
            
            // Cargamos el XML
            XmlDocument documento = new XmlDocument();
            documento.Load("adivinanzas.xml");

            // Nodo adivinanzaS
            foreach (XmlNode nodo1 in documento.ChildNodes)
            {
                if (nodo1.HasChildNodes)
                {
                    // Nodo adivinanza 
                    foreach (XmlNode nodo2 in nodo1.ChildNodes)
                    {
                        posicion = nodo2.Attributes["numero"].Value;
                        enunciado = nodo2["enunciado"].InnerText;
                        respuesta = nodo2["respuesta"].InnerText;
                        
                        Adivinanza adi = new Adivinanza(posicion,enunciado,respuesta);
                        lista.Add(adi);
                    }
                }
            }
            return lista;
        }
    }
    
    /**
     * Clase para gestionar las adivinanzas
     */
    public class Adivinanza
    {
        public string posicion;
        public string enunciado;
        public string respuesta;

        public Adivinanza(string posicion, string enunciado, string respuesta)
        {
            this.posicion = posicion;
            this.enunciado = enunciado;
            this.respuesta = respuesta; 
        }
    }

   
}