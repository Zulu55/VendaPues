﻿using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Orders.Backend.Helpers;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;
using Orders.Shared.Enums;

namespace Orders.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly IRuntimeInformationWrapper _runtimeInformationWrapper;

        public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork, IFileStorage fileStorage, IRuntimeInformationWrapper runtimeInformationWrapper)
        {
            _context = context;
            _usersUnitOfWork = usersUnitOfWork;
            _fileStorage = fileStorage;
            _runtimeInformationWrapper = runtimeInformationWrapper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesFullAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckRolesAsync();
            await CheckProductsAsync();
            await CheckUserAsync("0001", "Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "JuanZuluaga.jpg", UserType.Admin);
            await CheckUserAsync("0002", "Ledys", "Bedoya", "ledys@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "LedysBedoya.jpg", UserType.User);
            await CheckUserAsync("0003", "Brad", "Pitt", "brad@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "Brad.jpg", UserType.User);
            await CheckUserAsync("0004", "Angelina", "Jolie", "angelina@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "Angelina.jpg", UserType.User);
            await CheckUserAsync("0005", "Bob", "Marley", "bob@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "bob.jpg", UserType.User);
            await CheckUserAsync("0006", "Celia", "Cruz", "celia@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "celia.jpg", UserType.Admin);
            await CheckUserAsync("0007", "Fredy", "Mercury", "fredy@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "fredy.jpg", UserType.User);
            await CheckUserAsync("0008", "Hector", "Lavoe", "hector@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "hector.jpg", UserType.User);
            await CheckUserAsync("0009", "Liv", "Taylor", "liv@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "liv.jpg", UserType.User);
            await CheckUserAsync("0010", "Otep", "Shamaya", "otep@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "otep.jpg", UserType.User);
            await CheckUserAsync("0011", "Ozzy", "Osbourne", "ozzy@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "ozzy.jpg", UserType.User);
            await CheckUserAsync("0012", "Selena", "Quintanilla", "selenba@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "selena.jpg", UserType.User);
        }

        private async Task CheckCountriesFullAsync()
        {
            if (!_context.Countries.Any())
            {
                var countriesStatesCitiesSQLScript = File.ReadAllText("Data\\CountriesStatesCities.sql");
                await _context.Database.ExecuteSqlRawAsync(countriesStatesCitiesSQLScript);
            }
        }

        private async Task CheckRolesAsync()
        {
            await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
            await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, string image, UserType userType)
        {
            var user = await _usersUnitOfWork.GetUserAsync(email);
            if (user == null)
            {
                var city = await _context.Cities.FirstOrDefaultAsync(x => x.Name == "Medellín");
                city ??= await _context.Cities.FirstOrDefaultAsync();

                string filePath;
                if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = $"{Environment.CurrentDirectory}\\Images\\users\\{image}";
                }
                else
                {
                    filePath = $"{Environment.CurrentDirectory}/Images/users/{image}";
                }

                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "users");

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = city,
                    UserType = userType,
                    Photo = imagePath,
                };

                await _usersUnitOfWork.AddUserAsync(user, "123456");
                await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

                var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
                await _usersUnitOfWork.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Apple" });
                _context.Categories.Add(new Category { Name = "Autos" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Comida" });
                _context.Categories.Add(new Category { Name = "Cosmeticos" });
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Gamer" });
                _context.Categories.Add(new Category { Name = "Jugetes" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                _context.Categories.Add(new Category { Name = "Nutrición" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Tecnología" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Adidas Barracuda", 270000M, ["Calzado", "Deportes"], ["adidas_barracuda.png"], 230000M, 0.3F);
                await AddProductAsync("Adidas Superstar", 250000M, ["Calzado", "Deportes"], ["Adidas_superstar.png"], 200000M, 0.3F);
                await AddProductAsync("Aguacate", 5000M, ["Comida"], ["Aguacate1.png", "Aguacate2.png", "Aguacate3.png"], 4000M, 0.3F);
                await AddProductAsync("AirPods", 1300000M, ["Tecnología", "Apple"], ["airpos.png", "airpos2.png"], 1000000M, 0.3F);
                await AddProductAsync("Akai APC40 MKII", 2650000M, ["Tecnología"], ["Akai1.png", "Akai2.png", "Akai3.png"], 2150000M, 0.3F);
                await AddProductAsync("Apple Watch Ultra", 4500000M, ["Apple", "Tecnología"], ["AppleWatchUltra1.png", "AppleWatchUltra2.png"], 4000000M, 0.3F);
                await AddProductAsync("Audifonos Bose", 870000M, ["Tecnología"], ["audifonos_bose.png"], 800000M, 0.3F);
                await AddProductAsync("Bicicleta Ribble", 12000000M, ["Deportes"], ["bicicleta_ribble.png"], 10000000M, 0.3F);
                await AddProductAsync("Camisa Cuadros", 56000M, ["Ropa"], ["camisa_cuadros.png"], 50000M, 0.3F);
                await AddProductAsync("Casco Bicicleta", 820000M, ["Deportes"], ["casco_bicicleta.png", "casco.png"], 750000M, 0.3F);
                await AddProductAsync("Gafas deportivas", 160000M, ["Deportes"], ["Gafas1.png", "Gafas2.png", "Gafas3.png"], 130000M, 0.3F);
                await AddProductAsync("Hamburguesa triple carne", 25500M, ["Comida"], ["Hamburguesa1.png", "Hamburguesa2.png", "Hamburguesa3.png"], 16500M, 0.3F);
                await AddProductAsync("iPad", 2300000M, ["Tecnología", "Apple"], ["ipad.png"], 200000M, 0.3F);
                await AddProductAsync("iPhone 13", 5200000M, ["Tecnología", "Apple"], ["iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png"], 4900000M, 0.3F);
                await AddProductAsync("Johnnie Walker Blue Label 750ml", 1266700M, ["Licores"], ["JohnnieWalker3.png", "JohnnieWalker2.png", "JohnnieWalker1.png"], 1000000M, 0.3F);
                await AddProductAsync("KOOY Disfraz inflable de gallo para montar", 150000M, ["Juguetes"], new List<string>() { "KOOY1.png", "KOOY2.png", "KOOY3.png" }, 100000M, 0.3F);
                await AddProductAsync("Mac Book Pro", 12100000M, ["Tecnología", "Apple"], ["mac_book_pro.png"], 11500000M, 0.3F);
                await AddProductAsync("Mancuernas", 370000M, ["Deportes"], ["mancuernas.png"], 300000M, 0.3F);
                await AddProductAsync("Mascarilla Cara", 26000M, ["Belleza"], ["mascarilla_cara.png"], 20000M, 0.3F);
                await AddProductAsync("New Balance 530", 180000M, ["Calzado", "Deportes"], ["newbalance530.png"], 140000M, 0.3F);
                await AddProductAsync("New Balance 565", 179000M, ["Calzado", "Deportes"], ["newbalance565.png"], 155000M, 0.3F);
                await AddProductAsync("Nike Air", 233000M, ["Calzado", "Deportes"], ["nike_air.png"], 200000M, 0.3F);
                await AddProductAsync("Nike Zoom", 249900M, ["Calzado", "Deportes"], ["nike_zoom.png"], 200000M, 0.3F);
                await AddProductAsync("Buso Adidas Mujer", 134000M, ["Ropa", "Deportes"], ["buso_adidas.png"], 100000M, 0.3F);
                await AddProductAsync("Suplemento Boots Original", 15600M, ["Nutrición"], ["Boost_Original.png"], 150000M, 0.3F);
                await AddProductAsync("Whey Protein", 252000M, ["Nutrición"], ["whey_protein.png"], 200000M, 0.3F);
                await AddProductAsync("Arnes Mascota", 25000M, ["Mascotas"], ["arnes_mascota.png"], 20000M, 0.3F);
                await AddProductAsync("Cama Mascota", 99000M, ["Mascotas"], ["cama_mascota.png"], 78000M, 0.3F);
                await AddProductAsync("Teclado Gamer", 67000M, ["Gamer", "Tecnología"], ["teclado_gamer.png"], 53000M, 0.3F);
                await AddProductAsync("Ring de Lujo 17", 1600000M, ["Autos"], ["Ring1.png", "Ring2.png"], 1350000M, 0.3F);
                await AddProductAsync("Silla Gamer", 980000M, ["Gamer", "Tecnología"], ["silla_gamer.png"], 715000M, 0.3F);
                await AddProductAsync("Mouse Gamer", 132000M, ["Gamer", "Tecnología"], ["mouse_gamer.png"], 99900M, 0.3F);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddProductAsync(string name, decimal price, List<string> categories, List<string> images, decimal cost, float desiredProfit)
        {
            Product prodcut = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Cost = cost,
                DesiredProfit = desiredProfit,
                ProductCategories = [],
                ProductImages = []
            };

            foreach (var categoryName in categories)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
                if (category != null)
                {
                    prodcut.ProductCategories.Add(new ProductCategory { Category = category });
                }
            }

            foreach (string? image in images)
            {
                string filePath;
                if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = $"{Environment.CurrentDirectory}\\Images\\products\\{image}";
                }
                else
                {
                    filePath = $"{Environment.CurrentDirectory}/Images/products/{image}";
                }

                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "products");
                prodcut.ProductImages.Add(new ProductImage { Image = imagePath });
            }

            _context.Products.Add(prodcut);
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _ = _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States =
                    [
                        new State()
                        {
                            Name = "Antioquia",
                            Cities = [
                                new() { Name = "Medellín" },
                                new() { Name = "Itagüí" },
                                new() { Name = "Envigado" },
                                new() { Name = "Bello" },
                                new() { Name = "Rionegro" },
                                new() { Name = "Marinilla" },
                            ]
                        },
                        new State()
                        {
                            Name = "Bogotá",
                            Cities = [
                                new() { Name = "Usaquen" },
                                new() { Name = "Champinero" },
                                new() { Name = "Santa fe" },
                                new() { Name = "Useme" },
                                new() { Name = "Bosa" },
                            ]
                        },
                    ]
                });
                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States =
                    [
                        new State()
                        {
                            Name = "Florida",
                            Cities = [
                                new() { Name = "Orlando" },
                                new() { Name = "Miami" },
                                new() { Name = "Tampa" },
                                new() { Name = "Fort Lauderdale" },
                                new() { Name = "Key West" },
                            ]
                        },
                        new State()
                        {
                            Name = "Texas",
                            Cities = [
                                new() { Name = "Houston" },
                                new() { Name = "San Antonio" },
                                new() { Name = "Dallas" },
                                new() { Name = "Austin" },
                                new() { Name = "El Paso" },
                            ]
                        },
                    ]
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}