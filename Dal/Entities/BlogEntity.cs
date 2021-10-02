using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Dal.Entities
{
    public class BlogEntity : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasOne(x => x.Owner)
                .WithMany(x => x.Blogs)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}