// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Data;
using Avalonia.Utilities;
using Moq;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace Avalonia.Base.UnitTests
{
    public class PriorityValueTests
    {
        private static readonly AvaloniaProperty TestProperty = 
            new StyledProperty<string>(
                "Test", 
                typeof(PriorityValueTests), 
                new StyledPropertyMetadata<string>());

        [Fact]
        public void Initial_Value_Should_Be_UnsetValue()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));

            Assert.Same(AvaloniaProperty.UnsetValue, target.Value);
        }

        [Fact]
        public void First_Binding_Sets_Value()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));

            target.Add(Single("foo"), BindingPriority.LocalValue);

            Assert.Equal("foo", target.Value);
        }

        [Fact]
        public void Changing_Binding_Should_Set_Value()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var subject = new BehaviorSubject<string>("foo");

            target.Add(subject, BindingPriority.LocalValue);
            Assert.Equal("foo", target.Value);
            subject.OnNext("bar");
            Assert.Equal("bar", target.Value);
        }

        [Fact]
        public void Setting_Direct_Value_Should_Override_Binding()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));

            target.Add(Single("foo"), BindingPriority.LocalValue);
            target.SetValue("bar", BindingPriority.LocalValue);

            Assert.Equal("bar", target.Value);
        }

        [Fact]
        public void Binding_Firing_Should_Override_Direct_Value()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var source = new BehaviorSubject<object>("initial");

            target.Add(source, BindingPriority.LocalValue);
            Assert.Equal("initial", target.Value);
            target.SetValue("first", BindingPriority.LocalValue);
            Assert.Equal("first", target.Value);
            source.OnNext("second");
            Assert.Equal("second", target.Value);
        }

        [Fact]
        public void Earlier_Binding_Firing_Should_Not_Override_Later()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var nonActive = new BehaviorSubject<object>("na");
            var source = new BehaviorSubject<object>("initial");

            target.Add(nonActive, BindingPriority.StyleTrigger);
            target.Add(source, BindingPriority.StyleTrigger);
            Assert.Equal("initial", target.Value);
            target.SetValue("first", BindingPriority.StyleTrigger);
            Assert.Equal("first", target.Value);
            nonActive.OnNext("second");
            Assert.Equal("first", target.Value);
        }

        [Fact]
        public void Binding_Completing_Should_Revert_To_Direct_Value()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var source = new BehaviorSubject<object>("initial");

            target.Add(source, BindingPriority.LocalValue);
            Assert.Equal("initial", target.Value);
            target.SetValue("first", BindingPriority.LocalValue);
            Assert.Equal("first", target.Value);
            source.OnNext("second");
            Assert.Equal("second", target.Value);
            source.OnCompleted();
            Assert.Equal("first", target.Value);
        }

        [Fact]
        public void Binding_With_Lower_Priority_Has_Precedence()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));

            target.Add(Single("foo"), BindingPriority.StyleTrigger);
            target.Add(Single("bar"), BindingPriority.LocalValue);
            target.Add(Single("baz"), BindingPriority.StyleTrigger);

            Assert.Equal("bar", target.Value);
        }

        [Fact]
        public void Later_Binding_With_Same_Priority_Should_Take_Precedence()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));

            target.Add(Single("foo"), BindingPriority.StyleTrigger);
            target.Add(Single("bar"), BindingPriority.LocalValue);
            target.Add(Single("baz"), BindingPriority.LocalValue);
            target.Add(Single("qux"), BindingPriority.StyleTrigger);

            Assert.Equal("baz", target.Value);
        }

        [Fact]
        public void Changing_Binding_With_Lower_Priority_Should_Set_Not_Value()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var subject = new BehaviorSubject<string>("bar");

            target.Add(Single("foo"), BindingPriority.LocalValue);
            target.Add(subject, BindingPriority.StyleTrigger);
            Assert.Equal("foo", target.Value);
            subject.OnNext("baz");
            Assert.Equal("foo", target.Value);
        }

        [Fact]
        public void UnsetValue_Should_Fall_Back_To_Next_Binding()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var subject = new BehaviorSubject<object>("bar");

            target.Add(subject, BindingPriority.LocalValue);
            target.Add(Single("foo"), BindingPriority.StyleTrigger);

            Assert.Equal("bar", target.Value);

            subject.OnNext(AvaloniaProperty.UnsetValue);

            Assert.Equal("foo", target.Value);
        }

        [Fact]
        public void Adding_Value_Should_Call_OnNext()
        {
            var owner = GetMockOwner();
            var target = new PriorityValue(owner.Object, TestProperty, typeof(string));

            target.Add(Single("foo"), BindingPriority.LocalValue);

            owner.Verify(x => x.Changed(target.Property, target.ValuePriority, AvaloniaProperty.UnsetValue, "foo"));
        }

        [Fact]
        public void Changing_Value_Should_Call_OnNext()
        {
            var owner = GetMockOwner();
            var target = new PriorityValue(owner.Object, TestProperty, typeof(string));
            var subject = new BehaviorSubject<object>("foo");

            target.Add(subject, BindingPriority.LocalValue);
            subject.OnNext("bar");

            owner.Verify(x => x.Changed(target.Property, target.ValuePriority, "foo", "bar"));
        }

        [Fact]
        public void Disposing_A_Binding_Should_Revert_To_Next_Value()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));

            target.Add(Single("foo"), BindingPriority.LocalValue);
            var disposable = target.Add(Single("bar"), BindingPriority.LocalValue);

            Assert.Equal("bar", target.Value);
            disposable.Dispose();
            Assert.Equal("foo", target.Value);
        }

        [Fact]
        public void Disposing_A_Binding_Should_Remove_BindingEntry()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));

            target.Add(Single("foo"), BindingPriority.LocalValue);
            var disposable = target.Add(Single("bar"), BindingPriority.LocalValue);

            Assert.Equal(2, target.GetBindings().Count());
            disposable.Dispose();
            Assert.Single(target.GetBindings());
        }

        [Fact]
        public void Completing_A_Binding_Should_Revert_To_Previous_Binding()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var source = new BehaviorSubject<object>("bar");

            target.Add(Single("foo"), BindingPriority.LocalValue);
            target.Add(source, BindingPriority.LocalValue);

            Assert.Equal("bar", target.Value);
            source.OnCompleted();
            Assert.Equal("foo", target.Value);
        }

        [Fact]
        public void Completing_A_Binding_Should_Revert_To_Lower_Priority()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var source = new BehaviorSubject<object>("bar");

            target.Add(Single("foo"), BindingPriority.StyleTrigger);
            target.Add(source, BindingPriority.LocalValue);

            Assert.Equal("bar", target.Value);
            source.OnCompleted();
            Assert.Equal("foo", target.Value);
        }

        [Fact]
        public void Completing_A_Binding_Should_Remove_BindingEntry()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(string));
            var subject = new BehaviorSubject<object>("bar");

            target.Add(Single("foo"), BindingPriority.LocalValue);
            target.Add(subject, BindingPriority.LocalValue);

            Assert.Equal(2, target.GetBindings().Count());
            subject.OnCompleted();
            Assert.Single(target.GetBindings());
        }

        [Fact]
        public void Direct_Value_Should_Be_Coerced()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(int), x => Math.Min((int)x, 10));

            target.SetValue(5, BindingPriority.LocalValue);
            Assert.Equal(5, target.Value);
            target.SetValue(15, BindingPriority.LocalValue);
            Assert.Equal(10, target.Value);
        }

        [Fact]
        public void Bound_Value_Should_Be_Coerced()
        {
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(int), x => Math.Min((int)x, 10));
            var source = new Subject<object>();

            target.Add(source, BindingPriority.LocalValue);
            source.OnNext(5);
            Assert.Equal(5, target.Value);
            source.OnNext(15);
            Assert.Equal(10, target.Value);
        }

        [Fact]
        public void Revalidate_Should_ReCoerce_Value()
        {
            var max = 10;
            var target = new PriorityValue(GetMockOwner().Object, TestProperty, typeof(int), x => Math.Min((int)x, max));
            var source = new Subject<object>();

            target.Add(source, BindingPriority.LocalValue);
            source.OnNext(5);
            Assert.Equal(5, target.Value);
            source.OnNext(15);
            Assert.Equal(10, target.Value);
            max = 12;
            target.Revalidate();
            Assert.Equal(12, target.Value);
        }

        /// <summary>
        /// Returns an observable that returns a single value but does not complete.
        /// </summary>
        /// <typeparam name="T">The type of the observable.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The observable.</returns>
        private IObservable<T> Single<T>(T value)
        {
            return Observable.Never<T>().StartWith(value);
        }

        private static Mock<IPriorityValueOwner> GetMockOwner()
        {
            var owner = new Mock<IPriorityValueOwner>();
            owner.Setup(o => o.GetNonDirectDeferredSetter(It.IsAny<AvaloniaProperty>())).Returns(new DeferredSetter<object>());
            return owner;
        }
    }
}
