using AvaliacaoFinalTU.Feature.Produto;

namespace AvaliacaoFinalTU.Core.Integration
{
    public interface IFactoryIntegration
    {
        public bool ExistProduct(Produto produto);
        public void ReservaProduto(Produto produto);
    }
}
