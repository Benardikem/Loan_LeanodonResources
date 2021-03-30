using System.Collections.Generic;

namespace Web.Models
{
    public class ActionResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
    }

    public class ReturnUploadResponse : ActionResponse
    {
        public int RecordCount { get; set; }
        public long TotalShares { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OfferRate { get; set; }
        public decimal Name { get; set; }
        public string FileName { get; set; }
        public string OfferName { get; set; }
        public string FilePath { get; set; }
        public List<string> ExistingBVNs { get; internal set; }
        public string ValidationError { get; internal set; }
        public List<string> DuplicateBVNs { get; internal set; }
    }
    public class UploadPaymentResponse : ActionResponse
    {
        public string name { get; internal set; }
        public int size { get; internal set; }
        public string type { get; internal set; }
        public string path { get; internal set; }
    }
    public class DownloadResponse : ActionResponse
    {
        public string blob { get; internal set; }
        public string name { get; internal set; }
        public int size { get; internal set; }
        public string type { get; internal set; }
        public string path { get; internal set; }
    }
    //public class GetBankResponse : ActionResponse
    //{
    //    public List<AccountItem> Accounts { get; set; }
    //}
}