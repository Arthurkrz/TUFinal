using FluentValidation.Results;
using System;

namespace AvaliacaoFinalTU.Core
{
    public class Entity
    {
        public Guid Id { get; set; }
        public ValidationResult ValidationResult { get; set; } = null!;
    }
}
