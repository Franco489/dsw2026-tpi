namespace Dsw2026Tpi.Domain.Utils
{
    public static class StringExtensions
    {
        public static DayOfWeek toDayOfWeek(this string day) 
        {
            return day.Trim().ToUpper() switch
            {
                "LUNES" => DayOfWeek.Monday,
                "MARTES" => DayOfWeek.Tuesday,
                "MIÉRCOLES" or "MIERCOLES" => DayOfWeek.Wednesday,
                "JUEVES" => DayOfWeek.Thursday,
                "VIERNES" => DayOfWeek.Friday,
                "SÁBADO" or "SABADO" => DayOfWeek.Saturday,
                "DOMINGO" => DayOfWeek.Sunday,
                _ => throw new ArgumentException($"El día '{day}' no es válido.")
            };
        }
    }
}
