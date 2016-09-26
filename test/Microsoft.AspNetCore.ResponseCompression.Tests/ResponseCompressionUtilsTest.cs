﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Microsoft.AspNetCore.ResponseCompression.Tests
{
    public class ResponseCompressionUtilsTest
    {
        private const string TextPlain = "text/plain";

        [Fact]
        public void CreateShouldCompressResponseDelegate_NullMimeTypes_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ResponseCompressionUtils.CreateShouldCompressResponseDelegate(null);
            });
        }

        [Fact]
        public void CreateShouldCompressResponseDelegate_Empty_DontCompress()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.ContentType = TextPlain;

            var func = ResponseCompressionUtils.CreateShouldCompressResponseDelegate(Enumerable.Empty<string>());

            var result = func(httpContext);

            Assert.False(result);
        }

        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/PLAIN")]
        [InlineData("text/plain; charset=ISO-8859-4")]
        [InlineData("text/plain ; charset=ISO-8859-4")]
        public void CreateShouldCompressResponseDelegate_WithCharset_Compress(string contentType)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.ContentType = contentType;

            var func = ResponseCompressionUtils.CreateShouldCompressResponseDelegate(new string[] { TextPlain });

            var result = func(httpContext);

            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("text/plain2")]
        public void CreateShouldCompressResponseDelegate_OtherContentTypes_NoMatch(string contentType)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.ContentType = contentType;

            var func = ResponseCompressionUtils.CreateShouldCompressResponseDelegate(new string[] { TextPlain });

            var result = func(httpContext);

            Assert.False(result);
        }
    }
}
