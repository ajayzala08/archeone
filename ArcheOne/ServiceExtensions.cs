using ArcheOne.Helper;

namespace ArcheOne
{
    public static class ServiceExtensions
    {
        public static void DIScopes(this IServiceCollection services)
        {
            //Helpers
            services.AddScoped<DbRepo>();
        }
    }
}
