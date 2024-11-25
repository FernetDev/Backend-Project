﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebAPI_Log.Context;

#nullable disable

namespace WebAPI_Log.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebAPI_Log.Entities.Cliente", b =>
                {
                    b.Property<int>("IdCliente")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCliente"));

                    b.Property<int>("ContactNro")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EstaPagada")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("FechaIngreso")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("FechaPago")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaVencimiento")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdPerfil")
                        .HasColumnType("int");

                    b.Property<string>("NombreCompleto")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdCliente");

                    b.HasIndex("IdPerfil");

                    b.ToTable("Cliente", (string)null);
                });

            modelBuilder.Entity("WebAPI_Log.Entities.Perfil", b =>
                {
                    b.Property<int>("IdPerfil")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPerfil"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdPerfil");

                    b.ToTable("Perfil", (string)null);

                    b.HasData(
                        new
                        {
                            IdPerfil = 1,
                            Nombre = "Alumno Normal"
                        },
                        new
                        {
                            IdPerfil = 2,
                            Nombre = "Alumno Personalizado"
                        },
                        new
                        {
                            IdPerfil = 3,
                            Nombre = "Alumno PowerLifter"
                        });
                });

            modelBuilder.Entity("WebAPI_Log.Entities.Usuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuario"));

                    b.Property<string>("Clave")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Correo")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Nombre")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("IdUsuario")
                        .HasName("PK__Usuario__5B65BF97B07C57D5");

                    b.ToTable("Usuario", (string)null);
                });

            modelBuilder.Entity("WebAPI_Log.Entities.Cliente", b =>
                {
                    b.HasOne("WebAPI_Log.Entities.Perfil", "PerfilReferencia")
                        .WithMany("ClientesReferencia")
                        .HasForeignKey("IdPerfil")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PerfilReferencia");
                });

            modelBuilder.Entity("WebAPI_Log.Entities.Perfil", b =>
                {
                    b.Navigation("ClientesReferencia");
                });
#pragma warning restore 612, 618
        }
    }
}
