namespace Practicle8.App;
internal sealed class Entry
{
    static void Main(string[] args)
    {            
        AtmApp atmApp = new AtmApp();
        atmApp.InitializeData();
        atmApp.Run();           
    }
}
