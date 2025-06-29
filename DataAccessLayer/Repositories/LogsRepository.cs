using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http.Headers;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories;

// Repository voor het beheren en ophalen van logregels uit de database
public class LogsRepository : ILogsRepository
{
    // Database context voor toegang tot logs
    private readonly MatrixIncDbContext _context;

    // Constructor voor dependency injection van de context
    public LogsRepository(MatrixIncDbContext context)
    {
        _context = context;
    }
    
    // Haalt alle logregels op
    public IEnumerable<Log> GetAllLogs()
    {
        return _context.Logs.AsEnumerable();
    }

    // Haalt logregels op basis van een ID
    public IEnumerable<Log>? GetLogByID(int id)
    {
        return _context.Logs.AsEnumerable().Where(log => log.Id == id);
    }

    // Voegt een nieuwe logregel toe aan de database (asynchroon)
    public async Task AddLog(Log log)
    {
        log.City = await GetCityByIP();
        
        _context.Logs.Add(log);
        _context.SaveChanges();
    }

    // Haalt logregels op basis van een actie
    public IEnumerable<Log>? GetLogByAction(string action)
    {
        return _context.Logs.AsEnumerable().Where(log => log.Action == action);
    }
    
    // Haalt de stad op van het huidige IP-adres via een externe API
    private static async Task<string> GetCityByIP()
    {
        string apiUrl = "https://ipwhois.app/json/";

        using HttpClient client = new HttpClient();

        try
        {
            // Maak een GET-aanroep naar de gratis API
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            // Lees de JSON-respons
            string responseBody = await response.Content.ReadAsStringAsync();

            // JSON-parser
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

            // Haal de gewenste gegevens op
            string city = data.city;
            Console.WriteLine(city);
            
            
            return city;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij het ophalen van locatie: {ex.Message}");
            return "failed to get city";
        }
    }
}