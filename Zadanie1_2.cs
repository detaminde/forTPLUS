using Npgsql;
using System.Diagnostics;
const string CONNECT_STR = "Host=localhost;Port=5432;Database=testTPLUS;User Id=postgres;Password=detam";

NpgsqlConnection nc = new NpgsqlConnection(CONNECT_STR);

try
{
    nc.Open();
    List<Thermal> items = GetAllIdFromTable("troubleshooting_cost_node", nc);
    nc.Close();
    if (items.Count > 0)
    {
        for(int i = 0, e = items.Count; i < e; ++i)
        {
            StartProcessForItem(items[i]);
            if(i < e - 1) Thread.Sleep(1000*120);
        }
    }
}
catch (Exception ex) { Console.WriteLine(ex.ToString()); }
//____________________________________________________

static List<Thermal> GetAllIdFromTable(string tableName, NpgsqlConnection nc)
{
    List<Thermal> ids = new List<Thermal>();
    NpgsqlCommand command = new NpgsqlCommand($"SELECT * FROM {tableName}", nc);
    NpgsqlDataReader reader = command.ExecuteReader();
    if (reader.HasRows)
    {
        while (reader.Read())
        {
            Thermal th = new(reader.GetInt32(reader.GetOrdinal("id")), 
                             reader.GetString(reader.GetOrdinal("name")),
                             reader.GetDouble(reader.GetOrdinal("cost")));
            ids.Add(th);
        }
        reader.Close();
    }
    return ids;
}
static void StartProcessForItem(Thermal th)
{
    Process process = Process.Start("publish/ConsoleApp1.exe", $"{th.Id} LogFile{th.Id}");
    string wrText = $"{DateTime.Now}: внешний процесс запущен для id={th.Id} name={th.Name}\n";
    File.WriteAllText("LogFileCA", wrText);
    Console.WriteLine(wrText);
    try
    {
        process.WaitForExit();
        wrText = $"Внешний процесс завершен для id={th.Id} name={th.Name}\n";
        File.WriteAllText("LogFileCA", wrText);
        Console.WriteLine(wrText);
    }
    catch(Exception ex) 
    {
        wrText = $"Внешний процесс завершен для id={th.Id} name={th.Name}\n" + ex.Message;
        File.WriteAllText("LogFileCA", wrText);
        Console.WriteLine(wrText); 
    }
    finally { process.Dispose(); }
}
class Thermal
{
    public Thermal(int id, string name, double cost)
    {
        Id = id;
        Name = name;
        Cost = cost;
    }
    public Thermal() { }
    public int Id = -1;
    public string Name = "unknown";
    public double Cost = -1;
};