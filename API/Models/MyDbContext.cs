using System.Data.Entity;

namespace API.Models
{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext() : base(Settings.Database.ConnectionString())
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<WRole> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<SubMenuRole> SubmenuRoles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<LoanRepayment> LoanRepayments { get; set; }
        public DbSet<CheckAppStatus> CheckAppStats { get; set; }
        public DbSet<LastLoanMonthRepaid> LastLoanMonth { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SMSContactList> SMSContactLists { get; set; }
        public DbSet<SMSLog> SMSLogs { get; set; }
        public DbSet<SMSContactCategory> SMSContactCategorys { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
