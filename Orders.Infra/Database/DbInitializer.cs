using Microsoft.EntityFrameworkCore;

namespace Orders.Infra.Database
{
    public static class DbInitializer
    {
        public static void InitializeContent(DbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
