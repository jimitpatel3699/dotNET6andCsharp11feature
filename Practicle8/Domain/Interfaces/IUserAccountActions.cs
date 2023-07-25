namespace Practicle8.Domain.Interfaces;

public interface IUserAccountActions
{
    void CheckBalance();
    void PlaceDeposite();
    void MakeWithDrawal();

    void DefaultImplementation()
    {
        Console.WriteLine("From defalult implementation");
    }
}
