using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Common.Data;

public class LinkVaultDbContext(DbContextOptions<LinkVaultDbContext> options)
    : DbContext(options) { }
