using System.Data;

using Dapper;

using DapperAPI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace DapperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _context;
        private readonly IDbConnection _db = new SqliteConnection("Data Source=Employee.db");

        public EmployeeController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/Employee/GetAll")]
        public async Task<IEnumerable<Employee>> GetAll()
        {
            return _db.QueryAsync<Employee>("Select * FROM Employees").Result;
            // return await _context.Employees.ToListAsync();
        }
        
        [HttpPost]
        [Route("api/Employee/Create")]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        [HttpPut]
        [Route("api/Employee/Update")]
        public async Task<IActionResult> Update([FromBody] Employee employee)
        {
            var isEmployeeExist = await _context.Employees.FindAsync(employee.Id);

            if (isEmployeeExist is null)
                return BadRequest();
            
            isEmployeeExist.Name = employee.Name;
            isEmployeeExist.Email = employee.Email;
            isEmployeeExist.Phone = employee.Phone;

            _context.Employees.Update(isEmployeeExist);
            await _context.SaveChangesAsync();
            return Ok(isEmployeeExist);
        }

        [HttpDelete]
        [Route("api/Employee/delete")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if(employee is null)
                return BadRequest();

            _context.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
