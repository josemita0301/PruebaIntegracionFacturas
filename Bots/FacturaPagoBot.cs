using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaIntegracionFacturas.Bots
{
    public class FacturaPagoBot
    {
        private readonly string connectionString;
        private readonly string folderPath;

        public FacturaPagoBot(string connectionString, string folderPath)
        {
            this.connectionString = connectionString;
            this.folderPath = folderPath;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                if (DateTime.Now.Hour == 23 && DateTime.Now.Minute == 59)
                {
                    string todayFileName = $"facturas_{DateTime.Now:yyyy-MM-dd}.csv";
                    string filePath = Path.Combine(folderPath, todayFileName);

                    if (File.Exists(filePath))
                    {
                        await ProcessCsvFile(filePath);
                    }
                    else
                    {
                        Console.WriteLine("No se encontró un archivo para la fecha de hoy.");
                    }

                    await Task.Delay(TimeSpan.FromDays(1));
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
        }

        private async Task ProcessCsvFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',');

                    if (values.Length >= 4 &&
                        int.TryParse(values[0], out int proveedorId) &&
                        decimal.TryParse(values[1], out decimal monto) &&
                        DateTime.TryParseExact(values[2], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaPago) &&
                        Enum.TryParse(values[3], true, out EstadoPago estadoPago))
                    {
                        int facturaId = await InsertFactura(proveedorId, monto);
                        if (facturaId > 0)
                        {
                            await InsertPago(facturaId, fechaPago, estadoPago.ToString());
                        }
                    }
                }
            }

            Console.WriteLine($"Procesado el archivo {Path.GetFileName(filePath)}");
        }

        private async Task<int> InsertFactura(int proveedorId, decimal monto)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "INSERT INTO Facturas (ProveedorID, Monto, Estado) OUTPUT INSERTED.FacturaID VALUES (@ProveedorID, @Monto, 'Pendiente')";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProveedorID", proveedorId);
                command.Parameters.AddWithValue("@Monto", monto);

                await connection.OpenAsync();
                var facturaId = (int)await command.ExecuteScalarAsync();
                return facturaId;
            }
        }

        private async Task InsertPago(int facturaId, DateTime fechaPago, string estadoPago)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "INSERT INTO Pagos (FacturaID, FechaPago, Estado) VALUES (@FacturaID, @FechaPago, @Estado)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FacturaID", facturaId);
                command.Parameters.AddWithValue("@FechaPago", fechaPago);
                command.Parameters.AddWithValue("@Estado", estadoPago);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public enum EstadoPago
    {
        Pendiente,
        Completado
    }
}
