using System;
using System.Threading;
using AvaliacaoFinalTU.Core.Integration;
using AvaliacaoFinalTU.Feature.Produto.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AvaliacaoFinalTU.Feature.Produto
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFactoryIntegration _factoryIntegration;
        private readonly ILogger<ProdutoService> _logger;
        private readonly IMediator _mediator;
        private RequestState requestState;

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IFactoryIntegration factoryIntegration,
            ILogger<ProdutoService> logger,
            IMediator mediator
            )
        {
            _produtoRepository = produtoRepository;
            _factoryIntegration = factoryIntegration;
            _logger = logger;
            _mediator = mediator;
        }

        public Unit RegistraPedidoProduto(Produto produto, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Requisição cancelada com sucesso.");
                requestState = RequestState.NotSupported;
                _mediator.Send(requestState);
                return Unit.Value;
            }

            if (!produto.IsValid())
            {
                _logger.LogError("Produto inválido");
                _logger.LogError(produto.ValidationResult.Errors.ToString());
                requestState = RequestState.NotSupported;
                _mediator.Send(requestState);
                throw new InvalidOperationException();
            }

            var productBD = _produtoRepository.BuscaProdutoPorSKU(produto.Sku);
            if (productBD != null)
            {
                if (produto.Fabricacao > produto.Validade)
                {
                    _logger.LogError("Fabricação do produto não pode ser maior que a validade do produto.");
                    requestState = RequestState.NotSupported;
                    _mediator.Send(requestState);
                    return Unit.Value;
                }

                _produtoRepository.RegistraPedidoProduto(produto);
                requestState = RequestState.Completed;
                _mediator.Send(requestState);
                return Unit.Value;
            }
            else
            {
                if (_factoryIntegration.ExistProduct(produto))
                {
                    _factoryIntegration.ReservaProduto(produto);
                    _produtoRepository.RegistraPedidoProduto(produto);
                    _logger.LogInformation("Produto reservado na fabrica, pedido concluido");
                    requestState = RequestState.Completed;
                    _mediator.Send(requestState);
                    return Unit.Value;
                }

                _logger.LogError("Produto indisponível no momento pedido cancelado.");
                requestState = RequestState.NotSupported;
                _mediator.Send(requestState);
                return Unit.Value;
            }
        }
    }
}
