using MediatR;
using System.Threading;

namespace AvaliacaoFinalTU.Feature.Produto.Contracts
{
    public interface IProdutoService
    {
        Unit RegistraPedidoProduto(Produto produto, CancellationToken cancellationToken);
    }
}
