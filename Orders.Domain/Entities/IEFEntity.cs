using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Entities
{
    public interface IEFEntity
    {
        public Guid ruid { get; set; }
        public DateTime? created_datetime { get; set; }
        public string? created_by { get; set; }
        public DateTime? modified_datetime { get; set; }
        public string? modified_by { get; set; }
    }
}
