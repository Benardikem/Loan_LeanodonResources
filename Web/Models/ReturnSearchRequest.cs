namespace Web.Models
{
    public class ReturnSearchRequest : SearchRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UserName { get; set; }
    }
    public class InvestorSearchRequest : SearchRequest
    {
        public long ReturnId { get; set; }
    }
    public class InvestorReportSearchRequest : SearchRequest
    {
        public string BVN { get; set; }
        public string Agent { get; set; }
    }
    public class UnapprovedReturnsBatchReportSearchRequest : SearchRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Agent { get; set; }
    }

    public class AgentSearchRequest : SearchRequest
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}