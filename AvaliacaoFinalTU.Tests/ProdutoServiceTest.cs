using System;
using Xunit;
using AvaliacaoFinalTU.Tests.Builders;
using Bogus;
using Moq;
using AvaliacaoFinalTU.Feature;
using AvaliacaoFinalTU.Feature.Produto;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using AvaliacaoFinalTU.Core.Integration;
using AvaliacaoFinalTU.Feature.Produto.Contracts;
using MediatR;

namespace AvaliacaoFinalTU.Tests
{
    public class ProdutoServiceTest
    {
        private readonly ProdutoService _sut;
        private readonly Mock<IProdutoRepository> _mockRepository;
        private readonly Mock<IFactoryIntegration> _mockFactoryIntegration;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<ProdutoService>> _mockLogger;

        public ProdutoServiceTest()
        {
            _mockRepository = new Mock<IProdutoRepository>();
            _mockFactoryIntegration = new Mock<IFactoryIntegration>();
            _mockLogger = new Mock<ILogger<ProdutoService>>();
            _mockMediator = new Mock<IMediator>();
            _sut = new ProdutoService(_mockRepository.Object,
                                      _mockFactoryIntegration.Object,
                                      _mockLogger.Object,
                                      _mockMediator.Object);
        }

        [Fact]
        public void RegistraPedidoProduto_CancelaComSucesso_QuandoCancelamentoRequisitado()
        {
            // Arrange
            CancellationToken cancellationToken = new CancellationToken(true);
            Produto produto = new ProdutoBuilder().NomeValido()
                                                  .SKUValido()
                                                  .FabricacaoValida()
                                                  .ValidadeValida()
                                                  .CategoriaValida()
                                                  .Build();

            // Act
            var result = _sut.RegistraPedidoProduto(produto, cancellationToken);

            // Assert
            _mockMediator.Verify(m => m.Send(RequestState.NotSupported, 
                                             cancellationToken), Times.Once);

            Assert.Equal(Unit.Value, result);
        }


        [Theory]
        [MemberData(nameof(InvalidProductGenerator))]
        public void RegistraPedidoProduto_EmiteErros_QuandoProdutoInvalido
                    (CancellationToken cancellationToken, Produto produto)
        {
            // Act
            var exception = Record.Exception(() => 
                            _sut.RegistraPedidoProduto
                            (produto, cancellationToken));

            // Assert
            _mockMediator.Verify(x => x.Send(RequestState.NotSupported, 
                                             cancellationToken), Times.Once);

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void RegistraPedidoProduto_EmiteErro_QuandoFabricacaoMaiorQueValidade()
        {
            // Arrange
            CancellationToken cancellationToken = new CancellationToken(false);
            Produto produtoVencido = new ProdutoBuilder().NomeValido()
                                                         .SKUValido()
                                                         .ProdutoVencido()
                                                         .CategoriaValida()
                                                         .Build();

            _mockRepository.Setup(x => x.BuscaProdutoPorSKU(produtoVencido.Sku))
                                        .Returns(produtoVencido);

            // Act
            var result = _sut.RegistraPedidoProduto(produtoVencido, cancellationToken);

            // Assert
            _mockMediator.Verify(x => x.Send(RequestState.NotSupported,
                                             cancellationToken), Times.Once);

            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public void RegistraPedidoProduto_RegistraComSucesso_QuandoProdutoValidoNaoVencido()
        {
            // Arrange
            CancellationToken cancellationToken = new CancellationToken(false);
            Produto produto = new ProdutoBuilder().NomeValido()
                                                  .SKUValido()
                                                  .FabricacaoValida()
                                                  .ValidadeValida()
                                                  .CategoriaValida()
                                                  .Build();

            _mockRepository.Setup(x => x.BuscaProdutoPorSKU(produto.Sku))
                                        .Returns(produto);

            // Act
            var result = _sut.RegistraPedidoProduto(produto, cancellationToken);

            // Assert
            _mockRepository.Verify(x => x.RegistraPedidoProduto(produto), Times.Once);
            _mockMediator.Verify(x => x.Send(RequestState.Completed, 
                                             cancellationToken), Times.Once);

            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public void RegistraPedidoProduto_RegistraComSucesso_QuandoProdutoValidoNaoExisteDBMasExisteFabrica()
        {
            // Arrange
            CancellationToken cancellationToken = new CancellationToken(false);
            Produto produto = new ProdutoBuilder().NomeValido()
                                                  .SKUValido()
                                                  .FabricacaoValida()
                                                  .ValidadeValida()
                                                  .CategoriaValida()
                                                  .Build();

            _mockRepository.Setup(x => x.BuscaProdutoPorSKU(produto.Sku))
                           .Returns((Produto)null);

            _mockFactoryIntegration.Setup(x => x.ExistProduct(produto))
                                   .Returns(true);

            // Act
            var result = _sut.RegistraPedidoProduto(produto, cancellationToken);

            // Assert
            _mockFactoryIntegration.Verify(x => x.ReservaProduto(produto), 
                                          Times.Once);

            _mockRepository.Verify(x => x.RegistraPedidoProduto(produto), 
                                          Times.Once);

            _mockMediator.Verify(x => x.Send(RequestState.Completed, 
                                             cancellationToken), Times.Once);
            
            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public void RegistraPedidoProduto_EmiteErro_QuandoProdutoInexistenteEmFabrica()
        {
            // Arrange
            CancellationToken cancellationToken = new CancellationToken(false);
            Produto produto = new ProdutoBuilder().NomeValido()
                                                  .SKUValido()
                                                  .FabricacaoValida()
                                                  .ValidadeValida()
                                                  .CategoriaValida()
                                                  .Build();

            _mockRepository.Setup(x => x.BuscaProdutoPorSKU(produto.Sku))
                           .Returns((Produto)null);

            _mockFactoryIntegration.Setup(x => x.ExistProduct(produto))
                                   .Returns(false);

            // Act
            var result = _sut.RegistraPedidoProduto(produto, cancellationToken);

            // Assert          
            _mockMediator.Verify(x => x.Send(RequestState.NotSupported, 
                                             cancellationToken), Times.Once);
            
            Assert.Equal(Unit.Value, result);
        }

        public static IEnumerable<object[]> InvalidProductGenerator()
        {
            yield return new object[]
            {
                new CancellationToken(false),
                new ProdutoBuilder().NomeInvalido()
                                    .SKUValido()
                                    .FabricacaoValida()
                                    .ValidadeValida()
                                    .CategoriaValida()
                                    .Build()
            };

            yield return new object[]
            {
                new CancellationToken(false),
                new ProdutoBuilder().NomeValido()
                                    .FabricacaoValida()
                                    .ValidadeValida()
                                    .CategoriaValida()
                                    .Build()
            };

            yield return new object[]
            {
                new CancellationToken(false),
                new ProdutoBuilder().NomeValido()
                                    .SKUValido()
                                    .FabricacaoValida()
                                    .ValidadeValida()
                                    .Build()
            };

            yield return new object[]
            {
                new CancellationToken(false),
                new ProdutoBuilder().NomeValido()
                                    .SKUValido()
                                    .FabricacaoInvalida()
                                    .ValidadeValida()
                                    .CategoriaValida()
                                    .Build()
            };

            yield return new object[]
            {
                new CancellationToken(false),
                new ProdutoBuilder().NomeValido()
                                    .SKUValido()
                                    .FabricacaoValida()
                                    .ValidadeInvalida()
                                    .CategoriaValida()
                                    .Build()
            };
        }
    }
}
