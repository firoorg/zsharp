using System;
using Xunit;

namespace Zsharp.Elysium.Tests
{
    public sealed class TokenAmountTests
    {
        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        [InlineData(0L)]
        public void Constructor_WithValidValue_ShouldSuccess(long value)
        {
            new TokenAmount(value);
        }

        [Theory]
        [InlineData(-9223372036854775808L)]
        [InlineData(-1)]
        [InlineData(1L)]
        [InlineData(100000000L)]
        [InlineData(9223372036854775807)]
        public void Value_GetAfterConstruct_ShouldReturnValuePassedToConstructor(long value)
        {
            var amount = new TokenAmount(value);

            Assert.Equal(value, amount.Value);
        }

        [Theory]
        [InlineData("-92233720368.54775808")]
        [InlineData("0.00000001")]
        [InlineData("0")]
        [InlineData("92233720368.54775807")]
        public void FromDivisible_WithValidValue_ShouldSuccess(string s)
        {
            var value = decimal.Parse(s);
            var amount = TokenAmount.FromDivisible(value);

            Assert.Equal((long)(value * 100000000), amount.Value);
        }

        [Theory]
        [InlineData("0.000000009")]
        [InlineData("-92233720368.54775809")]
        [InlineData("92233720368.54775808")]
        public void FromDivisible_WithInvalidValue_ShouldThrow(string s)
        {
            var v = decimal.Parse(s);

            Assert.Throws<ArgumentOutOfRangeException>(() => TokenAmount.FromDivisible(v));
        }

        [Fact]
        public void Negate_WithMinValue_ShouldThrow()
        {
            var v = new TokenAmount(long.MinValue);

            Assert.Throws<OverflowException>(() => TokenAmount.Negate(v));
        }

        [Theory]
        [InlineData(long.MaxValue, -long.MaxValue)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(long.MinValue + 1, long.MaxValue)]
        public void Negate_WithNagatableAmount_ShouldReturnNagatedValue(long value, long expected)
        {
            // Arrange.
            var amount = new TokenAmount(value);

            // Act.
            var negated = TokenAmount.Negate(amount);

            // Assert.
            Assert.Equal(new TokenAmount(expected), negated);
        }

