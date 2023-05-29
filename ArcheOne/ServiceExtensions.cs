using ArcheOne.Helper.CommonHelpers;

namespace ArcheOne
{
    public static class ServiceExtensions
    {
        public static void DIScopes(this IServiceCollection services)
        {
            //Helpers
            services.AddScoped<DbRepo>();
            services.AddScoped<CommonConstant>();
            services.AddScoped<CommonHelper>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		}
	}
}
