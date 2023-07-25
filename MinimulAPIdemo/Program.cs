var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserProvider, UserProvider>();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/api/user", ([FromServices] IUserProvider userprovider) => userprovider.Get());

app.MapGet("/api/user/{id}", ([FromServices] IUserProvider userprovider,int id) => userprovider.Get(id));

app.MapPost("/api/user", ([FromServices] IUserProvider userprovider,User user) =>
{
    var addedUser = userprovider.Add(user);
    return Results.Created($"/api/user/{addedUser.Id}", addedUser);
});

app.MapPut("/api/user/{id}", ([FromServices] IUserProvider userprovider,User user) =>
{
    //user.Id = user.Id;
    var updatedUser = userprovider.Update(user);
    if (updatedUser != null)
    {
        return Results.Ok(updatedUser);
    }
    return Results.NotFound();
});

app.MapDelete("/api/user/{id}", ([FromServices] IUserProvider userprovider, int id) =>
{
    var result = userprovider.Delete(id);
    if (result)
    {
        return Results.Ok(result);
    }
    return Results.NotFound();
});

app.Run();