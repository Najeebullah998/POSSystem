namespace POSSystem.Entities
{
    public class UnitViewModel
    {
        public int UnitId { get; set; }

        public string? UnitName { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public int? CompanyId { get; set; }

        public int? BranchId { get; set; }
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
