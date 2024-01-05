# OData for ASP.NET 6

## Introduction
OData is an open data protocol to add capabilities to your API to shape, order, and select portions of data (among other things). These queries are executed on the server without having to create new endpoints whenever a new capability needs to be added and without having to return large json objects that retain irrelevant information. So long as the data returned is an IQueryable, OData will be able to execute a query on the stored data.
</br></br>
The general structure should be brokers, services, and controllers. Brokers propvide the data, services manipulate the data and create a level of abstraction, and controllers offer that data. This tutorial removes the broker layer.

## Querying the Backend
As with usual API controllers, this application starts with a basic HttpGet endpoint, which can retrieve all data at `/api/Students`.
```
private readonly IStudentService studentService;

public StudentsController(IStudentService studentService)
{
    this.studentService = studentService;
}

[HttpGet]
public ActionResult<IQueryable<Student>> GetAllStudents()
{
    IQueryable<Student> retrievedStudents = this.studentService.RetrieveAllStudents();
    return Ok(retrievedStudents);
}
```
```
public class StudentService : IStudentService
    {
        public IQueryable<Student> RetrieveAllStudents()
        {
            return new List<Student> 
            {
                new Student
                {
                    Id = Guid.NewGuid(),
                    Name = "Vishu Goli",
                    Score = 200
                },
                new Student
                {
                    Id = Guid.NewGuid(),
                    Name = "Kailu Hu",
                    Score = 160
                },
                new Student
                {
                    Id = Guid.NewGuid(),
                    Name = "Sean Hobbs",
                    Score = 170
                }
            }.AsQueryable();
        }
    }
```
The implementation of the IStudentService interface is specified in Startup.cs and the controller uses dependency injection to access it at runtime.
```
public void ConfigureServices(IServiceCollection services)
{

    services.AddControllers();
    services.AddTransient<IStudentService,  StudentService>();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "dotnetfive", Version = "v1" });
    });
}
```
Without having to create a new endpoint or modify the existing controller, a request with specific query parameters can be sent to and processed/executed by the backend.
</br></br>
First, install the Microsoft.AspNetCore.OData NuGet package.
</br></br>
Then, in Startup.cs, add OData to the application services.
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers().AddOData();
    services.AddTransient<IStudentService,  StudentService>();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "dotnetfive", Version = "v1" });
    });
}
```
Make sure your endpoints allow OData queries but adding the `[EnableQuery]` decorator.
```
[HttpGet]
[EnableQuery]
public ActionResult<IQueryable<Student>> GetAllStudents()
{
    IQueryable<Student> retrievedStudents = this.studentService.RetrieveAllStudents();
    return Ok(retrievedStudents);
}
```
OData can be configured in Startup.cs to provide specific functionality. For example, the .Select() option allows you to select only specific properties from the payload.
```
services.AddControllers().AddOData(options => 
    options.Select());
```
To run a select query, use the following query parameter formatting:
```
/api/Students?$select=<property>
```
`/api/Students?$select=name` would select only the name property and return the following:
```
[
	{
		"Name": "Vishu Goli"
	},
	{
		"Name": "Kailu Hu"
	},
	{
		"Name": "Sean Hobbs"
	}
]
```
`/api/Students?$select=id` would select only the id:
```
[
	{
		"Id": "57cc99b6-5667-4570-b977-e1d4ac6aa5ad"
	},
	{
		"Id": "414ce537-dc69-420d-99ab-0a43fd323988"
	},
	{
		"Id": "266b61c2-bdb5-4312-b348-ad5532b8860e"
	}
]
```
`/api/Students?$select=id, name` can be used to return both id and name:
```
[
	{
		"Id": "bab2a718-2d60-49f5-a883-fc0a02567dac",
		"Name": "Vishu Goli"
	},
	{
		"Id": "16e15279-c165-4e59-a745-73af3cd54a60",
		"Name": "Kailu Hu"
	},
	{
		"Id": "7e18f4fe-ca7a-4018-be65-bcb8a7399284",
		"Name": "Sean Hobbs"
	}
]
```
To filter results to only include results following a specific condition, add the .Filter() option to your configuration.
```
services.AddControllers().AddOData(options => 
    options.Select().Filter());
