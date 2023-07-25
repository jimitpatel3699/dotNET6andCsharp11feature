namespace Practicle8.Domain.Entities;
internal static class GenerateId
{
    public static string GuId()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString();
    }

}
