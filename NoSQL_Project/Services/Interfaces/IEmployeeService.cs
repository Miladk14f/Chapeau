// Author: Student1
using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    /// <summary>Business-logic layer for employee accounts.</summary>
    public interface IEmployeeService
    {
        /// <summary>Returns all employees.</summary>
        Task<List<Employee>> GetAllAsync();

        /// <summary>Returns a single employee by id, or null.</summary>
        Task<Employee?> GetByIdAsync(string id);

        /// <summary>
        /// Validates login credentials. Returns the employee on success, null on failure.
        /// Password check uses constant-time comparison via PasswordHelper.
        /// </summary>
        Task<Employee?> ValidateLoginAsync(string email, string password);

        /// <summary>Creates a new employee account from the form model.</summary>
        Task CreateAsync(CreateEmployeeViewModel model);

        /// <summary>Updates an existing employee. Optionally changes password.</summary>
        Task UpdateAsync(EditEmployeeViewModel model);

        /// <summary>Deletes an employee account.</summary>
        Task DeleteAsync(string id);

        /// <summary>Returns employees matching the given ids (used for log changedBy lookups).</summary>
        Task<List<Employee>> GetByIdsAsync(IEnumerable<string> ids);

        /// <summary>Returns all employees each paired with their reported ticket count.</summary>
        Task<List<UserWithTicketCountViewModel>> GetAllWithTicketCountAsync();
    }
}
