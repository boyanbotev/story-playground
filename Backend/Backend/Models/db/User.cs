using Microsoft.AspNetCore.Identity;

namespace Backend.Models.Db;
public class User: IdentityUser
{    
    public List<Story> Stories { get; set; }
}