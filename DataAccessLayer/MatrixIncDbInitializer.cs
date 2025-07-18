﻿using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    // Klasse voor het initialiseren en vullen van de database met testdata
    public static class MatrixIncDbInitializer
    {
        // Initialiseert de database en voegt standaarddata toe als deze nog leeg is
        public static void Initialize(MatrixIncDbContext context)
        {
            // Controleer of er al klanten zijn, zo ja: stop
            if (context.Customers.Any())
            {
                return;
            }
            
            // Voeg standaard klanten toe
            var customers = new Customer[]
            {
                new Customer { Name = "Neo", Address = "123 Elm St" , Active=true},
                new Customer { Name = "Morpheus", Address = "456 Oak St", Active = true },
                new Customer { Name = "Trinity", Address = "789 Pine St", Active = true }
            };
            context.Customers.AddRange(customers);

            // Voeg een standaard gebruiker toe
            var Users = new User { UserName = "Admin", Password = "A9jU6tAyZwvsi/WbeeBtCA==", Permissions=0 };
            context.Users.Add(Users);
            context.SaveChanges();

            // Voeg standaard orders toe (leeg in dit voorbeeld)
            var orders = new Order[]
            {
                
            };  
            context.Orders.AddRange(orders);

            // Voeg standaard producten toe
            var products = new Product[]
            {
                new Product { Name = "Nebuchadnezzar", Description = "Het schip waarop Neo voor het eerst de echte wereld leert kennen", Price = 10000.00m, Discount = 0.00m },
                new Product { Name = "Jack-in Chair", Description = "Stoel met een rugsteun en metalen armen waarin mensen zitten om ingeplugd te worden in de Matrix via een kabel in de nekpoort", Price = 500.50m, Discount = 1.00m },
                new Product { Name = "EMP (Electro-Magnetic Pulse) Device", Description = "Wapentuig op de schepen van Zion", Price = 129.99m, Discount = 0.00m }
            };
            context.Products.AddRange(products);

            // Voeg standaard onderdelen toe
            var parts = new Part[]
            {
                new Part { Name = "Tandwiel", Description = "Overdracht van rotatie in bijvoorbeeld de motor of luikmechanismen"},
                new Part { Name = "M5 Boutje", Description = "Bevestiging van panelen, buizen of interne modules"},
                new Part { Name = "Hydraulische cilinder", Description = "Openen/sluiten van zware luchtsluizen of bewegende onderdelen"},
                new Part { Name = "Koelvloeistofpomp", Description = "Koeling van de motor of elektronische systemen."}
            };
            context.Parts.AddRange(parts);

            // Voeg standaard logs toe
            var logs = new Log[]
            {
                new Log { Action = "Edit Product", Time = DateTime.Now, City = "Utrecht", User = "Admin01"},
                new Log { Action = "Delete Product", Time = DateTime.Now, City = "Maastricht", User = "Admin02" },
                new Log { Action = "Create Product", Time = DateTime.Now, City = "Rotterdam", User = "Admin02" },
            };
            context.Logs.AddRange(logs);

            context.SaveChanges();

            // Zorg dat de database is aangemaakt
            context.Database.EnsureCreated();
        }
    }
}
