using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Project_2017
{
    class AccountTransaction
    {
        public AccountTransaction(int TransactionNumber, int AccountNumber,
                                  DateTime TransactionDate, decimal TransactionAmount)
        {
            this.TransactionNumber = TransactionNumber;
            this.AccountNumber = AccountNumber;
            this.TransactionDate = TransactionDate;
            this.TransactionAmount = TransactionAmount;
            
        }

        public int TransactionNumber { get; set; }
        public int AccountNumber { get; set; }

        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }

        public override string ToString()
        {
            string transaction;
            if(TransactionAmount > 0) transaction = string.Format("          {0,-8}{1,32}{2,51:C}",TransactionNumber, TransactionDate,TransactionAmount);
            else transaction = string.Format("          {0,-8}{1,32}{2,28:C}", TransactionNumber, TransactionDate, TransactionAmount);
            return transaction;
        }   
           
       


    }
}