```
To run a filter query, use the following query parameter formatting:
```
/api/Students?$filter=<property><condition>
```
For example, to select only students with the name "Vishu Goli", query the following endpoint:
```
/api/Students?$filter=name eq 'Vishu Goli'
```
*The filter is case sensitive and must use single quotes
```
[
	{
		"id": "70fcbdfd-a7cb-4e4a-808b-51aaab351ffb",
		"name": "Vishu Goli",
		"score": 200
	}
]
```
You can also filter for a score less than 165 at `/api/Students?$filter=Score lt 165`:
```
[
	{
		"id": "435c7939-9f1f-4405-ad67-3eac0a2d4e59",
		"name": "Kailu Hu",
		"score": 160
	}
]
```
Or greater than at `/api/Students?$filter=Score gt 165`:
```
[
	{
		"id": "9bd31c3b-0510-446d-81bf-e1c0479f6517",
		"name": "Vishu Goli",
		"score": 200
	},
	{
		"id": "e0e484b8-ef57-43ab-884e-36368d2843ae",
		"name": "Sean Hobbs",
		"score": 170
	}
]
```
You can also filter based on partials, such as names that start with the letter "S" at `/api/Students?$filter=startswith(name, 'S')`:
</br>
*Remember, filter parameters are case sensitive, so you must use an uppercase S to return the correct student
```
[
	{
		"id": "49cd42bb-1c9f-429d-9bfe-e3ff537933ca",
		"name": "Sean Hobbs",
		"score": 170
	}
]
```
`/api/Students?$filter=startswith(name, 'K')` will return users whose name starts with 'K':
```
[
	{
		"id": "93a388a9-38d3-41c5-adbc-ec7d0c97fce1",
		"name": "Kailu Hu",
		"score": 160
	}
]
```
You can also search for a selection of users. For example, if the name is "Sean Hobbs" OR "Vishu Goli", `/api/Students?$filter=Name in ('Sean Hobbs', 'Vishu Goli')` will return the student:
```
[
	{
		"id": "703ab9d7-9895-4b98-8d8c-752cccb5e623",
		"name": "Vishu Goli",
		"score": 200
	},
	{
		"id": "ca8ed2b0-b9e1-47c0-b4b8-d63cd008313c",
		"name": "Sean Hobbs",
		"score": 170
	}
]
```
Results can also be ordered according to a specific property. To do this, add the .OrderBy() option to your configuration.
```
services.AddControllers().AddOData(options => 
    options.Select().Filter().OrderBy());
```
`/api/Students?$orderby=score` will order all students by their score, in ascending order.
```
[
	{
		"id": "64013195-32ba-4fc4-b831-b33e6555171a",
		"name": "Kailu Hu",
		"score": 160
	},
	{
		"id": "ae086681-3077-43a0-8432-a33c15c41055",
		"name": "Sean Hobbs",
		"score": 170
	},
	{
		"id": "6ff46e71-3b47-4d49-b4dd-12d73811b988",
		"name": "Vishu Goli",
		"score": 200
	}
]
```
To switch to descending order, add the `desc` parameter `/api/Students?$orderby=score desc`.
```
[
	{
		"id": "7a595bc6-acf1-4c85-9ecc-9495cc97ff7b",
		"name": "Vishu Goli",
		"score": 200
	},
	{
		"id": "7562c5f0-bd60-462d-abc2-0fe105d89d1e",
		"name": "Sean Hobbs",
		"score": 170
	},
	{
		"id": "01a8f8d1-e6fe-44d5-88bc-7f794a94f37a",
		"name": "Kailu Hu",
		"score": 160
	}
]
```
There are many more capabilities available through the OData documentation, such as pagination or the .Expand() option which is for nested entities.

### Helpful Documentation
[https://devblogs.microsoft.com/odata/supercharging-asp-net-core-api-with-odata/](https://devblogs.microsoft.com/odata/supercharging-asp-net-core-api-with-odata/)
[https://devblogs.microsoft.com/odata/simplifying-edm-with-odata/](https://devblogs.microsoft.com/odata/simplifying-edm-with-odata/)
[https://devblogs.microsoft.com/odata/enabling-endpoint-routing-in-odata/](https://devblogs.microsoft.com/odata/enabling-endpoint-routing-in-odata/)
