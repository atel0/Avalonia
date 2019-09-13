namespace Avalonia.Data.Core
{
    interface ITransformNode<TIn, TOut>
    {
        TOut Transform(TIn value);
    }
}
