using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos
{
    public class ProfileModel
    {
        public record Request(string Name, string Phone);
        public record Response(string Dni, string Name, string Phone);
    }
}
