//static LoggerFactory object
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>()
                .AddDatabaseDeveloperPageExceptionFilter();          

//enable CORS(Cross-Origin requests)
builder.Services.AddCors();

//Open Api Specification
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

var app = builder.Build();

//Cors Policy
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

//Swagger
app.MapSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoAPI v1"));

app.MapGet("/api/hello", () => "HelloWorld").WithName("HelloWorld");

//get all todos list
app.MapGet("/todos", async (ToDoDbContext toDoDb) => 
                       await toDoDb.ToDos.ToListAsync())
   .WithName("GetAllToDos");

//get complete todos list
app.MapGet("/todos/complete", async (ToDoDbContext toDoDb) => 
                                await toDoDb.ToDos.Where(t=>t.IsComplete).ToListAsync())
   .WithName("GetCompeleteToDos");

//get incomplete todos list
app.MapGet("/todos/incomplete", async (ToDoDbContext toDoDb) =>
                                await toDoDb.ToDos.Where(t => !t.IsComplete).ToListAsync())
   .WithName("GetIncompeleteToDos");


//find todo item by Id
app.MapGet("/todos/{id}", async (int id, ToDoDbContext toDoDb) =>
                                {
                                   if (id <= 0) 
                                        return Results.BadRequest($"Invalid {nameof(id)} param : {id}");
                                   return await toDoDb.ToDos.FindAsync(id)
                                                 is ToDo todo ? Results.Ok(todo)
                                                              : Results.NotFound();
                                })

   .WithName("GetToDoItemById")
   .Produces<ToDo>(StatusCodes.Status200OK)
   .Produces(StatusCodes.Status404NotFound)
   .Produces<string>(StatusCodes.Status400BadRequest);

//add todo item by Id
app.MapPost("/todos", async (ToDo todo, ToDoDbContext toDoDb) =>
{
    if (string.IsNullOrWhiteSpace(todo.Title))
        return Results.BadRequest($"Invalid {nameof(todo.Title)} param : {todo.Title}");
    await toDoDb.ToDos.AddAsync(todo);
    await toDoDb.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
})
   .WithName("CreateToDoItem")
   .ProducesValidationProblem()
   .Produces<string>(StatusCodes.Status400BadRequest)
   .Produces<ToDo>(StatusCodes.Status201Created);

//update todo item by Id
app.MapPut("/todos/{id}", async (int id, ToDo inputTodo, ToDoDbContext toDoDb) =>
{
    if (id <= 0)
        return Results.BadRequest($"Invalid {nameof(id)} param : {id}");

    var todo = await toDoDb.ToDos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(inputTodo.Title))
        return Results.BadRequest($"Invalid {nameof(inputTodo.Title)} param : {inputTodo.Title}");

    todo.Title = inputTodo.Title;
    todo.IsComplete = inputTodo.IsComplete;
    await toDoDb.SaveChangesAsync();

    return Results.NoContent();
})
   .WithName("UpdateToDoItemById")
   .ProducesValidationProblem()
   .Produces<string>(StatusCodes.Status400BadRequest)
   .Produces(StatusCodes.Status404NotFound)
   .Produces(StatusCodes.Status204NoContent);

//mark todo item as complete
app.MapPut("/todos/{id}/complete", async (int id, ToDoDbContext toDoDb) =>
{
    if (id <= 0)
        return Results.BadRequest($"Invalid {nameof(id)} param : {id}");

    var todo = await toDoDb.ToDos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.IsComplete = true;
    await toDoDb.SaveChangesAsync();

    return Results.NoContent();
})
   .WithName("MarkToDoItemAsComplete")
   .ProducesValidationProblem()
   .Produces<string>(StatusCodes.Status400BadRequest)
   .Produces(StatusCodes.Status404NotFound)
   .Produces(StatusCodes.Status204NoContent);

//Delete todo item by Id
app.MapDelete("/todos/{id}", async (int id,ToDoDbContext toDoDb) =>
{
    if (id <= 0)
        return Results.BadRequest($"Invalid {nameof(id)} param : {id}");

    var todo = await toDoDb.ToDos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    toDoDb.Remove(todo);
    await toDoDb.SaveChangesAsync();
    return Results.NoContent();
})
   .WithName("DeleteToDoItemById")
   .Produces<string>(StatusCodes.Status400BadRequest)
   .Produces(StatusCodes.Status404NotFound)
   .Produces(StatusCodes.Status204NoContent);

//Delete All todos 
app.MapDelete("/todos/delete-all", async (ToDoDbContext toDoDb) =>
                      Results.Ok(await toDoDb.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[ToDos];")))
   .WithName("DeleteAllTodos")
   .Produces<int>(StatusCodes.Status200OK)
   .Produces(StatusCodes.Status500InternalServerError);

app.Run();


