namespace ToDoWebAPI.Data;

 public class ToDo
  {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
 }


 public class ToDoEntityTypeConfiguration:IEntityTypeConfiguration<ToDo>
 {
   public void Configure(EntityTypeBuilder<ToDo> entityBuilder)
   {
        entityBuilder.Property(t => t.Title)
                        .HasMaxLength(100);
   }
 }

