﻿//Listing 5.1 A minimal API that uses a value from the URL
using System.Xml.Linq;
using System;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

var people = new List<Person>                             
{                                                         
    new("Tom", "Hanks"),                                  
    new("Denzel", "Washington"),                          
    new("Leondardo", "DiCaprio"),                         
    new("Al", "Pacino"),                                  
    new("Morgan", "Freeman"),                             
};                                                        
 
app.MapGet("/person/{name}", (string name) =>             
    people.Where(p => p.FirstName.StartsWith(name)));     
 
app.Run();

public record Person(string FirstName, string LastName);