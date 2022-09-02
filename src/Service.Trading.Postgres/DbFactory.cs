using Microsoft.EntityFrameworkCore;

namespace Service.Trading.Postgres;

public class DbFactory: IDbFactory
{
    private readonly DbContextOptionsBuilder<MyContext> _dbContextOptionsBuilder;
    
    public DbFactory(DbContextOptionsBuilder<MyContext> dbContextOptionsBuilder)
    {
        _dbContextOptionsBuilder = dbContextOptionsBuilder;
    }

    public MyContext Context()
    {
        return new MyContext(_dbContextOptionsBuilder.Options);
    }
}

public interface IDbFactory
{
    MyContext Context();
}