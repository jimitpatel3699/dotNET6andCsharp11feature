namespace Practicle8;
internal class AtmApp : TransactionData,IUserLogin,IUserAccountActions
{
    private List<UserAccount> userAccountsList;
    private UserAccount selectedAccount;
    private List<Transaction> _listofTransactionsl;
    private const decimal minimumKeptAmount = 500;
    private readonly AppScreen screen;
    public AtmApp()
    {
        screen = new AppScreen();
    }
    public void Run()
    {
        AppScreen.Welcome();
        CheckUserCardNumAndPassword();
        AppScreen.WelcomeCustomer(selectedAccount.FullName);
        while(true)
        {
            AppScreen.DisplayAppMenu();
            ProcessMenuoption();
        }            
    }
    public void InitializeData()
    {
        userAccountsList = new List<UserAccount>
        {
            new UserAccount{Id=1,FullName="jimit patel",AccountNumber=123456,CardNumber=321321,CardPin=3699,AccountBalance=5000.00m,IsLocked=false,WithdrawlAmount=0 },
            new UserAccount{Id=2,FullName="saurabh mishra",AccountNumber=654321,CardNumber=123123,CardPin=3699,AccountBalance=50000.00m,IsLocked=false,WithdrawlAmount=0 },
            new UserAccount{Id=3,FullName="jagu patel",AccountNumber=901692,CardNumber=789987,CardPin=3699,AccountBalance=50000.00m,IsLocked=false , WithdrawlAmount = 0},
        };
        _listofTransactionsl = new List<Transaction>();
    }
    public void CheckUserCardNumAndPassword()
    {
        bool isCorrectLogin = false;
        while (isCorrectLogin == false)
        {
            UserAccount inputAccount = AppScreen.UserLoginform();
            AppScreen.LoginProgress();
            foreach (UserAccount account in userAccountsList)
            {
                selectedAccount = account;
                if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                {
                    selectedAccount.TotalLogin++;
                    if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                    {
                        selectedAccount = account;

                        if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                        {
                            //print a lock message
                            AppScreen.PrintLockScreen();
                        }
                        else
                        {
                            selectedAccount.TotalLogin = 0;
                            isCorrectLogin = true;
                            break;
                        }
                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\n Invalid card number or PIN.", false);
                        selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                        if (selectedAccount.IsLocked)
                        {
                            AppScreen.PrintLockScreen();
                        }
                    }
                    Console.Clear();
                }
            }
        }
    }
    private void ProcessMenuoption()
    {
        switch(Validator.Convert<int>("an option:"))
        {
            case (int)AppMenu.CheckBalance:
                {
                    CheckBalance();
                    break;
                }
            case (int)AppMenu.Placedeposite:
                {
                    PlaceDeposite();
                    break;
                }
            case (int)AppMenu.MakeWithdrawal:
                {
                    MakeWithDrawal();
                    break;
                }
            case (int)AppMenu.InternalTransfer:
                {
                    var interTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(interTransfer);
                    break;
                }   
            case (int)AppMenu.ViewTransaction:
                {
                    ViewTransaction();
                    break;
                }
            case (int)AppMenu.Logout:
                {
                    AppScreen.LogoutProgress();
                    Utility.PrintMessage("You have successfully logged out. Please collect your ATM card.");
                    Run();
                    break;
                }
            default:
                {
                    Utility.PrintMessage("Invalid option", false);
                    break;
                }
        }
    }
    public void CheckBalance()
    {
        Utility.PrintMessage($"Your account balance is :{Utility.FormatAmout(selectedAccount.AccountBalance)}");
    }
    public void PlaceDeposite()
    {
        Console.WriteLine($"\nOnly multiple of 100 and 500 Rs. notes allowed.\n");
        var TransactionAmt = Validator.Convert<int>($"amount {AppScreen.cur}");
        //counting
        Console.WriteLine("\nChecking and counting notes.");
        Utility.PrintDotAnimation();
        Console.WriteLine("");
        if(TransactionAmt<=0)
        {
            Utility.PrintMessage("Amount needs to be greater than zero. try again.", false);
            return;
        }
        if(TransactionAmt%100!=0) 
        {            
            Utility.PrintMessage($"Enter deposite amount in multiple of 100 or 500. Try again",false);
            return;
        }
        if(PreviewBanknotesCount(TransactionAmt)==false)
        {
            Utility.PrintMessage($"You have cancelled your action",false);
            return;
        }
        InsertTransaction(selectedAccount.Id, TransactionType.Deposit, TransactionAmt, "");
        //update account balance
        selectedAccount.AccountBalance += TransactionAmt;
        //print success message
        Utility.PrintMessage($"Your deposite of {Utility.FormatAmout(TransactionAmt)} was succesful",true);
    }
    public void MakeWithDrawal()
    {
        var transactionAmt = 0;
        int selectedAmount = AppScreen.SelectAmount();            
        if(selectedAmount!=0)
        {
            transactionAmt= selectedAmount;
        }           
        //input validation
        if(transactionAmt<=0)
        {
            Utility.PrintMessage("Amount needs to be greter than zero.", false);
            return;
        }
        else if (transactionAmt > 20000)
        {
            Utility.PrintMessage("Withdrawl amount needs to be less than 20,000.", false);
            return;
        }else if(selectedAccount.WithdrawlAmount + transactionAmt > 20000)
        {
            Utility.PrintMessage($"{transactionAmt} is exceeds withdrawal of your daily 20,000 limit. ", false);
            return;
        }
        else if(selectedAccount.WithdrawlAmount>20000 )
        {
            Utility.PrintMessage($"{selectedAccount.WithdrawlAmount} is exceeds withdrawal of your daily 20,000 limit.", false);
            return;
        }
        else if(transactionAmt%100!=0)
        {
            Utility.PrintMessage("Amount needs to be multiply of 100 or 500.", false);
            return;
        }
        //business logic validation
        if(transactionAmt>selectedAccount.AccountBalance)
        {
            Utility.PrintMessage("Withdrawl fail due to low balance.", false);
            return;
        }
        if(selectedAccount.AccountBalance-transactionAmt<minimumKeptAmount)
        {
            Utility.PrintMessage($"Withdrawal failed. your account needs to have miminum {Utility.FormatAmout(minimumKeptAmount)}");
            return;
        }
        //bind withdrawls details to transaction object
        InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, -transactionAmt, "");
        //update account balance
        selectedAccount.AccountBalance -= transactionAmt;
        //add withdrawl amount
        selectedAccount.WithdrawlAmount += transactionAmt;
        //success message
        Utility.PrintMessage($"You have successfully withdrawl {Utility.FormatAmout(transactionAmt)}", true);        
    }
    private bool PreviewBanknotesCount(int amount)
    {
        int fiveHundredNotesCount = amount / 500;
        int oneHundredNotescount = (amount % 500) / 100;
        Console.WriteLine("\nSummary");
        Console.WriteLine("---------");
        if(fiveHundredNotesCount>0)
        {
            Console.WriteLine($"{AppScreen.cur}500 X {fiveHundredNotesCount}");
        }
        if(oneHundredNotescount>0)
        {
            Console.WriteLine($"{AppScreen.cur}100 X {oneHundredNotescount}");
        }
        Console.WriteLine("---------");
        Console.WriteLine($"\nTotal amount: {Utility.FormatAmout(amount)}\n\n");
        int opt = Validator.Convert<int>("1 to Confirm");
        return opt.Equals(1);
    }
    public override void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
    {
        //create new transaction object
        var transaction = new Transaction()
        {
            TransactionId = Utility.GetTransactionID(),
            UserBankAccountId= _UserBankAccountId,
            TransactionDate= DateTime.Now,
            TransactionType= _tranType,
            TransactionAmount= _tranAmount,
            Description= _desc
        };
        //add transaction object to thr list
        _listofTransactionsl.Add(transaction);
    }
    public override void ViewTransaction()
    {
       var filteredTransactionList = _listofTransactionsl.Where(t=>t.UserBankAccountId==selectedAccount.Id).ToList();
        //check if data available or not
        if(filteredTransactionList.Count <= 0 )
        {
            Utility.PrintMessage($"No transaction found", true);
        }
        else
        {
            var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount" + AppScreen.cur);
            foreach(var tran in filteredTransactionList)
            {
                table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
            }
            table.Options.EnableCount = false;
            table.Write();
            Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction", true);
        }
    }
    private void ProcessInternalTransfer(InternalTransfer internalTransfer)
    {
        if( internalTransfer.TransferAmount<=0 ) 
        {
            Utility.PrintMessage("Amount needs to be more than zero.", false);
            return;
        }
        //check sender account balance
        if(internalTransfer.TransferAmount>selectedAccount.AccountBalance ) 
        {
            Utility.PrintMessage("Balance insufficints to transfer. transfer failed", false);
            return;
        }
        //check the minimum kept amount
        if((selectedAccount.AccountBalance-internalTransfer.TransferAmount)<minimumKeptAmount)
        {
            Utility.PrintMessage("Transfer failed due to minimum maitain balance ", false);
            return;
        }
        //check receiver acount number is valid
        UserAccount ReceiverAccount;
        ReceiverAccount = selectedAccount;
        foreach(UserAccount userAccount in userAccountsList)
        {               
            if(userAccount.AccountNumber == internalTransfer.ReciepeintAccNum)
            {
                ReceiverAccount= userAccount;
                break;
            }        
        }
        if (ReceiverAccount.AccountNumber == selectedAccount.AccountNumber)
        {                
            Utility.PrintMessage("Transfer failed! in same account transfer not allowed.", false);
            return;
        }
        else if(ReceiverAccount.AccountNumber != internalTransfer.ReciepeintAccNum)
        {                
            Utility.PrintMessage("Transfer failed due to receiver account number invalid ", false);
            return;
        }else if(ReceiverAccount.FullName != internalTransfer.ReciepeintName)
        {
            Utility.PrintMessage("Transfer failed due to receiver name not match ", false);
            return;
        }
        else 
        {
            //add transaction at sender side
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, -internalTransfer.TransferAmount, $"{internalTransfer.ReciepeintAccNum}");
            //update balance of sender
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;
            //add transaction record at receiver side
            InsertTransaction(ReceiverAccount.Id, TransactionType.Transfer, internalTransfer.TransferAmount, $"{selectedAccount.AccountNumber}");
            //update balance of receiver
            ReceiverAccount.AccountBalance += internalTransfer.TransferAmount;
            Utility.PrintMessage($"Transfer of {Utility.FormatAmout(internalTransfer.TransferAmount)} succesfully to receiver account.\nReceiver account number:{ReceiverAccount.AccountNumber}\nReceiver name:{ReceiverAccount.FullName} ", true);
            return;
        }
    }       
}