        [Theory]
        [InlineData("-92233720368.54775808", -9223372036854775808L)]
        [InlineData("-0.00000001", -1L)]
        [InlineData("0.00000001", 1L)]
        [InlineData("0.1", 10000000L)]
        [InlineData("0.0", 0L)]
        [InlineData("1.0", 100000000L)]
        [InlineData("1.9", 190000000L)]
        [InlineData("92233720368.54775807", 9223372036854775807L)]
        public void Parse_WithValidDivisible_ShouldSuccess(string divisible, long expected)
        {
            var amount = TokenAmount.Parse(divisible);

            Assert.Equal(expected, amount.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a.0")]
        [InlineData("1a.1")]
        [InlineData("1.a")]
        [InlineData("1.1a")]
        [InlineData("0.000000001")]
        public void Parse_WithInvalidDivisible_ShouldThrow(string s)
        {
            Assert.Throws<FormatException>(() => TokenAmount.Parse(s));
        }

        [Theory]
        [InlineData("-92233720368.54775809")]
        [InlineData("92233720368.54775808")]
        public void Parse_WithDivisibleOutOfRange_ShouldThrow(string s)
        {
            Assert.Throws<OverflowException>(() => TokenAmount.Parse(s));
        }

        [Theory]
        [InlineData("-9223372036854775808", -9223372036854775808L)]
        [InlineData("-1", -1L)]
        [InlineData("0", 0L)]
        [InlineData("9223372036854775807", 9223372036854775807L)]
        public void Parse_WithValidIndivisible_ShouldSuccess(string indivisible, long expected)
        {
            var amount = TokenAmount.Parse(indivisible);

            Assert.Equal(expected, amount.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("aa")]
        [InlineData("1a")]
        public void Parse_WithInvalidIndivisible_ShouldThrow(string s)
        {
            Assert.Throws<FormatException>(() => TokenAmount.Parse(s));
        }

        [Theory]
        [InlineData("-9223372036854775809")]
        [InlineData("9223372036854775808")]
        public void Parse_WithIndivisibleOutOfRange_ShouldThrow(string s)
        {
            Assert.Throws<OverflowException>(() => TokenAmount.Parse(s));
        }

        [Theory]
        [InlineData(-2L, -1L)]
        [InlineData(-1L, 0L)]
        [InlineData(0L, 1L)]
        public void Equals_WithValueLessThan_ShouldReturnFalse(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.False(first.Equals(second));
            Assert.False(first.Equals((object)second));
            Assert.False(first == second);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        [InlineData(1L)]
        public void Equals_WithSameValue_ShouldReturnTrue(long value)
        {
            var first = new TokenAmount(value);
            var second = new TokenAmount(value);

            Assert.True(first.Equals(second));
            Assert.True(first.Equals((object)second));
            Assert.True(first == second);
        }

        [Theory]
        [InlineData(-1L, -2L)]
        [InlineData(0L, -1L)]
        [InlineData(1L, 0L)]
        public void Equals_WithValueGreaterThan_ShouldReturnFalse(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.False(first.Equals(second));
            Assert.False(first.Equals((object)second));
            Assert.False(first == second);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            var first = new TokenAmount(0);

            Assert.False(first.Equals(null));
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            var first = new TokenAmount(0);

            Assert.False(first.Equals(0L));
        }

        [Theory]
        [InlineData(-9223372036854775808L, "-92233720368.54775808")]
        [InlineData(-1, "-0.00000001")]
        [InlineData(1L, "0.00000001")]
        [InlineData(100000000L, "1.00000000")]
        [InlineData(9223372036854775807, "92233720368.54775807")]
        public void ToDivisible_WhenInvoke_ShouldReturnValueDivideBy100000000(long value, string expect)
        {
            var amount = new TokenAmount(value);

            Assert.Equal(decimal.Parse(expect), amount.ToDivisible());
        }

        [Theory]
        [InlineData(-9223372036854775808L, "-92233720368.54775808")]
        [InlineData(-1L, "-0.00000001")]
        [InlineData(0L, "0.00000000")]
        [InlineData(1L, "0.00000001")]
        [InlineData(100000000L, "1.00000000")]
        [InlineData(9223372036854775807L, "92233720368.54775807")]
        public void ToString_WithDivisible_ShouldSuccess(long value, string expect)
        {
            var amount = new TokenAmount(value);

            Assert.Equal(expect, amount.ToString(TokenType.Divisible));
        }

        [Theory]
        [InlineData(-9223372036854775808L)]
        [InlineData(-1L)]
        [InlineData(0L)]
        [InlineData(1L)]
        [InlineData(100000000L)]
        [InlineData(9223372036854775807L)]
        public void ToString_WithIndivisible_ShouldSuccess(long value)
        {
            var amount = new TokenAmount(value);

            Assert.Equal(value.ToString(), amount.ToString(TokenType.Indivisible));
        }

        [Fact]
        public void ToString_WithInvalidType_ShouldThrow()
        {
            var amount = new TokenAmount(0);

            Assert.Throws<ArgumentOutOfRangeException>("type", () => amount.ToString((TokenType)100));
        }

        [Fact]
        public void ToString_WithoutArguments_ShouldReturnFullTypeName()
        {
            var amount = new TokenAmount(1);

            Assert.Equal(typeof(TokenAmount).FullName, amount.ToString());
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(4611686018427387904, 4611686018427387903, 9223372036854775807)]
        [InlineData(4611686018427387903, 4611686018427387904, 9223372036854775807)]
        [InlineData(-4611686018427387904, -4611686018427387903, -9223372036854775807)]
        [InlineData(-4611686018427387903, -4611686018427387904, -9223372036854775807)]
        [InlineData(-4611686018427387904, -4611686018427387904, -9223372036854775808)]
        [InlineData(9223372036854775807, -9223372036854775808, -1)]
        [InlineData(-9223372036854775808, 9223372036854775807, -1)]
        public void Addition_SumIsNotOverflow_ShouldReturnThatValue(long left, long right, long expect)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            var result = first + second;

            Assert.Equal(new TokenAmount(expect), result);
        }

        [Theory]
        [InlineData(9223372036854775807, 9223372036854775807)]
        [InlineData(9223372036854775807, 1)]
        [InlineData(1, 9223372036854775807)]
        [InlineData(-9223372036854775808, -9223372036854775808)]
        [InlineData(-9223372036854775808, -1)]
        [InlineData(-1, -9223372036854775808)]
        public void Addition_SumIsOverflow_ShouldThrow(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.Throws<OverflowException>(() => first + second);
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(0)]
        [InlineData(long.MaxValue)]
        public void Division_DivisorIsZero_ShouldThrow(long value)
        {
            var amount = new TokenAmount(value);

            Assert.Throws<DivideByZeroException>(() => amount / 0);
        }

        [Fact]
        public void Division_ResultIsOverflow_ShouldThrow()
        {
            var amount = new TokenAmount(long.MinValue);

            Assert.Throws<OverflowException>(() => amount / -1);
        }

        [Theory]
        [InlineData(9223372036854775807L, 2, 4611686018427387903L)]
        [InlineData(9223372036854775807L, -2, -4611686018427387903L)]
        [InlineData(0L, 2, 0L)]
        [InlineData(0L, -2, 0L)]
        [InlineData(-9223372036854775808L, 2, -4611686018427387904L)]
        [InlineData(-9223372036854775808L, -2, 4611686018427387904L)]
        public void Division_WithValidDivisor_ResultShouldBeDividedByThatAmount(long value, int divisor, long expect)
        {
            var amount = new TokenAmount(value);
            var result = amount / divisor;

            Assert.Equal(expect, result.Value);
        }

        [Theory]
        [InlineData(-2L, -1L)]
        [InlineData(-1L, 0L)]
        [InlineData(0L, 1L)]
        public void GreaterThan_WithLess_ShouldReturnFalse(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.False(first > second);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        [InlineData(1L)]
        public void GreaterThan_WithSame_ShouldReturnFalse(long value)
        {
            var first = new TokenAmount(value);
            var second = new TokenAmount(value);

            Assert.False(first > second);
        }

        [Theory]
        [InlineData(-1L, -2L)]
        [InlineData(0L, -1L)]
        [InlineData(1L, 0L)]
        public void GreaterThan_WithGreater_ShouldReturneTrue(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.True(first > second);
        }

        [Theory]
        [InlineData(-2L, -1L)]
        [InlineData(-1L, 0L)]
        [InlineData(0L, 1L)]
        public void GreaterThanOrEqual_WithLess_ShouldReturnFalse(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.False(first >= second);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        [InlineData(1L)]
        public void GreaterThanOrEqual_WithSame_ShouldReturnTrue(long value)
        {
            var first = new TokenAmount(value);
            var second = new TokenAmount(value);

            Assert.True(first >= second);
        }

        [Theory]
        [InlineData(-1L, -2L)]
        [InlineData(0L, -1L)]
        [InlineData(1L, 0L)]
        public void GreaterThanOrEqual_WithGreater_ShouldReturnTrue(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.True(first >= second);
        }

        [Theory]
        [InlineData(-2L, -1L)]
        [InlineData(-1L, 0L)]
        [InlineData(0L, 1L)]
        public void Inequality_WithLess_ShouldReturnTrue(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.True(first != second);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        [InlineData(1L)]
        public void Inequality_WithSame_ShouldReturnFalse(long value)
        {
            var first = new TokenAmount(value);
            var second = new TokenAmount(value);

            Assert.False(first != second);
        }

        [Theory]
        [InlineData(-1L, -2L)]
        [InlineData(0L, -1L)]
        [InlineData(1L, 0L)]
        public void Inequality_WithGreater_ShouldReturnTrue(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.True(first != second);
        }

        [Theory]
        [InlineData(-2L, -1L)]
        [InlineData(-1L, 0L)]
        [InlineData(0L, 1L)]
        public void LessThan_WithLess_ShouldReturnTrue(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.True(first < second);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        [InlineData(1L)]
        public void LessThan_WithSame_ShouldReturnFalse(long value)
        {
            var first = new TokenAmount(value);
            var second = new TokenAmount(value);

            Assert.False(first < second);
        }

        [Theory]
        [InlineData(-1L, -2L)]
        [InlineData(0L, -1L)]
        [InlineData(1L, 0L)]
        public void LessThan_WithGreater_ShouldReturnFalse(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.False(first < second);
        }

        [Theory]
        [InlineData(-2L, -1L)]
        [InlineData(-1L, 0L)]
        [InlineData(0L, 1L)]
        public void LessThanOrEqual_WithLess_ShouldReturnTrue(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.True(first <= second);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        [InlineData(1L)]
        public void LessThanOrEqual_WithSame_ShouldReturnTrue(long value)
        {
            var first = new TokenAmount(value);
            var second = new TokenAmount(value);

            Assert.True(first <= second);
        }

        [Theory]
        [InlineData(-1L, -2L)]
        [InlineData(0L, -1L)]
        [InlineData(1L, 0L)]
        public void LessThanOrEqual_WithGreater_ShouldReturnFalse(long left, long right)
        {
            var first = new TokenAmount(left);
            var second = new TokenAmount(right);

            Assert.False(first <= second);
        }

        [Fact]
        public void NegateOperator_WithMinValue_ShouldThrow()
        {
            var v = new TokenAmount(long.MinValue);

            Assert.Throws<OverflowException>(() => -v);
        }

        [Theory]
        [InlineData(long.MaxValue, -long.MaxValue)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(long.MinValue + 1, long.MaxValue)]
        public void NegateOperator_WithNagatableAmount_ShouldReturnNagatedValue(long value, long expected)
        {
            // Arrange.
            var amount = new TokenAmount(value);

            // Act.
            var negated = -amount;

            // Assert.
            Assert.Equal(new TokenAmount(expected), negated);
        }
    }
}
