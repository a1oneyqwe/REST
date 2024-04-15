using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

var animalList = new List<Animal>
{
    new Animal { ID = 1, Name = "Patron", Sort = "Dog", WeightValue = 30, Fur = "Brown" },
    new Animal { ID = 2, Name = "Stepan", Sort = "Cat", WeightValue = 4, Fur = "White" }
};

var visitList = new List<Visit>
{
    new Visit { ID = 1, AnimalID = 1, AppointmentDate = "2022-02-22", Synopsis = "Health injuries", CostValue = 37 }
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var application = builder.Build();

application.UseSwagger();
application.UseSwaggerUI();

application.MapGet("/animals", () => Results.Ok(animalList));

application.MapGet("/animals/{id}", (int id) =>
{
    var specimen = animalList.FirstOrDefault(a => a.ID == id);
    return specimen != null ? Results.Ok(specimen) : Results.NotFound();
});

application.MapPost("/animals", (Animal animal) =>
{
    animal.ID = animalList.Count + 1;
    animalList.Add(animal);
    return Results.Created($"/animals/{animal.ID}", animal);
});

application.MapPut("/animals/{id}", (int id, Animal animal) =>
{
    var existingSpecimen = animalList.FirstOrDefault(a => a.ID == id);
    if (existingSpecimen != null)
    {
        existingSpecimen.Name = animal.Name;
        existingSpecimen.Sort = animal.Sort;
        existingSpecimen.WeightValue = animal.WeightValue;
        existingSpecimen.Fur = animal.Fur;
        return Results.Ok(existingSpecimen);
    }
    return Results.NotFound();
});

application.MapDelete("/animals/{id}", (int id) =>
{
    var specimen = animalList.FirstOrDefault(a => a.ID == id);
    if (specimen != null)
    {
        animalList.Remove(specimen);
        return Results.NoContent();
    }
    return Results.NotFound();
});

application.MapGet("/animals/{id}/visits", (int id) =>
{
    var specimenVisits = visitList.Where(v => v.AnimalID == id).ToList();
    return Results.Ok(specimenVisits);
});

application.MapPost("/animals/{id}/visits", (int id, Visit visit) =>
{
    visit.ID = visitList.Count + 1;
    visit.AnimalID = id;
    visitList.Add(visit);
    return Results.Created($"/animals/{id}/visits/{visit.ID}", visit);
});

application.Run();

public class Animal
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Sort { get; set; }
    public double WeightValue { get; set; }
    public string Fur { get; set; }
}

public class Visit
{
    public int ID { get; set; }
    public int AnimalID { get; set; }
    public string AppointmentDate { get; set; }
    public string Synopsis { get; set; }
    public decimal CostValue { get; set; }
}
