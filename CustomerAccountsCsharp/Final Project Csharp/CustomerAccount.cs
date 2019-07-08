using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Project_2017
{
    class CustomerAccount
    {
        public CustomerAccount(int AccountNumber,  
                               string FirstName, string LastName,
                               string Email, string PhoneNumber)
        {
            this.AccountNumber = AccountNumber;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;

        }

        public int AccountNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            string customer;
            customer = string.Format("     {0,-5}{1,17}{2,20}{3,20}", AccountNumber, FirstName, LastName, Email);
            return(customer);
        }
       
        /*
         Account
         FirstName
         LastName
         Email
         PhoneNumber
         */
        

    }
}
