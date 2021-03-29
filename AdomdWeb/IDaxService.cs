using System.Collections.Generic;
using Microsoft.AnalysisServices.AdomdClient;

public interface IDaxService
{
    void OpenConnection();
    void CloseConnection();
    AdomdDataReader GetDaxResult(string dax, List<AdomdParameter> daxParameters = null);
}