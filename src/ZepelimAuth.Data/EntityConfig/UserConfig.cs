

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZepelimAuth.Business.Models;

namespace ZAuth.Database.EntityConfig
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.Guid).IsUnique();

            builder.Property(p => p.Nome);
            builder.Property(p => p.Email).IsRequired();
            builder.Property(p => p.DocumentoUsuario);
            builder.Property(p => p.DocumentoEmpresa);
            builder.Property(p => p.RazaoSocial);
            builder.Property(p => p.POS);
            builder.Property(p => p.HUB);
            builder.Property(p => p.LOG);
            builder.Property(p => p.Role).IsRequired();

            // builder.ToTable("user");
        }
    }
}
