using System;
using AvaliacaoFinalTU.Core;
using AvaliacaoFinalTU.Feature.Produto.Validation;

namespace AvaliacaoFinalTU.Feature.Produto
{
    public class Produto : Entity
    {
        // 'set;' adicionado nas propriedades para implementação de builder.
        public string Nome { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public DateTime Fabricacao { get; set; }
        public DateTime Validade { get; set; }
        public Categoria Categoria { get; set; }
        public int Quantidade { get; set; }

        // Transformado em 'public' para instanciamento na classe de teste.
        public Produto() { }

        public Produto
            (
                string nome,
                string sku,
                DateTime fabricacao,
                DateTime validade,
                Categoria categoria,
                int quantidade)
        {
            Nome = nome;
            Sku = sku;
            Fabricacao = fabricacao;
            Validade = validade;
            Categoria = categoria;
            Quantidade = quantidade;
        }

        public bool IsValid()
        {
            ValidationResult = new ProdutoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public enum Categoria
    {
        Agua = 1,
        Refrigerante = 2,
        Isotonico = 3,
        Alcoolico = 4
    }
}
