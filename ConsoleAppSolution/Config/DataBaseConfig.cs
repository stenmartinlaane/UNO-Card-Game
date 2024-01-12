namespace Config;

public class DataBaseConfig
{
    public static string GetDataBasePath() {
        var connectionString = "DataSource=<%tempPath%>uno.db;Cache=Shared";
        connectionString = connectionString.Replace("<%tempPath%>", Path.GetTempPath());
        return connectionString;
    }
}