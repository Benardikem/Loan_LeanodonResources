using API.Models;
using API.Utilities;
using Web.Codes;
using Web.Interfaces;
using Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Web.Extensions
{
    public static class Extension
    {
        public static string AsPropertyName(this string source)
        {
            return char.ToUpper(source[0]) + source.Substring(1);
        }
        public static bool ContainsIgnoringCase(this string source, string substring)
        {
            if (String.IsNullOrEmpty(source)) return false;
            return source.ToLower().Contains(substring.ToLower());
        }
       

        //public static List<Investor> ToInvestorList(this DataTable dt, string uploader, decimal rate, int multiples, out List<InvestorValidationResponse> validationResponses, out List<String> existingBVN)
        //{
        //    List<Investor> items = new List<Investor>();
        //    validationResponses = new List<InvestorValidationResponse> { };
        //    existingBVN = new List<string>();
        //    try
        //    {
        //        IAppManager _appMgr = new AppManager();
        //        int iCount = 2;
        //        using (var _db = new MyDbContext())
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                try
        //                {
        //                    Investor item = new Investor();
        //                    //Surname
        //                    item.Surname = General.GetXString(dr[0]);
        //                    //First_Name
        //                    item.FirstName = General.GetXString(dr[1]);
        //                    //Other_ Name	
        //                    item.OtherName = General.GetXString(dr[2]);
        //                    //Adress 1	
        //                    item.Address1 = General.GetXString(dr[3]);
        //                    //Address 2	
        //                    item.Address2 = General.GetXString(dr[4]);
        //                    //City
        //                    item.City = General.GetXString(dr[5]);
        //                    //State	
        //                    item.StateOfOrigin = General.GetXString(dr[6]);
        //                    //Country
        //                    item.Country = General.GetXString(dr[7]);
        //                    //No fo Shares
        //                    item.NoOfShares = Convert.ToInt32(General.GetXCurrency(dr[8]));
        //                    //Amount	
        //                    item.Amount = General.GetXCurrency(dr[9]);
        //                    //Bank_Name	
        //                    item.BankName = General.GetXString(dr[10]);
        //                    //Bank_ Account No	
        //                    item.BankAccountNo = General.GetXString(dr[11]);
        //                    //BVN	
        //                    item.BVN = General.GetXString(dr[12]);
        //                    //Telephone_ No	
        //                    item.TelephoneNo = General.GetXString(dr[13]);
        //                    //CHN	
        //                    item.CHN = General.GetXString(dr[14]);
        //                    //RC_ No
        //                    item.RCNo = General.GetXString(dr[15]);
        //                    //Title	
        //                    item.Title = General.GetXString(dr[16]);
        //                    //Status
        //                    item.Status = General.GetXString(dr[17]);
        //                    //Gender	
        //                    item.Gender = General.GetXString(dr[18]);
        //                    //Year of Incorporation	
        //                    item.YearOfIncorporation = General.GetXString(dr[19]);
        //                    //Date of Birth
        //                    item.DateOfBirth = General.GetXString(dr[20]);
        //                    //Email	
        //                    item.Email = General.GetXString(dr[21]);
        //                    //Occupation/Nature of Business
        //                    item.Occupation = General.GetXString(dr[22]);
        //                    var results = new List<ValidationResult>();
        //                    if (_appMgr.TryValidate(item, out results))
        //                    {
        //                        Boolean isValid = true;
        //                        //Check Bank and Account
        //                        string _bankCode = item.BankName;
        //                        string[] arr = item.BankName.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
        //                        if (arr.Length == 1)
        //                            _bankCode = arr[0].Trim();
        //                        else if (arr.Length == 2)
        //                            _bankCode = arr[1].Trim();
        //                        NubanValidationResult nubanValidationResult = Nuban.validateAccount(_bankCode, item.BankAccountNo);
        //                        if (nubanValidationResult.resultCode != "00")
        //                        {
        //                            validationResponses.Add(new InvestorValidationResponse
        //                            {
        //                                Row = iCount,
        //                                ValidationResults = new List<ValidationResult> { new ValidationResult(nubanValidationResult.resultMessage) }
        //                            });
        //                            isValid = false;
        //                        }
        //                        //Check Amount 
        //                        if (item.Amount != item.NoOfShares * rate)
        //                        {
        //                            validationResponses.Add(new InvestorValidationResponse
        //                            {
        //                                Row = iCount,
        //                                ValidationResults = new List<ValidationResult> { new ValidationResult("Incorrect Amount") }
        //                            });
        //                            isValid = false;
        //                        }
        //                        //Check Multiples 
        //                        if (item.NoOfShares % multiples > 0)
        //                        {
        //                            validationResponses.Add(new InvestorValidationResponse
        //                            {
        //                                Row = iCount,
        //                                ValidationResults = new List<ValidationResult> { new ValidationResult($"Shares must be in multiples of {multiples.ToString("#,##0")}") }
        //                            });
        //                            isValid = false;
        //                        }
        //                        if (isValid)
        //                        {
        //                            //Check if exists
        //                            var _check = _db.Investors.Count(i => i.Return.CreatedBy == uploader && i.Return.Status != Status.REJECTED && i.BVN == item.BVN);
        //                            if (_check > 0)
        //                                existingBVN.Add(item.BVN);
        //                            items.Add(item);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        validationResponses.Add(new InvestorValidationResponse
        //                        {
        //                            Row = iCount,
        //                            ValidationResults = results
        //                        });
        //                    }
        //                }
        //                catch (Exception ex1)
        //                {
        //                    General.LOGGER.Error(string.Format("Row {0}", iCount), ex1);
        //                    validationResponses.Add(new InvestorValidationResponse
        //                    {
        //                        Row = iCount,
        //                        ValidationResults = new List<ValidationResult> { new ValidationResult("System Error") }
        //                    });
        //                }
        //                iCount++;
        //            }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.LOGGER.Error(ex.Source, ex);
        //    }
        //    return items;
        //}
    }
}