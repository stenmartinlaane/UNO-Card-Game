namespace Config;

public class DataBaseConfig
{
    public static string GetDataBasePath() {
        var connectionString = "DataSource=<%tempPath%>school.db;Cache=Shared";
        connectionString = connectionString.Replace("<%tempPath%>", Path.GetTempPath());
        return connectionString;
    }
}