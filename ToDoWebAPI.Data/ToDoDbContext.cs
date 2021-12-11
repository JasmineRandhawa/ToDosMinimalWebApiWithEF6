namespace ToDoWebAPI.Data;
public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options) { }

    public DbSet<ToDo> ToDos => Set<ToDo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*modelBuilder.Entity<ToDo>()
                    .Property(t => t.Name)
                    .HasMaxLength(100);*/

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToDoDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()
                                                                                 .AddFilter(level => level >= LogLevel.Information)))
                          .UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

        }
    }
}

public class ToDoDbContextFactory:IDesignTimeDbContextFactory<ToDoDbContext>
{
    public ToDoDbContext CreateDbContext(string[]? args = null)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

        var optionsBuilder = new DbContextOptionsBuilder<ToDoDbContext>();

        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()
                                                                             .AddFilter(level => level >= LogLevel.Information)))
                      .UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

        return new ToDoDbContext(optionsBuilder.Options);
    }
}
