using System;

namespace API.Utilities
{
    public class NubanValidationResult
    {
        public String resultCode { get; set; }
        public String resultMessage { get; set; }
    }
    public class Nuban
    {
        public static NubanValidationResult validateAccount(String BankCode, String AccountNo)
        {
            NubanValidationResult ret = new NubanValidationResult { resultCode = "99" };
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (!regex.IsMatch(BankCode) || BankCode.Length != 3)
            {
                ret.resultMessage = "Bank code must be 3 digit";
            }
            else
            {
                if (!regex.IsMatch(AccountNo) || AccountNo.Length != 10)
                {
                    ret.resultMessage = "Account number must be 10 digit";
                }
                else
                {
                    //ret.resultCode = "00";
                    int[] arrCheck = new int[] { 3, 7, 3, 3, 7, 3, 3, 7, 3 };
                    char[] arrBankCode = BankCode.ToCharArray();
                    char[] arrAccountNo = AccountNo.ToCharArray();
                    int sBankCode = 0, sAccountNo = 0;

                    //Sum up account nos
                    for (int i = 0; i < 9; i++)
                    {
                        sAccountNo += Convert.ToInt32(arrAccountNo[i].ToString()) * arrCheck[i];
                    }

                    //Sum up account nos
                    for (int i = 0; i < 3; i++)
                    {
                        sBankCode += Convert.ToInt32(arrBankCode[i].ToString()) * arrCheck[i];
                    }

                    int iDigit = sAccountNo + sBankCode;
                    int iMod = iDigit % 10;
                    int iCheckDigit = iMod == 0 ? 0 : 10 - iMod;
                    if (iCheckDigit.ToString() == arrAccountNo[9].ToString())
                        ret.resultCode = "00";
                    else
                        ret.resultMessage = "Invalid NUBAN Number";
                }
            }
            return ret;
        }
    }
}
