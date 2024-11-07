using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace PruebaIntegracionFacturas.Generators
{

    public class CsvGenerator
    {
        private readonly string outputFolderPath;

        public CsvGenerator(string outputFolderPath)
        {
            this.outputFolderPath = outputFolderPath;
        }

        public void GenerateCsv()
        {
            var facturas = GenerateFakeData();

            string fileName = $"facturas_{DateTime.Now:yyyy-MM-dd}.csv";
            string filePath = Path.Combine(outputFolderPath, fileName);

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(facturas);
            }

            Console.WriteLine($"Archivo CSV generado en: {filePath}");
        }

        private List<FacturaRecord> GenerateFakeData()
        {
            var facturas = new List<FacturaRecord>();
            var estados = new[] { "Pendiente", "Pagada", "Erronea" };
            var random = new Random();

            for (int i = 1; i <= 10; i++) // Generar 10 registros ficticios
            {
                facturas.Add(new FacturaRecord
                {
                    FacturaID = i,
                    ProveedorID = random.Next(1, 5), // ProveedorID entre 1 y 4
                    Monto = (decimal)Math.Round(random.Next(100, 5000) + random.NextDouble(), 2), // Monto entre 100 y 5000
                    Estado = estados[random.Next(estados.Length)], // Estado aleatorio
                    FechaCreacion = DateTime.Now.AddDays(-random.Next(1, 30)) // Fecha aleatoria en los últimos 30 días
                });
            }

            return facturas;
        }
    }

    public class FacturaRecord
    {
        public int FacturaID { get; set; }
        public int ProveedorID { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
