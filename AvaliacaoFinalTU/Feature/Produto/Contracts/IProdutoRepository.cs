using AvaliacaoFinalTU.Core;

namespace AvaliacaoFinalTU.Feature.Produto.Contracts
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Produto BuscaProdutoPorSKU(string sku);
        void RegistraPedidoProduto(Produto produto);
    }
}
