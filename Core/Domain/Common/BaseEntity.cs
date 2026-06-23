using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    //tüm entitylerde olması gerken ortak base entityimiz
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; } = DateTime.UtcNow;
    }
}
