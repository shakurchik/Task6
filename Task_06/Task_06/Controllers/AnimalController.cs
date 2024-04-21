using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Task_06.Models;
using Task_06.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Task_06.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AnimalController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals(string orderBy = "name")
    {
        string sqlOrderBy = orderBy.ToLower() switch
        {
            "name" => "Name",
            "description" => "Description",
            "category" => "Category",
            "area" => "Area",
            _ => "Name" 
        };

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        using SqlCommand command = new SqlCommand($"SELECT * FROM Animal ORDER BY {sqlOrderBy} ASC", connection);
        
        var reader = command.ExecuteReader();
        var animals = new List<Animal>();

        while (reader.Read())
        {
            animals.Add(new Animal
            {
                IdAnimal = reader.GetInt32("IdAnimal"),
                Name = reader.GetString("Name"),
                Description = reader.GetString("Description"),
                Category = reader.GetString("Category"),
                Area = reader.GetString("Area")
            });
        }

        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal([FromBody] AddAnimal animal)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        using SqlCommand command = new SqlCommand("INSERT INTO Animal (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)", connection);
        command.Parameters.AddWithValue("@Name", animal.Name);
        command.Parameters.AddWithValue("@Description", animal.Description ?? string.Empty); 
        command.Parameters.AddWithValue("@Category", animal.Category ?? string.Empty);
        command.Parameters.AddWithValue("@Area", animal.Area ?? string.Empty);

        command.ExecuteNonQuery();
        int newId = Convert.ToInt32(command.ExecuteScalar()); 
        return CreatedAtAction(nameof(GetAnimalById), new { id = newId }, animal);
    }
    [HttpGet("{id}")]
    public IActionResult GetAnimalById(int id)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Animal WHERE IdAnimal = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var animal = new Animal
                {
                    IdAnimal = reader.GetInt32("IdAnimal"),
                    Name = reader.GetString("Name"),
                    Description = reader.GetString("Description"),
                    Category = reader.GetString("Category"),
                    Area = reader.GetString("Area")
                };
                return Ok(animal);
            }
            return NotFound();
        }
    }

    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, [FromBody] UpdateAnimal animal)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        using SqlCommand command = new SqlCommand("UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal", connection);
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);
        command.Parameters.AddWithValue("@Name", animal.Name);
        command.Parameters.AddWithValue("@Description", animal.Description ?? string.Empty);
        command.Parameters.AddWithValue("@Category", animal.Category ?? string.Empty);
        command.Parameters.AddWithValue("@Area", animal.Area ?? string.Empty);

        int affectedRows = command.ExecuteNonQuery();
        if (affectedRows == 0)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        using SqlCommand command = new SqlCommand("DELETE FROM Animal WHERE IdAnimal = @IdAnimal", connection);
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

        int affectedRows = command.ExecuteNonQuery();
        if (affectedRows == 0)
        {
            return NotFound();
        }
        return NoContent();
    }
}

