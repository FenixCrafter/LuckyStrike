namespace WebApi.Handlers;

public class FileHandler
{
    private readonly IConfiguration _configuration;
    public FileHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetProductImagePath(string productName)
    {
        var path = _configuration.GetValue<string>("StorageLocation");
        path += "\\" + productName + ".png";
        return path;
    }
}
