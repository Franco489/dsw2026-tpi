using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos
{
    public class ProfileModel
    {
        public record UpdateProfileRequest(string Dni, string Name, String Phone);
    }
}
