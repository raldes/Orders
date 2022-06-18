using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Orders.Domain.Entities
{
    public abstract class EFEntity : IEFEntity
    {
        [JsonIgnore]
        public Guid ruid { get; set; }
        [JsonIgnore]
        public DateTime? created_datetime { get; set; }
        [JsonIgnore]
        public string? created_by { get; set; }
        [JsonIgnore]
        public DateTime? modified_datetime { get; set; } 
        [JsonIgnore]
        public string? modified_by { get; set; }

        //domain events managment
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

    }
}
