using LeMarconnes.Entities;
using System.Collections.Generic;

namespace LeMarconnes.Services
{
    public interface IJwtService
    {
        string CreateToken(User user, IList<string> roles);
    }
}
