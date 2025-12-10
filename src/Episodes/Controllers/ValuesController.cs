using Episodes.Data;
using Episodes.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Controllers;

[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ValuesController(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // GET api/values
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        bool canConnect;

        try
        {
            canConnect = await _db.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                canConnect = false,
                error = "Error while checking DB connectivity",
                exception = ex.Message
            });
        }

        if (!canConnect)
        {
            return StatusCode(500, new
            {
                canConnect = false,
                error = "Database is not reachable"
            });
        }
        
        var newUser = new User
        {
            Email = "jose@asu.edu",
            PasswordHash = "sadfklasjdflka"
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        
        var fetchedUser = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == newUser.Id);

        return Ok(new
        {
            canConnect,
            fetchedUser
        });
    }
    
    // GET api/values/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody]string value)
    {
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}