using Marten;
using Marten.Schema;
using Wolverine;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var martenCn = builder.Configuration.GetConnectionString("Marten")!;

builder.Services.AddMarten(opts =>
    {
        opts.Connection(martenCn);

        opts.Schema.For<User>()
            .UniqueIndex(UniqueIndexType.Computed, u => u.Email)
            .Index(u => u.RatePayersAssociationRef.Id)
            .Index(u => u.RatePayersAssociationRef.MunicipalityRef.Id)
            .Duplicate(x => x.JoinedUtc, pgType: "varchar(50)", notNull: true);

    })
    .OptimizeArtifactWorkflow()
    .UseLightweightSessions()
    .IntegrateWithWolverine("wolverine_messages");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWolverine(opts =>
{
    // TODO: uncomment the below line to see NO DDL statement for User table, especially NO Index DDL statements.

    //opts.Services.AddMarten(martenCn)
    //    .OptimizeArtifactWorkflow()
    //    .UseLightweightSessions()
    //    .IntegrateWithWolverine();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("utils/db-schema-ddl", (IDocumentStore store) => store.Storage.ToDatabaseScript());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string JoinedUtc { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StreetAddress { get; set; }
    public string MobileNumber { get; set; }
    public string MunicipalAccountNumber { get; set; }
    public RpaRef RatePayersAssociationRef { get; set; }
    public string Area { get; set; }
}

public record DocumentRef(string Id, string Descriptor);

public record RpaRef(string Id, string Descriptor, DocumentRef MunicipalityRef) : DocumentRef(Id, Descriptor);
