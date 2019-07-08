using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Final_Project_2017
{
    /// This Program contains two list boxes.  The top list box contains customer information and the bottom list box contains 
    /// transactions that are tied to the account that is selected.  
    /// 
    /// Interaction logic for MainWindow.xaml
    /// 
    /// Note: Transaction Date Text field is set so the user can not interact with it, as the user should not be able to edit or change the date
    /// the transaction was done on.
    ///       Any new transactions will use the current systems date and time.  
    public partial class MainWindow : Window
    {

        //Note - No Data Type here - ArrayLists hold "Objects"
        //Private ArrayLists 
        private List<CustomerAccount> CustomersAccountList = new List<CustomerAccount>();
        private CustomerAccount CurrentCustomer;
        private List<AccountTransaction> CustomersTransactionList = new List<AccountTransaction>();
        private AccountTransaction CurrentTransaction;


        private int CurrentSelectedIndexAccount; //used to contain the idex of the selected line item for account
        private int CurrentselectedindexTransaction;//used to contain the index of the selected line item for transaction
        private bool NewFlag = false, ClickedAway=true, AccountSaveClick = false, TransactionSaveClick=false;


        //This loads the transactionListbox from the database, based off of AccountNumber that is assigned to the line item that 
        //was selected in the AccountListBox
        private void LoadTransactions(int AccountNumber)
        {
            
            CustomersTransactionList.Clear(); //Empties out the List<>
            transactionListbox.Items.Clear(); // Empties out the listbox
            decimal SelectedCustomerBalance = 0;
            balanceTextbox.Text = "";

           
            //Adds Row Headers to the transactionListbox
            transactionListbox.Items.Add("  Transaction Number           Transaction Date                     Withdrawals               Deposit");
           
            //declare a connecton object
            using (SqlConnection connection = new SqlConnection())
            {
                //declare a connectino object
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                //opens the connection
                connection.Open();

                //Create a SQL command object.
                string sql = $"SELECT * FROM AccountTransactions where Accountnumber = {AccountNumber} Order by TransactionDate;";
                SqlCommand myCommand = new SqlCommand(sql, connection);
                //Run the command: into a data reader using the ExecuteReader() Method.
                using (SqlDataReader myDataReader = myCommand.ExecuteReader())
                {
                    //Loop over the results.  MyDataReader.Read returns false at the end of the records returned
                    while (myDataReader.Read())
                    {
                        //inserts the record that is contained in the myDataReader.Read and puts each column of the datareader into 
                        //one property of a AccountTransaction Object. 
                        AccountTransaction NewTransaction = new AccountTransaction((Int32)myDataReader["TransactionNumber"],
                                                            (Int32)myDataReader["AccountNumber"],
                                                            (DateTime)myDataReader["TransactionDate"],
                                                            (decimal)myDataReader["TransactionAmount"]);

                        //Add the Trandsaction to the List<Contact>
                        CustomersTransactionList.Add(NewTransaction);
                        // Add the contact to the listbox
                        transactionListbox.Items.Add(NewTransaction);
                        //Keeps a running balance of the contacts 
                        SelectedCustomerBalance = SelectedCustomerBalance + NewTransaction.TransactionAmount;
                       
                    }
                }
            }
            //Inserts the balance total into the text box
            balanceTextbox.Text = string.Format("{0:0.00}", SelectedCustomerBalance);
        }

        //Takes the inforamtion from the object and displays them in text boxes.  
       private void DisplayTransaction(AccountTransaction TransactionItem)
        {
            // This checks to see if a record was selected in the Transaction List Box.  If no item was selected then all transaciton
            // text boxes have a value of ""
            if (transactionListbox.SelectedItem != null)
            {
                transactionNumbertextBox.Text = TransactionItem.TransactionNumber.ToString();
                transactionDatetextBox.Text = TransactionItem.TransactionDate.ToString();
                transactionAmountTextbox.Text = string.Format("{0:0.00}",TransactionItem.TransactionAmount);
            }
            else
            {
                transactionNumbertextBox.Text = "";
                transactionDatetextBox.Text = "";
                transactionAmountTextbox.Text = "";
            }
        }


        //Fills the AccountListbox with information from the database.  
        private void LoadCustomers()
        {
            CustomersAccountList.Clear();//clear the List<>
            AccountListbox.Items.Clear();//clears the Listbox
            
            //adds a title for the records that are to be placed in the listbox
            AccountListbox.Items.Add("Customer Account     First Name       Last Name       Email Address");

            //declare a connectino object
            using (SqlConnection connection = new SqlConnection())
            {
                //point connectino to the database to be used
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                //open the connection
                connection.Open();
                string sql = "SELECT * FROM CustomerAccounts Order by AccountNumber;";
                SqlCommand myCommand = new SqlCommand(sql, connection);
                //Run the command: into a data reader using the ExecuteReader()Method.
                using (SqlDataReader myDataReader = myCommand.ExecuteReader())
                {
                    //performs a loop until the myDataReader returns the last record in the list.  
                    while (myDataReader.Read())
                    {
                        //inserts the record that is contained in the myDataReader.Read and puts each column of the datareader into 
                        //one property of a AccountTransaction Object. 
                        CustomerAccount NewCustomer = new CustomerAccount((Int32)myDataReader["AccountNumber"],
                                                            (string)myDataReader["FirstName"],
                                                            (string)myDataReader["LastName"],
                                                            (string)myDataReader["Email"],
                                                            (string)myDataReader["Phone"]);

                        //adds the record to the list<>
                        CustomersAccountList.Add(NewCustomer);
                        //adds the record to the listbox
                        AccountListbox.Items.Add(NewCustomer);

                    }
                }
            }
        }

        //Assigns value to the textboxes that are related to the CustomerAccount.  
        private void DisplayCustomer(CustomerAccount CustomerItem)
        {
            //reset the content in the error label boxes.  
            accountErrorLabel.Content = "";
            transactionErrorlabel.Content = "";

            //checks to see if a record within the AccountListbox was selected and if there wasn't set the text boxes value to "" 
            //else fill in the information using the records in the LIST<>
            if (AccountListbox.SelectedItem != null)
            {
                accountNumbertextBox.Text = CustomerItem.AccountNumber.ToString();
                firstNametextBox.Text = CustomerItem.FirstName;
                lastNametextBox.Text = CustomerItem.LastName;
                emailTextBox.Text = CustomerItem.Email;
                phoneTextbox.Text = CustomerItem.PhoneNumber;
            }
            else
            {
                accountNumbertextBox.Text = "";
                firstNametextBox.Text = "";
                lastNametextBox.Text = "";
                emailTextBox.Text = "";
                phoneTextbox.Text = "";
            }
        }

        //Checks to see if values are valid in certain fields.  
        private bool IsDataValid()
        {
            string Firstname, LastName, Email, TransactionAmount;
            decimal x;


            Firstname = firstNametextBox.Text;
            LastName = lastNametextBox.Text;
            Email = emailTextBox.Text;
            TransactionAmount = transactionAmountTextbox.Text;

            //if the Save button that is related to the Customer Accounts is clicked a boolean flag is set for AccountSaveClick
            if(AccountSaveClick)
            {
                if (Firstname == "") //checks to see if the firstname field is blank
                {
                    accountErrorLabel.Content = "Please enter a Valid First Name";
                    return false;
                }

                if (LastName == "") //checks to see if the lastname field is blank
                {
                    accountErrorLabel.Content = "Please enter a Valid Last Name";
                    return false;
                }

                if (Email == "") //checks to see if the email field is blank
                {
                    accountErrorLabel.Content = "Please enter a Valid Email Address";
                    return false;
                }
            }

            //If the Save button that is related to the Transaction Accounts is clicked a boolena flag is set for TransactinoSaveClick
            if (TransactionSaveClick)
            {
                //checks to see if the value that is put into the transacinoamount is a valid decimal type
                if (!decimal.TryParse(TransactionAmount, out x))
                {
                    
                    transactionErrorlabel.Content = "Please enter a valid Transaction Amount";
                    return false;
                }
                else if (x > 300000000) 
                {
                    transactionErrorlabel.Content = "Transaction Amount can not be greater than 300,000,000.00";
                    return false;
                }
            }
            //sets the saveclick flags to false. 
            AccountSaveClick = false;
            TransactionSaveClick = false;
            return true; // if there was no issue true is returned

        }


        public MainWindow()
        {
            InitializeComponent();
            LoadCustomers(); //calls the method that will be used to load the information in the list box.
        }

        //This Method is called when ever the focus within the ListBox changes
        private void AccountListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //checks to see if the top Row (titles are selected and if so, make it so no line item is selected within the listbox
            if (AccountListbox.SelectedIndex == 0)
            {
                AccountListbox.SelectedIndex = -1; //unselects any item within the list box
                LoadTransactions(-1); //reloads the Transactionlistbox
                return;
            }
            
                 int SelectedAccountNumber = 0;
           


            //checks to make sure that the item is not a new item or if a item in the listbox is selected after the new record button was 
            //initiated.  

            if(NewFlag == false || ClickedAway == true)
            {
                
                CurrentCustomer = (CustomerAccount)AccountListbox.SelectedItem;
                CurrentSelectedIndexAccount = AccountListbox.SelectedIndex;
                if (CurrentSelectedIndexAccount == -1)return; //no record selected, when you click add the method is called and the event needs to handle -1 index
                else
                {
                    SelectedAccountNumber = CurrentCustomer.AccountNumber;
                    DisplayCustomer(CurrentCustomer);
                    LoadTransactions(SelectedAccountNumber);
                    ClickedAway = true;
                    NewFlag = false;
                }

            }
            else 
            {
                CurrentCustomer = (CustomerAccount)AccountListbox.SelectedItem;
            }
            DisplayCustomer(CurrentCustomer); //calls the Method DisplayCustomer
        

           

        }

        //Blanks out the text fields related tot he customer accounts.
        private void accountNewbutton_Click(object sender, RoutedEventArgs e)
        {
            NewFlag = true;
            ClickedAway = true;
            //clears out the list<>
            CustomersAccountList.Clear();
            //clears out the listbox
            AccountListbox.Items.Clear();
            //clears out the list<>
            CustomersTransactionList.Clear();
            //clears out the listbox
            transactionListbox.Items.Clear();
            //reloads the customer list
            LoadCustomers();
            //sets the text box to blank
            accountNumbertextBox.Text = "";
            firstNametextBox.Text = "";
            lastNametextBox.Text = "";
            phoneTextbox.Text = "";
            emailTextBox.Text = "";
            balanceTextbox.Text = "";
            //sets focus to the firstname textbox
            firstNametextBox.Focus();
            

        }

        
        //Saves the record either new or updated record when the button is clicked.  
        private void accountSavebutton_Click(object sender, RoutedEventArgs e)
        {

            AccountSaveClick = true; //sets the variable to true so the IsDataValid method knows what values to check
            if (!IsDataValid()) return; //data validation
            ClickedAway = false; 
           
            //performs this is the account is a new record.  
            if (NewFlag)
            {
                int number = 0;

                //declare a connection object
                using (SqlConnection connection = new SqlConnection())
                {
                    //points connection to the database
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    //opens the connection 
                    connection.Open();
                    //grabs the max AccountNumber value that is contained in CustomerAccounts table
                    string sql = "SELECT MAX(AccountNumber) as AccountNumber FROM CustomerAccounts;";
                    SqlCommand myCommand = new SqlCommand(sql, connection);
                    //Run the command: puts the max value into ExecuteScalar and increases it by 1
                    number = Convert.ToInt32(myCommand.ExecuteScalar()) + 1;

                }
                //creates a new objet in the list 
                CustomerAccount NewContact = new CustomerAccount(number, "", "", "", "");

                //inputs the values from the text boxs into the list
                NewContact.FirstName = firstNametextBox.Text;
                NewContact.LastName = lastNametextBox.Text;
                NewContact.Email = emailTextBox.Text;
                NewContact.PhoneNumber = phoneTextbox.Text;

                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    connection.Open();
                    //insterts the new record into the table CustomerAccounts
                    string sql = $"INSERT INTO CustomerAccounts Values(" +
                                 $"{NewContact.AccountNumber}, " +
                                 $"'{NewContact.FirstName}', " +
                                 $"'{NewContact.LastName}', " +
                                 $"'{NewContact.Email}', " +
                                 $"'{NewContact.PhoneNumber}'); ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    //reloads the listbox
                    LoadCustomers();
                   //selects that new record 
                    AccountListbox.SelectedIndex = CustomersAccountList.Count();
                }
            }
            else  //update to an existing record 
            {
                //sets the values in the textboxs to the list<> value
                CurrentCustomer.FirstName = firstNametextBox.Text;
                CurrentCustomer.LastName = lastNametextBox.Text;
                CurrentCustomer.Email = emailTextBox.Text;
                CurrentCustomer.PhoneNumber = phoneTextbox.Text;
                CurrentCustomer.AccountNumber = Convert.ToInt32(accountNumbertextBox.Text);


                if (AccountListbox.SelectedItem == null)
                {
                    MessageBox.Show("No Customer Selected");
                    return;
                }

                using (SqlConnection connection = new SqlConnection())
                {

                    int IndextoReselect;
                    IndextoReselect = CurrentSelectedIndexAccount;
                    //point connectino to the database
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    //open the database
                    connection.Open();

                    //updates the record in the database with the new information
                    string sql = $"Update CustomerAccounts Set " +
                               $"FirstName = '{CurrentCustomer.FirstName}', " +
                               $"LastName = '{CurrentCustomer.LastName}', " +
                               $"Phone = '{CurrentCustomer.PhoneNumber}', " +
                               $"Email = '{CurrentCustomer.Email}' " +
                               $"Where AccountNumber = '{CurrentCustomer.AccountNumber}';";



                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery(); // we expect no records back
                    }
                    LoadCustomers();//reloads the list
                    AccountListbox.SelectedIndex = IndextoReselect;
                  
                }
            }
            NewFlag = false; //sets the NewFlag to false 
            firstNametextBox.Focus(); //sets the focus to the firstname textbox
        }

        //Deletes the selected record in the AccountListBox 
        private void accountDeletebutton_Click(object sender, RoutedEventArgs e)
        {
            int IndextoDelete;
            //checks to make sure a record is selected
            if (AccountListbox.SelectedItem == null)
            {
                MessageBox.Show("You must select a Customer to delete.", "Databsae");
                return;
            }
            //prompts the user with a messagebox making sure they want to delete the selected record
            var result = MessageBox.Show("Are you sure you want to delete " + CurrentCustomer.ToString() + "?",
                "Database", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                //record the index we are set to delete 
                IndextoDelete = CurrentSelectedIndexAccount;
                using (SqlConnection connection = new SqlConnection())
                {
                    //point connection to the database
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    //open the connection 
                    connection.Open();
                    //the sql delete string which will be used to delete the record 
                    string sql = $"Delete from CustomerAccounts where AccountNumber = {CurrentCustomer.AccountNumber}";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException) 
                        {
                            Exception error = new Exception("Unable to delete, there are transactions tied to this customer");
                            MessageBox.Show(error.Message,"Deletion Error"); //if an error is tossed this message is to be displayed instead of crashing the program
                            return;
                           // throw error;
                        }
                    }
                    CustomersAccountList.Remove(CurrentCustomer); //remove from the list <>
                    AccountListbox.Items.Remove(CurrentCustomer); // remove from the listbox 
                    if (IndextoDelete > CustomersAccountList.Count) //Checks to see if the deleted row was the last item in the list
                    {
                        CurrentSelectedIndexAccount = CustomersAccountList.Count ; //if the item was the last in the list, set the index count -1 
                    }
                    else
                    {
                        CurrentSelectedIndexAccount = IndextoDelete;
                    }
                    AccountListbox.SelectedIndex = CurrentSelectedIndexAccount; //select the item in the listbox 
                }
            }
            firstNametextBox.Focus();//sets focus to the firstname text box
        }

        //When ever the focus is changed within the TransactinoListbox
        private void transactionListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if the title bar is selected
            if (transactionListbox.SelectedIndex == 0)
            {
                transactionListbox.SelectedIndex = -1;
                return;
            }

            //loads the selected record into the object list
            CurrentTransaction = (AccountTransaction)transactionListbox.SelectedItem;
            //sets the index value
            CurrentselectedindexTransaction = transactionListbox.SelectedIndex;
            //loads the selected row into the transaction text boxes
            DisplayTransaction(CurrentTransaction);
            NewFlag = false;

        }

        //Method is performed when the Transaction new button is clicked, set to clear out the transaction text boxes
        private void transactionNewbutton_Click(object sender, RoutedEventArgs e)
        {
            int accountnumber = 0;
            //sets the selected customer account number
            accountnumber = CurrentCustomer.AccountNumber;

            //clears the list <>
            CustomersTransactionList.Clear();
            //clears the listbox
            transactionListbox.Items.Clear();
            //reloads the transactionlistbox using the selected customer account number
            LoadTransactions(accountnumber);
            transactionNumbertextBox.Text = "";
            transactionDatetextBox.Text = "";
            
            //sets the Newflag to true, to be used in  transactinoSavebutton method
            NewFlag = true;

        }

        //Method is performed when the save button is clicked.  Saves the record to the database
        private void transactionSavebutton_Click(object sender, RoutedEventArgs e)
        {
            //sets the flag to true, this is used in the IsDataValid method so the method knows what values to check
            TransactionSaveClick = true;

            //checks to see if the data is valid
            if (!IsDataValid()) return;

            //Checks to see if a customer record was selected 
            if (AccountListbox.SelectedIndex == -1)
            {
                transactionErrorlabel.Content = "No Customer record is selected";
                return;
            }
            //if a new record, or no transactions exist or if no transactions are selected 
            if (NewFlag || CustomersTransactionList.Count==0 || transactionListbox.SelectedIndex==-1)
            {
                int Number = 0;
                int CurrentAccountNumber = 0;
                DateTime CurrentDateTime = DateTime.Now;
               

                CurrentAccountNumber = CurrentCustomer.AccountNumber;

                using (SqlConnection connection = new SqlConnection())
                {
                    //point connection to the database
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    //open the connection
                    connection.Open();
                    //sql command that will get the max transaction number from accounttransactions table
                    string sql = "SELECT MAX(TransactionNumber) as TransactionNumber FROM AccountTransactions;";
                    SqlCommand myCommand = new SqlCommand(sql, connection);
                    //If no transactions exist set the transaction number to 1
                    if (Convert.ToString(myCommand.ExecuteScalar()) == "") Number =1;
                    //grab the max transaction number and add one
                    else Number = Convert.ToInt32(myCommand.ExecuteScalar()) + 1;

                }
                //creates a new list Object and inputs the number, accountnumber and current date
                AccountTransaction NewTransaction = new AccountTransaction(Number, CurrentAccountNumber, CurrentDateTime, 0);
                //adds the amount to the list 
                NewTransaction.TransactionAmount = Convert.ToDecimal(transactionAmountTextbox.Text);

                using (SqlConnection connection = new SqlConnection())
                {
                    //point connection to the database
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    //open the connectino 
                    connection.Open();

                    //sql comment that will be executed: inserts the values into the table 
                    string sql = $"INSERT INTO AccountTransactions Values(" +
                                 $"{NewTransaction.TransactionNumber}, " +
                                 $"{NewTransaction.AccountNumber}, " +
                                 $"'{NewTransaction.TransactionDate}', " +
                                 $"{NewTransaction.TransactionAmount});";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    //loads the transactionlistbox
                    LoadTransactions(NewTransaction.AccountNumber);
                    //clears out the textbox
                    transactionAmountTextbox.Text = "";
                    NewFlag = false;

                }
            }
            else //performs when the record was modified 
            {
               

                if (transactionListbox.SelectedItem == null)
                {
                    MessageBox.Show("No Records loaded");
                    transactionAmountTextbox.Text = "";
                    return;
                }

                CurrentTransaction.TransactionAmount = Convert.ToDecimal(transactionAmountTextbox.Text);

                using (SqlConnection connection = new SqlConnection())
                {

                    int IndextoReselect;
                    //stores the index value
                    IndextoReselect = CurrentselectedindexTransaction;
                        //CurrentSelectedIndexAccount;
                    //points connection to the database
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    //open the connection
                    connection.Open();
                    //sql string to be executed to update the record
                    string sql = $"Update AccountTransactions Set " +
                               $"TransactionAmount = {CurrentTransaction.TransactionAmount} " +
                               $"Where TransactionNumber = '{CurrentTransaction.TransactionNumber}';";
                    
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery(); // we expect no records back
                    }
                    //refreshes the listbox
                    LoadTransactions(CurrentTransaction.AccountNumber);
                    //selects the record that was updated
                    transactionListbox.SelectedIndex = IndextoReselect;
                
                }
            }
        }

        //deletes a record from the transactionlistbox
        private void transactionDeletebutton_Click(object sender, RoutedEventArgs e)
        {
            int IndextoDelete;

            //If no record was selected before the delete button was clicked
            if (CurrentSelectedIndexAccount == -1 || transactionListbox.SelectedItem == null)
            {
                MessageBox.Show("You must select a Transaction to delete.", "Databsae");
                return;
            }
            //confirmation delete message 
            var result = MessageBox.Show("Are you sure you want to delete " + CurrentTransaction.ToString() + "?",
               "Database", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IndextoDelete = CurrentselectedindexTransaction;
                    //CurrentSelectedIndexAccount;
                using (SqlConnection connection = new SqlConnection())
                {
                    //points connection to the database 
                    connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CustomerAccounts.mdf;Integrated Security=True";
                    //opens the connection
                    connection.Open();
                    //sql command to be executed: Delete record from table AccountTransactions
                    string sql = $"Delete from AccountTransactions where TransactionNumber = {CurrentTransaction.TransactionNumber}";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            Exception error = new Exception("No record matching that Account Number", ex);
                            throw error;
                        }
                    }
                    //removes from the list<>
                    CustomersTransactionList.Remove(CurrentTransaction);
                    //refreshes the listbox
                    LoadTransactions(CurrentCustomer.AccountNumber);
                    CurrentSelectedIndexAccount = IndextoDelete;
                    //selects a new record that is now in the place of the deleted record in the list
                    transactionListbox.SelectedIndex = IndextoDelete-1;
                }
            }
        }
    }
}

