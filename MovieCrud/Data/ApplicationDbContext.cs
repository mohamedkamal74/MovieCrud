using Microsoft.EntityFrameworkCore;

namespace MovieCrud.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {

        }
    }
}
