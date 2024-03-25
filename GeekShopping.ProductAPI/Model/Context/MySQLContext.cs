using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Model.Context;

public class MySQLContext : DbContext
{
    public MySQLContext() { }

    public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(new Product
        {
            Id = 3,
            Name = "Camisets Elon Musk",
            Price = new decimal(69.9),
            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
            CategoryName = "T-Shirt"
        });
        modelBuilder.Entity<Product>().HasData(new Product
        {
            Id = 4,
            Name = "Camiseta GNU Linux",
            Price = new decimal(69.9),
            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
            CategoryName = "T-Shirt"
        });
        modelBuilder.Entity<Product>().HasData(new Product
        {
            Id = 5,
            Name = "Camiseta Goku Fases",
            Price = new decimal(69.9),
            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
            CategoryName = "T-Shirt"
        });
        modelBuilder.Entity<Product>().HasData(new Product
        {
            Id = 6,
            Name = "Camiseta feminina Coffe Benefits",
            Price = new decimal(69.9),
            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
            CategoryName = "T-Shirt"
        });
        modelBuilder.Entity<Product>().HasData(new Product
        {
            Id = 7,
            Name = "Moletom com Capuz Cobra Kai",
            Price = new decimal(159.9),
            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
            CategoryName = "Sweatshirt"
        });
    }
}

