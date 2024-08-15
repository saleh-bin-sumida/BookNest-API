
namespace RepositoryWithUWO.Core.Helper;

public class JwtOptions
{
    public string  Issure { get; set; }

    public string Audience { get; set; }
    public int LifeTime { get; set; }

    public string SigningKey { get; set; }
}
