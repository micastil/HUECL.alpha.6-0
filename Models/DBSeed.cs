﻿using HUECL.alpha._6_0.Areas.Identity;
using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Areas.Identity.Pages;
using HUECL.alpha._6_0.Models.Projects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Security.Claims;

namespace HUECL.alpha._6_0.Models
{
    public static class DBSeed
    {
        public static void SeedCategories(IApplicationBuilder applicationBuilder)
        {
            AppDbContext context = applicationBuilder.ApplicationServices.
                CreateScope().ServiceProvider.
                GetRequiredService<AppDbContext>();

            if (context.Categories.Any() || context.SubCategories.Any())
            {
                return;
            }

            var categories = new Category[] {
                    new Category{ Name="Encoders", Code="5614", Active = 1},
                    new Category{ Name = "Motores Electricos", Code = "5624", Active = 1}
                };

            foreach (Category item in categories)
            {
                context.Categories.Add(item);
            }

            context.SaveChanges();

            var subcategories = new SubCategory[]{
                    new SubCategory{ Active = 1, Name = "Incremental Hollow Shaft", Code = "561401", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Incremental Solid Shaft", Code = "561402", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Magnetico", Code = "561403", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Absoluto", Code = "561404", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Universal", Code = "561406", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Overspeed", Code = "561408", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Tacho Generador", Code = "561409", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Accesorio Mecanico", Code = "561414", CategoryId = categories[0].Id },
                    new SubCategory{ Active = 1, Name = "Accesorio Electronico", Code = "561415", CategoryId = categories[0].Id },
                };

            foreach (SubCategory item in subcategories)
            {
                context.SubCategories.Add(item);
            }

            context.SaveChanges();
        }

        public static void SeedBase(IApplicationBuilder applicationBuilder)
        {
            AppDbContext context = applicationBuilder.ApplicationServices.
                CreateScope().ServiceProvider.
                GetRequiredService<AppDbContext>();

            if (!context.Units.Any())
            {
                context.Units.AddRange(
                    new Unit { Name = "Unidad", Active = 1, Code = "UN" },
                    new Unit { Name = "Kit", Active = 1, Code = "Kit" });
            }

            if (!context.Currencies.Any())
            {
                context.Currencies.AddRange(
                    new Currency { Name = "Peso Chileno", Code = "CLP" }
                    );
            }

            if (!context.Providers.Any())
            {
                context.Providers.AddRange(
                    new Provider { Name = "Johannes Huebner Giessen", Active = 1 },
                    new Provider { Name = "VEM Motors", Active = 1 }
                    );
            }

            if (!context.Countries.Any())
            {
                var countries = new Country[]
                {
                    new Country{ Name = "Chile", Code = "CL"},
                    new Country{ Name = "Peru", Code = "PE"}
                };

                foreach (Country country in countries)
                {
                    context.Countries.Add(country);
                }

                context.SaveChanges();

                if (!context.Customers.Any())
                {
                    context.Customers.AddRange(
                        new Customer
                        {
                            Active = Active.Active,
                            Name = "Collahuasi",
                            CreationDate = DateTime.Today,
                            TaxId = "89.468.900-5",
                            Address = "Avda. Baquedano 902, Iquique, Chile",
                            CountryId = countries[0].Id
                        },
                        new Customer
                        {
                            Active = Active.Active,
                            Name = "Minera Los Pelambres",
                            CreationDate = DateTime.Today,
                            TaxId = "96.790.240-3",
                            Address = "Av Apoquindo 4001 Of 1802, Santiago, Chile",
                            CountryId = countries[0].Id
                        }
                        );
                }

                context.SaveChanges();
            }

            if (!context.ProjectStatuses.Any()) 
            {
                context.ProjectStatuses.AddRange(
                    new ProjectStatus { Name = "Conceptual Engineering", Active = true },
                    new ProjectStatus { Name = "Basic Engineering", Active = true },
                    new ProjectStatus { Name = "Detailed Engineering", Active = true },
                    new ProjectStatus { Name = "Construction", Active = true },
                    new ProjectStatus { Name = "Finished", Active = true }
                    );
                context.SaveChanges();
            }

            if(!context.ProjectSectors.Any()) 
            {
                context.ProjectSectors.AddRange(
                    new ProjectSector { Name = "Mining", Active = true },
                    new ProjectSector { Name = "Port", Active = true },
                    new ProjectSector { Name = "Industry", Active = true },
                    new ProjectSector { Name = "Energy", Active = true },
                    new ProjectSector { Name = "Steel", Active = true },
                    new ProjectSector { Name = "Pulp and Paper", Active = true },
                    new ProjectSector { Name = "Cement", Active = true }
                    );
                context.SaveChanges();
            }

            if (!context.InteractionTypes.Any()) 
            {
                context.InteractionTypes.AddRange(
                    new InteractionType { Name = "Meeting", Active = true },
                    new InteractionType { Name = "Email", Active = true },
                    new InteractionType { Name = "Call", Active = true },
                    new InteractionType { Name = "WhatsApp", Active = true },
                    new InteractionType { Name = "Technical Visit", Active = true },
                    new InteractionType { Name = "Training Session", Active = true }
                    );
                context.SaveChanges();
            }

            if (context.ProjectDocumentTypes.Any()) 
            {
                context.ProjectDocumentTypes.AddRange(
                    new ProjectDocumentType { Name = "Quotation", Active = true },
                    new ProjectDocumentType { Name = "Invoice", Active = true },
                    new ProjectDocumentType { Name = "Delivery", Active = true },
                    new ProjectDocumentType { Name = "Technical Report", Active = true }
                    );
                context.SaveChanges();
            }
        }

        public static async Task SeedSuperUser(IApplicationBuilder applicationBuilder, IConfiguration configurationApp)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope()) 
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var secretPass = configurationApp["ExternalPassword"];

                if (!await roleManager.RoleExistsAsync("SuperAdministrator"))
                {
                    var role = new IdentityRole("SuperAdministrator");
                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync("Manager"))
                {
                    var role = new IdentityRole("Manager");
                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync("Sales"))
                {
                    var role = new IdentityRole("Sales");
                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync("Guest"))
                {
                    var role = new IdentityRole("Guest");
                    await roleManager.CreateAsync(role);
                }

                if (await userManager.FindByNameAsync("super@user.admin") == null)
                {
                    var superUser = new ApplicationUser
                    {
                        UserName = "super@user.admin",
                        Email = "super@user.admin",
                    };

                    var result = await userManager.CreateAsync(superUser, secretPass);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superUser, "SuperAdministrator");

                        await userManager.AddClaimAsync(superUser, new Claim(GlobalPermissionType.CanRead, "True"));
                        await userManager.AddClaimAsync(superUser, new Claim(GlobalPermissionType.CanWrite, "True"));
                        await userManager.AddClaimAsync(superUser, new Claim(GlobalPermissionType.CanDelete, "True"));
                    }
                    else
                    {
                        throw new ApplicationException("Error creating SuperUser.");
                    }
                }
            }
        }
    }
}
