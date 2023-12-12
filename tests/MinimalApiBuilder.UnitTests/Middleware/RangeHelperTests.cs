using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Primitives;
using MinimalApiBuilder.Middleware;
using NUnit.Framework;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal class RangeHelperTests
{
    [Test]
    public void HasRangeHeaderField_False_Without_Range_Header()
    {
        DefaultHttpContext context = new();

        bool result = RangeHelper.HasRangeHeaderField(context);

        Assert.That(result, Is.False);
    }

    [Test]
    public void HasRangeHeaderField_True_With_Range_Header()
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Range = "bytes=0-1";

        bool result = RangeHelper.HasRangeHeaderField(context);

        Assert.That(result, Is.True);
    }

    [Test]
    public void TryParseRange_Without_Range_Header()
    {
        DefaultHttpContext context = new();
        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();

        bool result = RangeHelper.TryParseRange(context, requestHeaders, 0, out (long, long)? range);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(range, Is.Null);
        });
    }

    [Test]
    public void TryParseRange_Disallows_Multiple_Ranges_From_Multiple_Field_Lines()
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Range = new StringValues(["bytes=0-1", "2-3"]);
        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();

        bool result = RangeHelper.TryParseRange(context, requestHeaders, 0, out (long, long)? range);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(range, Is.Null);
        });
    }

    [Test]
    public void TryParseRange_Disallows_Multiple_Ranges_From_Single_Field_Line()
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Range = new StringValues(["bytes=0-1,2-3"]);
        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();

        bool result = RangeHelper.TryParseRange(context, requestHeaders, 0, out (long, long)? range);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(range, Is.Null);
        });
    }

    [Test]
    public void TryParseRange_Ignores_Invalid_Header()
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Range = "0-1";
        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();

        bool result = RangeHelper.TryParseRange(context, requestHeaders, 10, out (long, long)? range);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(range, Is.Null);
        });
    }

    // https://www.rfc-editor.org/rfc/rfc9110.html#section-14.1.2-10
    [TestCase("bytes=11-30")] // int-range, first-pos greater than length
    [TestCase("bytes=11-")] // int-range, first-pos greater than length, without last-pos
    [TestCase("bytes=10-20")] // int-range, first-pos equals length
    [TestCase("bytes=10-")] // int-range, first-pos equals length, without last-pos
    [TestCase("bytes=-0")] // suffix-range, zero suffix-length
    public void TryParseRange_Not_Satisfiable(string rangeHeader)
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Range = rangeHeader;
        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();

        bool result = RangeHelper.TryParseRange(context, requestHeaders, 10, out (long, long)? range);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(range, Is.Null);
        });
    }

    [TestCase("bytes=0-8", 0, 8)]
    [TestCase("bytes=0-9", 0, 9)]
    [TestCase("bytes=0-10", 0, 9)]
    [TestCase("bytes=2-30", 2, 9)]
    [TestCase("bytes=-3", 7, 9)]
    [TestCase("bytes=-9", 1, 9)]
    [TestCase("bytes=-10", 0, 9)]
    [TestCase("bytes=-11", 0, 9)]
    [TestCase("bytes=-30", 0, 9)]
    public void TryParseRange_Normalizes_Range(string rangeHeader, int expectedStart, int expectedEnd)
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Range = rangeHeader;
        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();

        bool result = RangeHelper.TryParseRange(context, requestHeaders, 10, out (long, long)? range);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(range, Is.EqualTo((expectedStart, expectedEnd)));
        });
    }
}
