namespace Avalonia.Data.Core
{
    public interface IExpressionNode<TIn>
    {
        void OnNext(TIn value);
    }
}
