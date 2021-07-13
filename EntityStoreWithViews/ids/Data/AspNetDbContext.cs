using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerHost.Data
{
    public class AspNetDbContext : IdentityDbContext
    {
        public AspNetDbContext(DbContextOptions<AspNetDbContext> options) : base(options)
        {

        }
    }
}
