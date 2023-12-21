using TorgovayaPloshadka.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace TorgovayaPloshadka.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            if (context.Categories.Any())
            {
                return;
            }

            Category category = new Category
            {
                CategoryName = "Категория1",
                Description = "",
                Products_Count = 0
            };
            context.Categories.Add(category);
            context.SaveChanges();

            Category category1 = new Category
            {
                CategoryName = "Категория2",
                Description = "",
                Products_Count = 0
            };
            context.Categories.Add(category1);
            context.SaveChanges();

            Customer customer = new Customer
            {
                Surname = "Александрова",
                Name = "Александра",
                Midname = "Александровна",
                Phone = "89001001010",
                Address = "ул. Соколова, 6"
            };
            context.Customers.Add(customer);
            context.SaveChanges();

            Customer customer1 = new Customer
            {
                Surname = "Васильева",
                Name = "Вера",
                Midname = "Васильевна",
                Phone = "89001002020",
                Address = "ул. Соколова, 7"
            };
            context.Customers.Add(customer1);
            context.SaveChanges();

            Manufacturer manufacturer = new Manufacturer
            {
                Title = "Производитель1",
                Address = "ул. Соколова, 6",
                Phone = "89001001010",
                Surname = "Александрова",
                Name = "Александра",
                Midname = "Александровна"
            };
            context.Manufacturers.Add(manufacturer);
            context.SaveChanges();

            Manufacturer manufacturer1 = new Manufacturer
            {
                Title = "Производитель2",
                Address = "ул. Соколова, 7",
                Phone = "89001002020",
                Surname = "Васильева",
                Name = "Вера",
                Midname = "Васильевна"
            };
            context.Manufacturers.Add(manufacturer1);
            context.SaveChanges();

            Supplier supplier = new Supplier
            {
                Title = "Поставщик1",
                Address = "ул. Соколова, 6",
                Phone = "89001001010",
                Surname = "Александрова",
                Name = "Александра",
                Midname = "Александровна"
            };
            context.Suppliers.Add(supplier);
            context.SaveChanges();

            Supplier supplier1 = new Supplier
            {
                Title = "Поставщик2",
                Address = "ул. Соколова, 7",
                Phone = "89001002020",
                Surname = "Васильева",
                Name = "Вера",
                Midname = "Васильевна"
            };
            context.Suppliers.Add(supplier1);
            context.SaveChanges();

            Product product = new Product
            {
                ProductName = "Товар1",
                CategoryId = category.CategoryId,
                ManufacturerId = manufacturer.ManufacturerId,
                SupplierId = supplier.SupplierId,
                Price = 1500,
                Weight = 500,
                Proportions = "7*15*20"
            };
            context.Products.Add(product);
            context.SaveChanges();

            Product product1 = new Product
            {
                ProductName = "Товар2",
                CategoryId = category1.CategoryId,
                ManufacturerId = manufacturer1.ManufacturerId,
                SupplierId = supplier1.SupplierId,
                Price = 1500,
                Weight = 500,
                Proportions = "7*15*20"
            };
            context.Products.Add(product1);
            context.SaveChanges();

            Order order = new Order
            {
                CustomerId = customer.CustomerId,
                ProductId = product.ProductId,
                Date_Ordered = new DateTime(2023, 12, 22, 18, 0, 0),
                Count = 1
            };
            context.Orders.Add(order);
            context.SaveChanges();

            Doljnost doljnost1 = new Doljnost
            {
                DoljnostName = "Администратор"
            };
            context.Doljnosts.Add(doljnost1);

            Doljnost doljnost2 = new Doljnost
            {
                DoljnostName = "Сотрудник"
            };
            context.Doljnosts.Add(doljnost2);

            context.SaveChanges();

            string[] roles = new string[] { "Administrator", "Guest" };
            foreach (string role in roles)
            {
                CreateRole(serviceProvider, role);
            }

            CustomUser customUser1 = new() { Surname = "Alekseev", Name = "Aleksei", Secsurname = "Alekseevich", UserName = "alekseev@mail.ru", Email = "alekseev@mail.ru", DoljnostId = doljnost1.DoljnostId };

            AddUserToRole(serviceProvider, "Password123!", "Administrator", customUser1);

            CustomUser customUser2 = new() { Surname = "Ivanov", Name = "Ivan", Secsurname = "Ivanovich", UserName = "ivanov@mail.ru", Email = "ivanov@mail.ru", DoljnostId = doljnost2.DoljnostId };

            AddUserToRole(serviceProvider, "Password123!", "Guest", customUser2);
        }

        private static void CreateRole(IServiceProvider serviceProvider, string roleName)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            Task<bool> roleExists = roleManager.RoleExistsAsync(roleName);
            roleExists.Wait();

            if (!roleExists.Result)
            {
                Task<IdentityResult> roleResult = roleManager.CreateAsync(new IdentityRole(roleName));
                roleResult.Wait();
            }

        }

        private static void AddUserToRole(IServiceProvider serviceProvider, string userPwd, string roleName, CustomUser customUser)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<CustomUser>>();

            Task<CustomUser> checkAppUser = userManager.FindByEmailAsync(customUser.Email); ;

            checkAppUser.Wait();

            if (checkAppUser.Result == null)
            {
                Task<IdentityResult> taskCreateAppUser = userManager.CreateAsync(customUser, userPwd);

                taskCreateAppUser.Wait();

                if (taskCreateAppUser.Result.Succeeded)
                {
                    Task<IdentityResult> newUserRole = userManager.AddToRoleAsync(customUser, roleName);
                    newUserRole.Wait();
                }
            }
        }
    }
}
