using FluentValidation;
using System;

namespace AvaliacaoFinalTU.Feature.Produto.Validation
{
    public class ProdutoValidation : AbstractValidator<Produto>
    {
        public ProdutoValidation()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(150)
                .WithMessage("Nome do produto não atende aos requisitos minimos");

            RuleFor(x => x.Sku).NotEmpty().NotNull().WithMessage("Código sku obrigatório");
            RuleFor(x => x.Fabricacao).Must(x => x != DateTime.MinValue);
            RuleFor(x => x.Validade).GreaterThan(DateTime.Now.AddMonths(3)).WithMessage("Produto fora da data de validade minima esperada.");
            RuleFor(x => x.Categoria).IsInEnum().WithMessage("Categoria escolhida não encontrada no sistema");
        }
    }
}