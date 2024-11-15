﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VehicleRegistration.Infrastructure.Data;

#nullable disable

namespace VehicleRegistration.Migrations
{
    [DbContext(typeof(VehicleRegistrationContext))]
    [Migration("20241113150303_SeedAdministrator")]
    partial class SeedAdministrator
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("VehicleRegistration.Domain.Entities.Administrator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Profile")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Administrators");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@teste.com",
                            Password = "admin",
                            Profile = "admin"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
