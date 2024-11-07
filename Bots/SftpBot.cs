using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
namespace PruebaIntegracionFacturas.Bots
{
    public class SftpBot
    {
        private readonly string sourceFolderPath;
        private readonly string destinationFolderPath;
        private readonly TimeSpan executionTime;

        public SftpBot(string sourceFolderPath, string destinationFolderPath, TimeSpan executionTime)
        {
            this.sourceFolderPath = sourceFolderPath;
            this.destinationFolderPath = destinationFolderPath;
            this.executionTime = executionTime;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                var now = DateTime.Now;
                var nextExecution = new DateTime(now.Year, now.Month, now.Day, executionTime.Hours, executionTime.Minutes, 0);

                // Si la hora de ejecución ya pasó hoy, agendar para el día siguiente
                if (now > nextExecution)
                {
                    nextExecution = nextExecution.AddDays(1);
                }

                var delay = nextExecution - now;
                Console.WriteLine($"Esperando hasta la hora de ejecución: {nextExecution}");
                await Task.Delay(delay);

                // Ejecutar el movimiento de archivo
                MoveFile();
            }
        }

        private void MoveFile()
        {
            var todayFileName = $"facturas_proveedor_{DateTime.Now:yyyy-MM-dd}.csv";
            var sourceFilePath = Path.Combine(sourceFolderPath, todayFileName);
            var destinationFilePath = Path.Combine(destinationFolderPath, todayFileName);

            if (File.Exists(sourceFilePath))
            {
                Directory.CreateDirectory(destinationFolderPath); // Asegura que la carpeta de destino existe
                File.Move(sourceFilePath, destinationFilePath);
                Console.WriteLine($"Archivo '{todayFileName}' movido a {destinationFolderPath}.");
            }
            else
            {
                Console.WriteLine($"El archivo '{todayFileName}' no se encontró en la carpeta de origen.");
            }
        }
    }
}
