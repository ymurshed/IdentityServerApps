using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ids.Data
{
    public class AspNetDbContext : IdentityDbContext
    {
        public AspNetDbContext(DbContextOptions<AspNetDbContext> options) : base(options)
        {

        }
    }
}
