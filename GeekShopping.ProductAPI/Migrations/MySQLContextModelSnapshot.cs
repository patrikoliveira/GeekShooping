﻿// <auto-generated />
using GeekShopping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GeekShopping.ProductAPI.Migrations
{
    [DbContext(typeof(MySQLContext))]
    partial class MySQLContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GeekShopping.ProductAPI.Model.Product", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("CategoryName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("category_name");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("description");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)")
                        .HasColumnName("image_url");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)")
                        .HasColumnName("price");

                    b.HasKey("Id");

                    b.ToTable("product");

                    b.HasData(
                        new
                        {
                            Id = 3L,
                            CategoryName = "T-Shirt",
                            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
                            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
                            Name = "Camisets Elon Musk",
                            Price = 69.9m
                        },
                        new
                        {
                            Id = 4L,
                            CategoryName = "T-Shirt",
                            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
                            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
                            Name = "Camiseta GNU Linux",
                            Price = 69.9m
                        },
                        new
                        {
                            Id = 5L,
                            CategoryName = "T-Shirt",
                            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
                            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
                            Name = "Camiseta Goku Fases",
                            Price = 69.9m
                        },
                        new
                        {
                            Id = 6L,
                            CategoryName = "T-Shirt",
                            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
                            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
                            Name = "Camiseta feminina Coffe Benefits",
                            Price = 69.9m
                        },
                        new
                        {
                            Id = 7L,
                            CategoryName = "Sweatshirt",
                            Description = "Lorem ipsum enim commodo habitant dapibus lobortis ac, mollis viverra lobortis donec risus elementum, laoreet aenean quisque tincidunt eget augue.",
                            ImageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/2_no_internet.jpg",
                            Name = "Moletom com Capuz Cobra Kai",
                            Price = 159.9m
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
