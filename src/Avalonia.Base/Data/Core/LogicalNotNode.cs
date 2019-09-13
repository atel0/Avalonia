// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Avalonia.Data.Core
{
    public class LogicalNotNode<TIn, TOut> : ExpressionNode<TIn, TOut>, ITransformNode<TIn, TOut>
    {
        public TOut Transform(TIn value)
        {
            throw new NotImplementedException();
        }
    }
}
