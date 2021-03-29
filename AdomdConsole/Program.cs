using System;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;

namespace AdomdConsole
{
    class Program
    {
        static AdomdConnection _connection = null;

        static void Main(string[] args)
        {
            OpenConnection(GetConnectionString());

            string dax = "EVALUATE 'Comunidades Autonomas'";
            PrintDaxResultSchema(dax);
            Console.WriteLine();
            PrintDaxResult(dax);
            Console.WriteLine();

            dax = "EVALUATE {[Unidades Vendidas]}";
            PrintDaxResult(dax);
            Console.WriteLine();
            Console.WriteLine($"Unidades vendidas: {GetDaxResultAsLong(dax)}");
            Console.WriteLine();

            dax = "EVALUATE {CALCULATE([Unidades Vendidas],'Comunidades Autonomas'[Codigo CA] = \"ES-CT\")}";
            PrintDaxResult(dax);
            Console.WriteLine();
            Console.WriteLine($"Unidades vendidas en Cataluña: {GetDaxResultAsLong(dax)}");
            Console.WriteLine();

            CloseConnection();
        }

        static string GetConnectionString()
        {
            var powerBiServer = Environment.GetEnvironmentVariable("PBI_SERVER");
            var powerBiDb = Environment.GetEnvironmentVariable("PBI_DB");
            var powerBiUser = Environment.GetEnvironmentVariable("PBI_USER");
            var powerBiPassword = Environment.GetEnvironmentVariable("PBI_PASSWORD");
            Console.WriteLine($"Servidor: {powerBiServer}");
            Console.WriteLine($"Conjunto de datos: {powerBiDb}");
            Console.WriteLine();
            return $"Provider=MSOLAP;Data Source={powerBiServer};Initial Catalog={powerBiDb};User ID={powerBiUser};Password={powerBiPassword};";
        }

        static void OpenConnection(string connectionString)
        {
            _connection = new AdomdConnection(connectionString);
            _connection.Open();
        }

        static void CloseConnection()
        {
            _connection.Close();
        }

        static long GetDaxResultAsLong(string daxExpression)
        {
            long value = 0;
            var reader = GetDaxResult(daxExpression);
            if (reader.Read())
                value = reader.GetInt64(0);
            reader.Close();
            return value;
        }
        static AdomdDataReader GetDaxResult(string daxExpression)
        {
            var command = _connection.CreateCommand();
            command.CommandText = daxExpression;
            return command.ExecuteReader();
        }

        static void PrintDaxResult(string daxExpression)
        {
            var reader = GetDaxResult(daxExpression);
            var isFirstRow = true;
            while (reader.Read())
            {
                if (isFirstRow)
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetName(i)}\t");
                    }
                    Console.WriteLine();
                    isFirstRow = false;
                }
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader[i].ToString()}\t");
                }
                Console.WriteLine();
            }
            reader.Close();
        }

        static void PrintDaxResultSchema(string daxExpression)
        {
            var reader = GetDaxResult(daxExpression);
            var schemaTable = reader.GetSchemaTable();
            foreach (DataRow row in schemaTable.Rows)
            {
                foreach (DataColumn column in schemaTable.Columns)
                {
                    Console.WriteLine(column.ColumnName + " = " + row[column]);
                }
                Console.WriteLine();
            }
            reader.Close();
        }

    }
}
