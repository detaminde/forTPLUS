using Npgsql;

// args[0] - id; args[1] - путь к логу
const string CONNECT_STR = "Host=localhost;Port=5432;Database=testTPLUS;User Id=postgres;Password=detam";
if (args.Length == 0) 
{
    Console.WriteLine("Не введены параметры для запуска");
    return;
}
int id = int.Parse(args[0]);
string logFilePath = args.Length == 1 ? $"LogFile" : args[1];
File.AppendAllText(logFilePath, $"Start programm: [{DateTime.Now}]\n");
NpgsqlConnection nc = new NpgsqlConnection(CONNECT_STR);

try
{
    nc.Open();
    double originalCost = ReadOriginalCostFromDB(id, nc);
    Random rnd = new Random();
    double newCost = originalCost + rnd.NextDouble() * (20000.0 - 5000.0) + 5000.0;
    if (UpdateCostInDB(id, nc, newCost))
        File.AppendAllText(logFilePath, $"[{DateTime.Now}] Node ID: {id}, Original Cost: {originalCost}, New Cost: {newCost}\n");

}
catch(Exception ex) 
{
    string errorMessage = $"An error occurred: {ex.Message}\n";
    Console.WriteLine(errorMessage);
    File.AppendAllText(logFilePath, errorMessage);
}
finally { nc.Close(); }

//_______________________________________
static double ReadOriginalCostFromDB(int id, NpgsqlConnection nc)
{
    double cost = 0;

    NpgsqlCommand command = new NpgsqlCommand($"select * from troubleshooting_cost_node where id = @Id", nc);
    command.Parameters.AddWithValue("Id", id);
    NpgsqlDataReader reader = command.ExecuteReader();

    if (reader.HasRows)
    {
        if (reader.Read())
            cost = reader.GetDouble(reader.GetOrdinal("cost"));
        reader.Close();
    }

    return cost;
}

static bool UpdateCostInDB(int id, NpgsqlConnection nc, double newCost)
{
    NpgsqlCommand command = new NpgsqlCommand
($"update troubleshooting_cost_node set cost = @COST where id = @ID", nc);
    command.Parameters.AddWithValue("ID", id);
    command.Parameters.AddWithValue("COST", newCost);

    return command.ExecuteNonQuery() > 0;
}