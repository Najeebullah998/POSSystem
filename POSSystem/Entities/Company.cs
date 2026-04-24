using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Entities
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;
        public string? OwnerName { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public string? TaxNumber { get; set; }
        public string? RegistrationNumber { get; set; }

        public string? LogoPath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }

}
