namespace Practicle8.Domain.Interfaces;

    internal abstract class TransactionData
    {
        abstract public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc);
        abstract public void ViewTransaction();
    }
