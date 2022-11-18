using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Uppgift2.Services
{
    public interface IDbService
    {
        Dictionary<string, string> CollectionNames { get; }

        T Add<T>(T document, string collectionName);
        Task<bool> DeleteRecordAsync<T>(Expression<Func<T, bool>> filter, string collectionName);
        Task<T?> FindRecordAsync<T>(Expression<Func<T, bool>> filter, string collectionName);
        Task<List<T>> GetAllAsync<T>(string collectionName);
    }

    public class DbService : IDbService
    {
        private readonly IMongoDatabase _database;

        public DbService(IOptions<DbConfig> options)
        {
            var e = options.Value;
            _database = new MongoClient(MongoClientSettings.FromConnectionString(options.Value.ConnectionString)).GetDatabase(options.Value.DatabaseName);
            CollectionNames = options.Value.CollectionNames;
        }

        public Dictionary<string, string> CollectionNames { get; private set; }

        /// <summary>
        /// Adds a record to the database. 
        /// </summary>
        /// <typeparam name="T">A <c>Data</c> model.</typeparam>
        /// <param name="document">The instance of the Data model to be added.</param>
        /// <param name="collectionName">The name of MongoDb collection.</param>
        /// <returns>An object of the data model class that was added (including its Id).</returns>
        public T Add<T>(T document, string collectionName)
        {
            var mongoCollection = _database.GetCollection<T>(collectionName);
            mongoCollection.InsertOne(document);
            return document;
        }

        /// <summary>
        /// Try to find a record in the database.
        /// </summary>
        /// <typeparam name="T">A <c>Data</c> model.</typeparam>
        /// <param name="filter">A callback function for the search.</param>
        /// <param name="collectionName">The name of MongoDb collection.</param>
        /// <returns>An object of the data model class. If no records were found, returns 'default' value.</returns>
        public async Task<T?> FindRecordAsync<T>(Expression<Func<T, bool>> filter, string collectionName)
        {
            var asyncCursor = await _database.GetCollection<T>(collectionName).FindAsync<T>(filter);
            return asyncCursor.FirstOrDefault();
        }

        /// <summary>
        /// Try to find multiple records in the database.
        /// </summary>
        /// <typeparam name="T">A <c>Data</c> model.</typeparam>
        /// <param name="collectionName">The name of MongoDb collection.</param>
        /// <returns>A list of objects of the data model class. If no records were found, returns an empty list.</returns>
        public async Task<List<T>> GetAllAsync<T>(string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return await collection.AsQueryable().ToListAsync<T>();
        }

        /// <summary>
        /// Delete a record from the database.
        /// </summary>
        /// <typeparam name="T">A <c>Data</c> model.</typeparam>
        /// <param name="filter">A callback function for the search.</param>
        /// <param name="collectionName">The name of MongoDb collection.</param>
        /// <returns><c>True</c> if record was found and deleted.</returns>
        public async Task<bool> DeleteRecordAsync<T>(Expression<Func<T, bool>> filter, string collectionName)
        {
            var v = await _database.GetCollection<T>(collectionName).DeleteOneAsync(filter);
            return v.IsAcknowledged;
        }
    }

    /// <summary>
    /// Used with WebApplicationBuilder to bind values for Mongo database configurations and the names of its Collections.
    /// </summary>
    public class DbConfig
    {
        public DbConfig()
        {
            ConnectionString = "";
            DatabaseName = "";
            CollectionNames = new();
        }

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public Dictionary<string, string> CollectionNames { get; set; }
    }
}
