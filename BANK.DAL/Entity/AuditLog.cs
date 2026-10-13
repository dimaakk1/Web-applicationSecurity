using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK.DAL.Entity
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Method { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
