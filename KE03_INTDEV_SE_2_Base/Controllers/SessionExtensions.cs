using Microsoft.AspNetCore.Http;
using System.Text.Json;

// Extensie methoden voor het werken met sessie gegevens
// Maakt het mogelijk om complexe objecten op te slaan en op te halen uit de sessie
public static class SessionExtensions
{
    // Slaat een object op in de sessie als JSON string
    // T: Het type van het object dat opgeslagen wordt
    // key: De sleutel waarmee het object wordt opgeslagen
    // value: Het object dat opgeslagen wordt
    public static void SetObjectAsJson<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    // Haalt een object op uit de sessie op basis van de sleutel
    // T: Het type van het object dat opgehaald wordt
    // key: De sleutel waarmee het object is opgeslagen
    // Returns: Het opgehaalde object of null als het niet bestaat
    public static T? GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}
