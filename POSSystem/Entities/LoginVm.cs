namespace POSSystem.Entities
{
    public class LoginVm
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public string? RoleName { get; set; }
        public string? BranchName { get; set; }
        public string? CompanyName { get; set; }
        public int BusinessTypeId { get; set; }
        public string BusinessName { get; set; }
    }
}
