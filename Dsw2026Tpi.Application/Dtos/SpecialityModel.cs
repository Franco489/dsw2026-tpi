using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos
{
    public record SpecialityModel
    {
        public record Request(string name, string description);
        public record UpdateRequest(string name, string description);
        public record UpdateResponse(Guid id, string name, string description);

        public record FilterResponse(Guid? id, string name, string description);
        public record CreateResponse(Guid? id, string name, string description);
        public record Response(string name, string description);
    }
       

}
