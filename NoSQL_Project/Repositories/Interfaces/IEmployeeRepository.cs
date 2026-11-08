// Author: Student1
using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories.Interfaces
{
    /// <summary>
    /// Contract for all employee (user account) data access operations.
    /// All methods are asynchronous to avoid blocking the ASP.NET Core thread pool.
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>Returns all employees sorted by name ascending.</summary>
        /// <returns>Full list of Employee documents.</returns>
        Task<List<Employee>> GetAllAsync();

        /// <summary>Returns a single employee by their ObjectId string.</summary>
        /// <param name="id">MongoDB ObjectId as string.</param>
        /// <returns>The matching Employee, or <c>null</c> if not found.</returns>
        Task<Employee?> GetByIdAsync(string id);

        /// <summary>Returns employees whose Id is in the provided list. Used for batched display joins.</summary>
        /// <param name="ids">Collection of ObjectId strings to look up.</param>
        /// <returns>Employees found; silently omits missing ids.</returns>
        Task<List<Employee>> GetByIdsAsync(IEnumerable<string> ids);

        /// <summary>Finds an employee by email address (unique index).</summary>
        /// <param name="email">E-mail to look up.</param>
        /// <returns>The Employee, or <c>null</c>.</returns>
        Task<Employee?> GetByEmailAsync(string email);

        /// <summary>
        /// Returns all employees each paired with the total number of tickets they reported.
        /// Uses a MongoDB $lookup + $size aggregation so the count is computed in the database,
        /// avoiding pulling all ticket documents into application memory.
        /// Alternative: load employees + all ticket reportedBy IDs in C# and group-count —
        /// simpler but transfers more data; $lookup is better here.
        /// </summary>
        /// <returns>Employees with ticket counts, sorted by name.</returns>
        Task<List<UserWithTicketCountViewModel>> GetAllWithTicketCountAsync();

        /// <summary>Inserts a new employee document.</summary>
        /// <param name="employee">Fully populated Employee; Id is set by MongoDB on insert.</param>
        Task AddAsync(Employee employee);

        /// <summary>Replaces the employee document identified by employee.Id.</summary>
        /// <param name="employee">Updated Employee; Id must match an existing document.</param>
        Task UpdateAsync(Employee employee);

        /// <summary>Deletes the employee document with the given id.</summary>
        /// <param name="id">ObjectId string of the employee to remove.</param>
        Task DeleteAsync(string id);
    }
}
