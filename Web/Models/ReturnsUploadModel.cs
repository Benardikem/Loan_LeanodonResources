using System.Collections.Generic;

namespace Web.Models
{
    public class ReturnsUploadModel
    {
        public string SessionId { get; set; }
        public List<OfferItem> Offers { get; set; }
        //public List<AccountItem> Accounts { get; set; }
        public List<BankItem> Banks { get; set; }
    }
}