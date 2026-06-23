using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Events
{
    public record UserRegisteredEvent(Guid UserId);
}
