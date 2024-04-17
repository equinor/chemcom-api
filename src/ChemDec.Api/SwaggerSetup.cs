﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace ChemDec.Api
{
    public static class SwaggerSetup
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,

                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"https://login.microsoftonline.com/{configuration["azure:TenantId"]}/oauth2/v2.0/token"),
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{configuration["azure:TenantId"]}/oauth2/v2.0/authorize"),
                            Scopes = new Dictionary<string, string> {
                                { $"api://{configuration["azure:ClientId"]}/User.Impersonation", "Chemcom API" }
                            },
                        }
                    },
                    Description = "Chemcom Security Scheme"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "OAuth2",
                                    Type = ReferenceType.SecurityScheme
                                } ,
                                Scheme = "OAuth2",
                                BearerFormat = "JWT",
                                Type = SecuritySchemeType.Http,
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            }, new List<string> { $"{configuration["azure:ClientId"]}/User.Impersonation"}
                        }
                    });
            });
        }

        public static void Configure(IConfiguration configuration, IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chemcom API");
                c.OAuthClientId(configuration["azure:ClientId"]);
                c.OAuthAppName("Chemcom");
                c.OAuthScopeSeparator(" ");
                c.OAuthUsePkce();
            });
        }
    }
}
