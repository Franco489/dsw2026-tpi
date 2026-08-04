using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Dsw2026Tpi.Application.Utils;

public static class DateOnlyExtensions
{
    public record Holiday(DateOnly Date, string Name);
    private static readonly List<Holiday> _holidays = [];
    private static void LoadHolidays()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Sources", "holidays.json");
        if (!File.Exists(filePath))
        {
            return;
        }
        var json = File.ReadAllText(filePath);
        _holidays.AddRange(JsonSerializer.Deserialize<List<Holiday>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }));
    }
    public static bool IsHoliday(this DateOnly date)
    {
        if (!_holidays.Any())
        {
            LoadHolidays();
        }
        return _holidays.Any(h => h.Date == date);
    }
}



