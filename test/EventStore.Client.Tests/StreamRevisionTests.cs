using System;
using System.Collections.Generic;
using Xunit;

namespace EventStore.Client {
	public class StreamRevisionTests {
		[Fact]
		public void Equality() {
			var sut = new StreamRevision(1);
			Assert.Equal(new StreamRevision(1), sut);
		}

		[Fact]
		public void Inequality() {
			var sut = new StreamRevision(1);
			Assert.NotEqual(new StreamRevision(2), sut);
		}

		[Fact]
		public void EqualityOperator() {
			var sut = new StreamRevision(1);
			Assert.True(new StreamRevision(1) == sut);
		}

		[Fact]
		public void InequalityOperator() {
			var sut = new StreamRevision(1);
			Assert.True(new StreamRevision(2) != sut);
		}

		[Fact]
		public void AdditionOperator() {
			var sut = new StreamRevision(0);
			Assert.Equal(new StreamRevision(1), sut + 1);
			Assert.Equal(new StreamRevision(1), 1 + sut);
		}

		[Fact]
		public void NextReturnsExpectedResult() {
			var sut = new StreamRevision(0);
			Assert.Equal(sut + 1, sut.Next());
		}

		public static IEnumerable<object[]> AdditionOutOfBoundsCases() {
			yield return new object[] {new StreamRevision(long.MaxValue), long.MaxValue + 2UL};
		}

		[Theory, MemberData(nameof(AdditionOutOfBoundsCases))]
		public void AdditionOutOfBoundsThrows(StreamRevision streamRevision, ulong operand) {
			Assert.Throws<OverflowException>(() => streamRevision + operand);
			Assert.Throws<OverflowException>(() => operand + streamRevision);
		}

		[Fact]
		public void SubtractionOperator() {
			var sut = new StreamRevision(1);
			Assert.Equal(new StreamRevision(0), sut - 1);
			Assert.Equal(new StreamRevision(0), 1 - sut);
		}

		public static IEnumerable<object[]> SubtractionOutOfBoundsCases() {
			yield return new object[] {new StreamRevision(1), 2};
			yield return new object[] {new StreamRevision(0), 1};
		}

		[Theory, MemberData(nameof(SubtractionOutOfBoundsCases))]
		public void SubtractionOutOfBoundsThrows(StreamRevision streamRevision, ulong operand) {
			Assert.Throws<OverflowException>(() => streamRevision - operand);
			Assert.Throws<OverflowException>(() => (ulong)streamRevision - new StreamRevision(operand));
		}

		public static IEnumerable<object[]> ArgumentOutOfRangeTestCases() {
			yield return new object[] {long.MaxValue + 1UL};
			yield return new object[] {ulong.MaxValue - 1UL};
		}

		[Theory, MemberData(nameof(ArgumentOutOfRangeTestCases))]
		public void ArgumentOutOfRange(ulong value) {
			var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new StreamRevision(value));
			Assert.Equal(nameof(value), ex.ParamName);
		}

		[Fact]
		public void FromStreamPositionEndThrows() {
			Assert.Throws<ArgumentOutOfRangeException>(() => StreamRevision.FromStreamPosition(StreamPosition.End));
		}

		[Fact]
		public void FromStreamPositionReturnsExpectedResult() {
			var result = StreamRevision.FromStreamPosition(StreamPosition.Start);

			Assert.Equal(new StreamRevision(0), result);
		}

		public static IEnumerable<object[]> ComparableTestCases() {
			var start = new StreamRevision(0);
			yield return new object[] {start, start, 0};
			yield return new object[] {start, StreamRevision.None, -1};
			yield return new object[] {StreamRevision.None, start, 1};
		}

		[Theory, MemberData(nameof(ComparableTestCases))]
		public void Comparability(StreamRevision left, StreamRevision right, int expected)
			=> Assert.Equal(expected, left.CompareTo(right));

		[Fact]
		public void ExplicitConversionFromStreamRevisionReturnsExpectedResult() {
			const ulong value = 0UL;
			var actual = (ulong)new StreamRevision(value);
			Assert.Equal(value, actual);
		}

		[Fact]
		public void ImplicitConversionFromStreamRevisionReturnsExpectedResult() {
			const ulong value = 0UL;
			ulong actual = new StreamRevision(value);
			Assert.Equal(value, actual);
		}

		[Fact]
		public void ExplicitConversionToStreamRevisionReturnsExpectedResult() {
			const ulong value = 0UL;
			var expected = new StreamRevision(value);
			var actual = (StreamRevision)value;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ImplicitConversionToStreamRevisionReturnsExpectedResult() {
			const ulong value = 0UL;
			var expected = new StreamRevision(value);
			StreamRevision actual = value;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ToStringExpectedResult() {
			var expected = 0UL.ToString();

			Assert.Equal(expected, new StreamRevision(0UL).ToString());
		}

		[Fact]
		public void ToUInt64ExpectedResult() {
			var expected = 0UL;

			Assert.Equal(expected, new StreamRevision(expected).ToUInt64());
		}
	}
}
