﻿using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using MediatR;
using Orders.Domain.Repositories;
using Orders.Infra.Database.Extensions;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Storage;

namespace Orders.Infra.Database
{
    public class OrdersDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        private IDbContextTransaction _currentTransaction;

        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
        {
        }
       
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            System.Diagnostics.Debug.WriteLine("Items DbContext::ctor ->" + this.GetHashCode());
        }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;


        public DbSet<OrderAggregateRoot> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderAggregateRoot>()
                .Property(p => p.Items)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                               v => JsonConvert.DeserializeObject<IDictionary<string, decimal>>(v));
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // 
            // Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }
 
        public override int SaveChanges()
        {
            try
            {
               foreach (var entry in this.ChangeTracker.Entries())
                {
                    if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                    {
                        entry.Property("created_datetime").CurrentValue = DateTime.UtcNow;
                        entry.Property("ruid").CurrentValue = Guid.NewGuid();
                    }

                    if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                    {
                        entry.Property("modified_datetime").CurrentValue = DateTime.UtcNow;
                    }
                }

                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
 
        }
    }
}