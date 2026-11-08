// Author: Student1
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IEmployeeRepository"/>.
    /// Operates on the "employees" collection.
    /// Uses Builders&lt;T&gt;.Filter / Sort strongly typed API for all single-collection queries.
    /// Uses a raw BsonDocument pipeline for the cross-collection $lookup aggregation.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMongoCollection<Employee> _employees;
        private readonly IMongoCollection<BsonDocument> _ticketsRaw;

        public EmployeeRepository(IMongoDatabase database)
        {
            _employees  = database.GetCollection<Employee>("employees");
            // Raw BsonDocument collection for cross-collection aggregation result mapping
            _ticketsRaw = database.GetCollection<BsonDocument>("tickets");
        }

        /// <inheritdoc/>
        public async Task<List<Employee>> GetAllAsync()
        {
            return await _employees
                .Find(Builders<Employee>.Filter.Empty)
                .SortBy(e => e.Name)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Employee?> GetByIdAsync(string id)
        {
            return await _employees
                .Find(Builders<Employee>.Filter.Eq(e => e.Id, id))
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Employee>> GetByIdsAsync(IEnumerable<string> ids)
        {
            var filter = Builders<Employee>.Filter.In(e => e.Id, ids);
            return await _employees.Find(filter).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _employees
                .Find(Builders<Employee>.Filter.Eq(e => e.Email, email.ToLowerInvariant()))
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<List<UserWithTicketCountViewModel>> GetAllWithTicketCountAsync()
        {
            // $lookup from employees → tickets by reportedBy = _id,
            // then $addFields ticketCount = $size of joined array,
            // finally $project away the full array to keep the result small.
            //
            // Alternative: load all employees (GetAllAsync) and all tickets (GetAllAsync on
            // ticket repo), group-count in C#.  Simpler code, but transfers all ticket data to
            // the app server.  The $lookup keeps counting inside MongoDB — better at scale.
            var pipeline = new BsonDocument[]
            {
                new("$lookup", new BsonDocument
                {
                    { "from",         "tickets"    },
                    { "localField",   "_id"        },
                    { "foreignField", "reportedBy" },
                    { "as",           "ticketList" }
                }),
                new("$addFields", new BsonDocument
                {
                    { "ticketCount", new BsonDocument("$size", "$ticketList") }
                }),
                new("$project", new BsonDocument("ticketList", 0)),
                new("$sort",    new BsonDocument("name", 1))
            };

            var pipelineDef = PipelineDefinition<Employee, BsonDocument>.Create(pipeline);
            var results     = await _employees.Aggregate(pipelineDef).ToListAsync();

            return results.Select(doc => new UserWithTicketCountViewModel
            {
                Employee = new Employee
                {
                    Id             = doc["_id"].AsObjectId.ToString(),
                    Name           = doc.GetValue("name", BsonString.Empty).AsString,
                    Email          = doc.GetValue("email", BsonString.Empty).AsString,
                    Role           = doc.GetValue("role", BsonString.Empty).AsString,
                    DepartmentName = doc.GetValue("departmentName", BsonString.Empty).AsString,
                    CreatedAt      = doc.GetValue("createdAt", BsonDateTime.Create(DateTime.UtcNow))
                                        .ToUniversalTime()
                },
                TicketCount = doc.GetValue("ticketCount", 0).AsInt32
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task AddAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            await _employees.InsertOneAsync(employee);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Employee employee)
        {
            var filter = Builders<Employee>.Filter.Eq(e => e.Id, employee.Id);
            await _employees.ReplaceOneAsync(filter, employee);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Employee>.Filter.Eq(e => e.Id, id);
            await _employees.DeleteOneAsync(filter);
        }
    }
}
