// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Avalonia.Data.Core
{
    public abstract class ExpressionNode<TIn, TOut> : IExpressionNode<TIn>
    {
        public IExpressionNode<TOut> Next { get; set; }

        void IExpressionNode<TIn>.OnNext(TIn value)
        {
            throw new NotImplementedException();
        }
    }
}
