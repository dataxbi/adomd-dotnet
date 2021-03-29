using System.Collections.Generic;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.Extensions.Options;

public class DaxService : IDaxService
{
    private readonly PowerBiOptions _powerBiOptions;
    private AdomdConnection _connection;

    public DaxService(IOptions<PowerBiOptions> powerBiOptions)
    {
        _powerBiOptions = powerBiOptions.Value;
    }

    public void OpenConnection()
    {
        _connection = new AdomdConnection(_powerBiOptions.ConnectionString);
        _connection.Open();
    }

    public void CloseConnection()
    {
        _connection.Close();
    }

    public AdomdDataReader GetDaxResult(string dax, List<AdomdParameter> daxParameters = null)
    {
        var command = _connection.CreateCommand();
        command.CommandText = dax;

        if (daxParameters != null)
        {
            foreach (var p in daxParameters)
            {
                command.Parameters.Add(new AdomdParameter(p.ParameterName, p.Value));
            }
        }
        return command.ExecuteReader();
    }
}