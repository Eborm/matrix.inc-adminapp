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

public class LogsRepository : ILogsRepository
{
    private readonly MatrixIncDbContext _context;

    public LogsRepository(MatrixIncDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Log> GetAllLogs()
    {
        return _context.Logs.AsEnumerable();
    }

    public IEnumerable<Log>? GetLogByID(int id)
    {
        return _context.Logs.AsEnumerable().Where(log => log.Id == id);
    }

    public void AddLog(Log log)
    {
        log.City = GetCityByIP().ToString();
        
        _context.Logs.Add(log);
        _context.SaveChanges();
    }

    public IEnumerable<Log>? GetLogByAction(string action)
    {
        return _context.Logs.AsEnumerable().Where(log => log.Action == action);
    }
    
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
            
            return city;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij het ophalen van locatie: {ex.Message}");
            return "failed to get city";
        }
    }
}