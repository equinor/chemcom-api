using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users;

public class User
{
    public User()
    {
        Roles = new List<Role>();
    }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Role> Roles { get; set; }
    public string Upn { get; set; }
}
