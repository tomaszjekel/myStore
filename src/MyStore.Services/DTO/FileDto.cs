using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Services.DTO
{
    public class FileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
