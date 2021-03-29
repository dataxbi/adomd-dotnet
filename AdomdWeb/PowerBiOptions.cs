public class PowerBiOptions
{
    public const string PowerBi = "PowerBi";

    public string Server { get; set; }

    public string Database { get; set; }

    public string User { get; set; }

    public string Password { get; set; }

    public string ConnectionString => $"Provider=MSOLAP;Data Source={Server};Initial Catalog={Database};User ID={User};Password={Password}";
}
