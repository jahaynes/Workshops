using Acid.Db;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Acid.Actions;

public class RedistributeWealthSql
{
    private readonly MyDbContext _dbContext;

    public RedistributeWealthSql(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Run(string query)
    {
        var done = false;
        while (!done)
        {
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync(query);
                Console.WriteLine("Executed!");
                done = true;
            }
            catch (SqlException ex)
            {
                done |= ex.Number != 120;
                if (!done)
                {
                    Console.WriteLine("Deadlocked.  Will try again");
                }
            }
        }
    }
}