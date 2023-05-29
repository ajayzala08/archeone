<<<<<<< HEAD
﻿using ArcheOne.Helper;
=======
﻿using ArcheOne.Helper.CommonHelpers;
>>>>>>> cd527d961c63f0609fca23c3d960641591127856

namespace ArcheOne
{
    public static class ServiceExtensions
    {
        public static void DIScopes(this IServiceCollection services)
        {
            //Helpers
            services.AddScoped<DbRepo>();
<<<<<<< HEAD
        }
    }
=======
            services.AddScoped<CommonConstant>();
            services.AddScoped<CommonHelper>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		}
	}
>>>>>>> cd527d961c63f0609fca23c3d960641591127856
}
