using Microsoft.EntityFrameworkCore;

namespace eTerminiAPI.Data
{
    public class eTerminiDbContext : DbContext
    {
        public eTerminiDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        //qetu i shtojm DbSet-et per tabelat qe do te kemi ne database
    }
}
