// Author: Student1
using NoSQL_Project.Helpers;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services
{
    /// <summary>
    /// Business-logic implementation for employee accounts.
    /// Hashes passwords, normalises email to lower-case, and delegates to the repository.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeService(IEmployeeRepository repo) => _repo = repo;

        /// <inheritdoc/>
        public Task<List<Employee>> GetAllAsync() => _repo.GetAllAsync();

        /// <inheritdoc/>
        public Task<Employee?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);

        /// <inheritdoc/>
        public async Task<Employee?> ValidateLoginAsync(string email, string password)
        {
            var employee = await _repo.GetByEmailAsync(email.ToLowerInvariant());
            if (employee == null) return null;
            if (!PasswordHelper.Verify(employee.PasswordHash, password)) return null;
            return employee;
        }

        /// <inheritdoc/>
        public async Task CreateAsync(CreateEmployeeViewModel model)
        {
            var employee = new Employee
            {
                Name           = model.Name.Trim(),
                Email          = model.Email.ToLowerInvariant().Trim(),
                PasswordHash   = PasswordHelper.Hash(model.Password),
                Role           = model.Role,
                DepartmentName = model.DepartmentName,
                CreatedAt      = DateTime.UtcNow
            };
            await _repo.AddAsync(employee);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(EditEmployeeViewModel model)
        {
            var existing = await _repo.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException($"Employee {model.Id} not found.");

            existing.Name           = model.Name.Trim();
            existing.Email          = model.Email.ToLowerInvariant().Trim();
            existing.Role           = model.Role;
            existing.DepartmentName = model.DepartmentName;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
                existing.PasswordHash = PasswordHelper.Hash(model.NewPassword);

            await _repo.UpdateAsync(existing);
        }

        /// <inheritdoc/>
        public Task<List<Employee>> GetByIdsAsync(IEnumerable<string> ids)
            => _repo.GetByIdsAsync(ids);

        /// <inheritdoc/>
        public Task DeleteAsync(string id) => _repo.DeleteAsync(id);

        /// <inheritdoc/>
        public Task<List<UserWithTicketCountViewModel>> GetAllWithTicketCountAsync()
            => _repo.GetAllWithTicketCountAsync();
    }
}
