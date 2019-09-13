using System;
using Avalonia.Data.Core;

namespace Avalonia.Markup.Parsers.Nodes
{
    internal class FindAncestorNode : ExpressionNode<object, object>
    {
        private Type _ancestorType;
        private int _ancestorLevel;

        public FindAncestorNode(Type ancestorType, int ancestorLevel)
        {
            _ancestorType = ancestorType;
            _ancestorLevel = ancestorLevel;
        }
    }
}
