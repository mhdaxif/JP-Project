﻿using System;
using System.Reflection;
using IdentityServer4.EntityFramework.Options;
using Jp.Infra.CrossCutting.Identity.Context;
using Jp.Infra.CrossCutting.Identity.Entities.Identity;
using Jp.Infra.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jp.Infra.Migrations.Sql.Identity.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentitySqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_DATABASE_CONNECTION") ?? configuration.GetConnectionString("SSOConnection");
            var migrationsAssembly = typeof(IdentityConfig).GetTypeInfo().Assembly.GetName().Name;

            var operationalStoreOptions = new OperationalStoreOptions();
            services.AddSingleton(operationalStoreOptions);

            var storeOptions = new ConfigurationStoreOptions();
            services.AddSingleton(storeOptions);

            services.AddEntityFrameworkSqlServer().AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
            services.AddDbContext<JpContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
            services.AddDbContext<EventStoreSQLContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            services.AddIdentity<UserIdentity, UserIdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
