using PruebaIntegracionFacturas.Bots;
using PruebaIntegracionFacturas.Generators;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string connectionString = "Server=JOSEMOSPC\\SQLEXPRESS;Database=FACTURACION;Trusted_Connection=True;";

        //Variables bots
        string csvOrigenFolder = @"C:\Users\josem\Desktop\FacturasOrigen";
        string csvDestinoFolder = @"C:\Users\josem\Desktop\FacturasDestino";
        TimeSpan executionTime = new TimeSpan(20, 23, 00);

        CsvGenerator generator = new CsvGenerator(csvOrigenFolder);
        generator.GenerateCsv();

    }
}