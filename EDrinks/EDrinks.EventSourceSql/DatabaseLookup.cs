using System.Collections.Generic;
using System.IO;
using System.Linq;
using EDrinks.Common;
using EDrinks.EventSourceSql.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EDrinks.EventSourceSql
{
    public interface IDatabaseLookup
    {
        string GetDatabase(string userId);
    }

    public class DatabaseLookup : IDatabaseLookup
    {
        private Dictionary<string, string> _userIdToDatabase = new Dictionary<string, string>();
        private readonly SystemContext _systemContext;
        private readonly string _dbBaseDirectory;

        private readonly object contextLock = new object();

        public DatabaseLookup(IConfiguration configuration)
        {
            _dbBaseDirectory = configuration.GetValue<string>("Data:BaseDir");
            var systemDb = configuration.GetValue<string>("Data:SystemDb");
            var systemDbPath = Path.Join(_dbBaseDirectory, systemDb);

            var systemOptions = new DbContextOptionsBuilder<SystemContext>()
                .UseSqlite($"Data Source={systemDbPath}")
                .Options;
            _systemContext = new SystemContext(systemOptions);
            _systemContext.Database.EnsureCreated();
        }

        public string GetDatabase(string userId)
        {
            // 1. check if userId already in dictionary
            if (_userIdToDatabase.ContainsKey(userId))
            {
                return _userIdToDatabase[userId];
            }

            lock (contextLock)
            {
                // 2. else check if userId is already in System DB
                var user = _systemContext.Users.FirstOrDefault(e => e.AuthIdentifier == userId);
                string fullPath = "";
                if (user == null)
                {
                    // 3. if not, create new DB for user
                    fullPath = CreateUserDb(userId);
                }
                else
                {
                    fullPath = Path.Join(_dbBaseDirectory, user.EventDbFile);
                }

                if (!_userIdToDatabase.ContainsKey(userId))
                {
                    _userIdToDatabase.Add(userId, fullPath);
                }

                return fullPath;
            }
        }

        private string CreateUserDb(string userId)
        {
            var dbFile = "events_" + HashUtil.CreateMd5(userId) + ".db";

            _systemContext.Users.Add(new User()
            {
                AuthIdentifier = userId,
                EventDbFile = dbFile
            });
            _systemContext.SaveChanges();

            var fullPath = Path.Join(_dbBaseDirectory, dbFile);

            var options = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite($"Data Source={fullPath}")
                .Options;

            var domainContext = new DomainContext(options);
            domainContext.Database.EnsureCreated();

            return fullPath;
        }
    }
}