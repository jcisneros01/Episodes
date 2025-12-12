using Episodes.Data;
using Episodes.Models.Entities;
using Episodes.Services.Tmdb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Controllers;

[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    private readonly ITmdbClient _client;

    public ValuesController(ApplicationDbContext db, ITmdbClient client)
    {
        _db = db;
        _client = client;
    }
    
    // GET api/values
    [HttpGet]
    public async Task<IActionResult> Get(string query)
    {
        var tvShows = await _client.SearchTvShowsAsync("breaking bad");
        return Ok(new
        {
          tvShows
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