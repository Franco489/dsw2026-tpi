using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Dsw2026Tpi.Domain.Utils;

public static class DateOnlyExtensions
{
    #region Load Holidays
    private static List<HolidayInfo> _holidays = [];
    private static void LoadHolidays() 
    {
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 1, 1), Name = "Año Nuevo" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 2, 16), Name = "Carnaval" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 2, 17), Name = "Carnaval" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 3, 24), Name = "Día Nacional de la Memoria por la Verdad y la Justicia" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 4, 2), Name = "Día del Veterano y de los Caídos en la Guerra de Malvinas / Jueves Santo" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 4, 3), Name = "Viernes Santo" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 5, 1), Name = "Día del Trabajador" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 5, 25), Name = "Día de la Revolución de Mayo" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 6, 15), Name = "Paso a la Inmortalidad del General Martín Miguel de Güemes" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 6, 20), Name = "Paso a la Inmortalidad del General Manuel Belgrano" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 7, 9), Name = "Día de la Independencia" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 8, 17), Name = "Paso a la Inmortalidad del General José de San Martín" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 10, 12), Name = "Día del Respeto a la Diversidad Cultural" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 11, 23), Name = "Día de la Soberanía Nacional" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 12, 8), Name = "Inmaculada Concepción de María" });
        _holidays.Add(new HolidayInfo { Date = new DateOnly(2026, 12, 25), Name = "Navidad" });
    }
    #endregion

    public static bool IsHoliday(this DateOnly date)
    {
        if (!_holidays.Any())
        {
            LoadHolidays();
        }
        return _holidays.Any(h => h.Date == date);
    }
}

public struct HolidayInfo
{
    public DateOnly Date { get; set; }
    public string Name { get; set; }
}

