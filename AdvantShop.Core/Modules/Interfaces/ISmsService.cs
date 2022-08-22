namespace AdvantShop.Core.Modules.Interfaces
{
    public interface ISmsService : IModule
    {
        string SendSms(long phone, string text);
    }
}
