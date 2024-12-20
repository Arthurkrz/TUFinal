using AvaliacaoFinalTU.Feature.Produto;
using Bogus;
using System;

namespace AvaliacaoFinalTU.Tests.Builders
{
    internal class ProdutoBuilder
    {
        private readonly Produto _produto;
        private readonly Faker _faker;

        public ProdutoBuilder()
        {
            _produto = new Produto();
            _faker = new Faker();
        }

        public Produto Build() => _produto;

        public ProdutoBuilder NomeValido(string nome = null)
        {
            if (nome == null)
                nome = _faker.Name.FullName();

            _produto.Nome = nome;
            return this;
        }

        public ProdutoBuilder SKUValido(string sku = null)
        {
            if (sku == null)
                sku = _faker.Random.Word();

            _produto.Sku = sku;
            return this;
        }

        public ProdutoBuilder FabricacaoValida(DateTime fabricacao = default)
        {
            if (fabricacao == default)
                fabricacao = _faker.Date.Past(1, DateTime.Now);

            _produto.Fabricacao = fabricacao;
            return this;
        }

        public ProdutoBuilder ValidadeValida(DateTime validade = default)
        {
            if (validade == default)
                validade = _faker.Date.Future(1, DateTime.Now);

            _produto.Validade = validade;
            return this;
        }

        public ProdutoBuilder CategoriaValida(Categoria categoria = default)
        {
            if (categoria == default)
                categoria = _faker.PickRandom<Categoria>();

            _produto.Categoria = categoria;
            return this;
        }

        public ProdutoBuilder QuantidadeValida(int qnt = 0)
        {
            if (qnt == 0)
                qnt = _faker.Random.Int(1, 10000);

            _produto.Quantidade = qnt;
            return this;
        }

        public ProdutoBuilder ProdutoVencido
                              (DateTime fabricacao = default, 
                               DateTime validade = default)
        {
            if (fabricacao == default)
                fabricacao = DateTime.Now.AddMonths(5);

            if (validade == default)
                validade = DateTime.Now.AddMonths(4);

            _produto.Fabricacao = fabricacao;
            _produto.Validade = validade;
            return this;
        }

        public ProdutoBuilder NomeInvalido(string nome = null)
        {
            if (nome == null)
                nome = "a";

            _produto.Nome = nome;
            return this;
        }

        public ProdutoBuilder FabricacaoInvalida(DateTime fabricacao = default)
        {
            if (fabricacao == default)
                fabricacao = DateTime.MinValue;

            _produto.Fabricacao = fabricacao;
            return this;
        }

        public ProdutoBuilder ValidadeInvalida(DateTime validade = default)
        {
            if (validade == default)
                validade = _faker.Date.Between(DateTime.Now, DateTime.Now.AddMonths(2));

            _produto.Validade = validade;
            return this;
        }
    }
}