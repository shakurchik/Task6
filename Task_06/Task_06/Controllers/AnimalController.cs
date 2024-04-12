using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Task_06.Models;
using Task_06.Models.DTOs;

namespace Task_06.Controllers;
[ApiController]
[Route("api/[controller]")]

public class AnimalController:ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals()
    {
        
//Open connectio
        
       using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
//Creae command
        using SqlCommand command= new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT*FROM Animal;";
        
        //Execute commamd
        var reader = command.ExecuteReader();
        var animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        
        while (reader.Read())
        {
animals.Add(new Animal()
{
    IdAnimal = reader.GetInt32(idAnimalOrdinal),
    Name=reader.GetString(nameOrdinal)
});
        }

        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
using SqlConnection connection= new SqlConnection(_configuration.GetConnectionString("Default"));
connection.Open();
using SqlCommand command = new SqlCommand();
command.Connection = connection;
command.CommandText = "INSERT into Animal VALUES(@animalName,'','','')";
command.Parameters.AddWithValue("@animalName", animal.Name);

command.ExecuteNonQuery();
return Created(" ",null);

    }
}