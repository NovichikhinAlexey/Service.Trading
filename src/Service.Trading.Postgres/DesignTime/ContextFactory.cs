using MyJetWallet.Sdk.Postgres;

namespace Service.Trading.Postgres.DesignTime;

public class ContextFactory : MyDesignTimeContextFactory<MyContext>
{
    public ContextFactory() : base(options => new MyContext(options))
    {
    }
}