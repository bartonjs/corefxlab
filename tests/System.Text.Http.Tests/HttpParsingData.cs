﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.Http.Parser.Tests
{
    public class HttpParsingData
    {
        public static IEnumerable<string[]> RequestLineValidData
        {
            get {
                var methods = new[]
                {
                    "GET",
                    "CUSTOM",
                };
                var paths = new[]
                {
                    Tuple.Create("/", "/"),
                    Tuple.Create("/abc", "/abc"),
                    Tuple.Create("/abc/de/f", "/abc/de/f"),
                    Tuple.Create("/%20", "/ "),
                    Tuple.Create("/a%20", "/a "),
                    Tuple.Create("/%20a", "/ a"),
                    Tuple.Create("/a/b%20c", "/a/b c"),
                    Tuple.Create("/%C3%A5", "/\u00E5"),
                    Tuple.Create("/a%C3%A5a", "/a\u00E5a"),
                    Tuple.Create("/%C3%A5/bc", "/\u00E5/bc"),
                    Tuple.Create("/%25", "/%"),
                    Tuple.Create("/%2F", "/%2F"),
                    Tuple.Create("http://host/abs/path", "/abs/path"),
                    Tuple.Create("http://host/abs/path/", "/abs/path/"),
                    Tuple.Create("http://host/a%20b%20c/", "/a b c/"),
                    Tuple.Create("https://host/abs/path", "/abs/path"),
                    Tuple.Create("https://host/abs/path/", "/abs/path/"),
                    Tuple.Create("https://host:22/abs/path", "/abs/path"),
                    Tuple.Create("https://user@host:9080/abs/path", "/abs/path"),
                    Tuple.Create("http://host/", "/"),
                    Tuple.Create("http://host", "/"),
                    Tuple.Create("https://host/", "/"),
                    Tuple.Create("https://host", "/"),
                    Tuple.Create("http://user@host/", "/"),
                    Tuple.Create("http://127.0.0.1/", "/"),
                    Tuple.Create("http://user@127.0.0.1/", "/"),
                    Tuple.Create("http://user@127.0.0.1:8080/", "/"),
                    Tuple.Create("http://127.0.0.1:8080/", "/"),
                    Tuple.Create("http://[::1]", "/"),
                    Tuple.Create("http://[::1]/path", "/path"),
                    Tuple.Create("http://[::1]:8080/", "/"),
                    Tuple.Create("http://user@[::1]:8080/", "/"),
                };
                var queryStrings = new[]
                {
                    "",
                    "?",
                    "?arg1=val1",
                    "?arg1=a%20b",
                    "?%A",
                    "?%20=space",
                    "?%C3%A5=val",
                    "?path=/home",
                    "?path=/%C3%A5/",
                    "?question=what?",
                    "?%00",
                    "?arg=%00"
                };
                var httpVersions = new[]
                {
                    "HTTP/1.0",
                    "HTTP/1.1"
                };

                return from method in methods
                       from path in paths
                       from queryString in queryStrings
                       from httpVersion in httpVersions
                       select new[]
                       {
                           $"{method} {path.Item1}{queryString} {httpVersion}\r\n",
                           method,
                           $"{path.Item1}{queryString}",
                           $"{path.Item1}",
                           $"{path.Item2}",
                           queryString,
                           httpVersion
                       };
            }
        }

        public static IEnumerable<string> RequestLineIncompleteData => new[]
        {
            "G",
            "GE",
            "GET",
            "GET ",
            "GET /",
            "GET / ",
            "GET / H",
            "GET / HT",
            "GET / HTT",
            "GET / HTTP",
            "GET / HTTP/",
            "GET / HTTP/1",
            "GET / HTTP/1.",
            "GET / HTTP/1.1",
            "GET / HTTP/1.1\r",
        };

        public static IEnumerable<string> RequestLineInvalidData
        {
            get {
                return new[]
                {
                    "G\r\n",
                    "GE\r\n",
                    "GET\r\n",
                    "GET \r\n",
                    "GET /\r\n",
                    "GET / \r\n",
                    "GET/HTTP/1.1\r\n",
                    "GET /HTTP/1.1\r\n",
                    " \r\n",
                    "  \r\n",
                    "/ HTTP/1.1\r\n",
                    " / HTTP/1.1\r\n",
                    "/ \r\n",
                    "GET  \r\n",
                    "GET  HTTP/1.0\r\n",
                    "GET  HTTP/1.1\r\n",
                    "GET / \n",
                    "GET / HTTP/1.0\n",
                    "GET / HTTP/1.1\n",
                    "GET / HTTP/1.0\rA\n",
                    "GET / HTTP/1.1\ra\n",
                    "GET? / HTTP/1.1\r\n",
                    "GET ? HTTP/1.1\r\n",
                    "GET /a?b=cHTTP/1.1\r\n",
                    "GET /a%20bHTTP/1.1\r\n",
                    "GET /a%20b?c=dHTTP/1.1\r\n",
                    "GET %2F HTTP/1.1\r\n",
                    "GET %00 HTTP/1.1\r\n",
                    "CUSTOM \r\n",
                    "CUSTOM /\r\n",
                    "CUSTOM / \r\n",
                    "CUSTOM /HTTP/1.1\r\n",
                    "CUSTOM  \r\n",
                    "CUSTOM  HTTP/1.0\r\n",
                    "CUSTOM  HTTP/1.1\r\n",
                    "CUSTOM / \n",
                    "CUSTOM / HTTP/1.0\n",
                    "CUSTOM / HTTP/1.1\n",
                    "CUSTOM / HTTP/1.0\rA\n",
                    "CUSTOM / HTTP/1.1\ra\n",
                    "CUSTOM ? HTTP/1.1\r\n",
                    "CUSTOM /a?b=cHTTP/1.1\r\n",
                    "CUSTOM /a%20bHTTP/1.1\r\n",
                    "CUSTOM /a%20b?c=dHTTP/1.1\r\n",
                    "CUSTOM %2F HTTP/1.1\r\n",
                    "CUSTOM %00 HTTP/1.1\r\n",
                }.Concat(MethodWithNonTokenCharData.Select(method => $"{method} / HTTP/1.0\r\n"));
            }
        }

        // Bad HTTP Methods (invalid according to RFC)
        public static IEnumerable<string> MethodWithNonTokenCharData
        {
            get {
                return new[]
                {
                    "(",
                    ")",
                    "<",
                    ">",
                    "@",
                    ",",
                    ";",
                    ":",
                    "\\",
                    "\"",
                    "/",
                    "[",
                    "]",
                    "?",
                    "=",
                    "{",
                    "}",
                    "get@",
                    "post=",
                }.Concat(MethodWithNullCharData);
            }
        }

        public static IEnumerable<string> MethodWithNullCharData => new[]
        {
            // Bad HTTP Methods (invalid according to RFC)
            "\0",
            "\0GET",
            "G\0T",
            "GET\0",
        };

        public static IEnumerable<string> TargetWithEncodedNullCharData => new[]
        {
            "/%00",
            "/%00%00",
            "/%E8%00%84",
            "/%E8%85%00",
            "/%F3%00%82%86",
            "/%F3%85%00%82",
            "/%F3%85%82%00",
        };

        public static TheoryData<string, string> TargetInvalidData
        {
            get {
                var data = new TheoryData<string, string>();

                // Invalid absolute-form
                data.Add("GET", "http://");
                data.Add("GET", "http:/");
                data.Add("GET", "https:/");
                data.Add("GET", "http:///");
                data.Add("GET", "https://");
                data.Add("GET", "http:////");
                data.Add("GET", "http://:80");
                data.Add("GET", "http://:80/abc");
                data.Add("GET", "http://user@");
                data.Add("GET", "http://user@/abc");
                data.Add("GET", "http://abc%20xyz/abc");
                data.Add("GET", "http://%20/abc?query=%0A");
                // Valid absolute-form but with unsupported schemes
                data.Add("GET", "otherscheme://host/");
                data.Add("GET", "ws://host/");
                data.Add("GET", "wss://host/");
                // Must only have one asterisk
                data.Add("OPTIONS", "**");
                // Relative form
                data.Add("GET", "../../");
                data.Add("GET", "..\\.");

                return data;
            }
        }

        public static TheoryData<string, Http.Method> MethodNotAllowedRequestLine
        {
            get {
                var methods = new[]
                {
                    "GET",
                    "PUT",
                    "DELETE",
                    "POST",
                    "HEAD",
                    "TRACE",
                    "PATCH",
                    "CONNECT",
                    "OPTIONS",
                    "CUSTOM",
                };

                var data = new TheoryData<string, Http.Method>();

                foreach (var method in methods.Except(new[] { "OPTIONS" })) {
                    data.Add($"{method} * HTTP/1.1\r\n", Http.Method.Options);
                }

                foreach (var method in methods.Except(new[] { "CONNECT" })) {
                    data.Add($"{method} www.example.com:80 HTTP/1.1\r\n", Http.Method.Connect);
                }

                return data;
            }
        }

        public static IEnumerable<string> TargetWithNullCharData
        {
            get {
                return new[]
                {
                    "\0",
                    "/\0",
                    "/\0\0",
                    "/%C8\0",
                }.Concat(QueryStringWithNullCharData);
            }
        }

        public static IEnumerable<string> QueryStringWithNullCharData => new[]
        {
            "/?\0=a",
            "/?a=\0",
        };

        public static TheoryData<string> UnrecognizedHttpVersionData => new TheoryData<string>
        {
            " ",
            "/",
            "H",
            "HT",
            "HTT",
            "HTTP",
            "HTTP/",
            "HTTP/1",
            "HTTP/1.",
            "http/1.0",
            "http/1.1",
            "HTTP/1.1 ",
            "HTTP/1.0a",
            "HTTP/1.0ab",
            "HTTP/1.1a",
            "HTTP/1.1ab",
            "HTTP/1.2",
            "HTTP/3.0",
            "hello",
            "8charact",
        };

        public static IEnumerable<object[]> RequestHeaderInvalidData => new[]
        {
            // Missing CR
            new[] { "Header: value\n\r\n", @"Invalid request header: 'Header: value\x0A'" },
            new[] { "Header-1: value1\nHeader-2: value2\r\n\r\n", @"Invalid request header: 'Header-1: value1\x0A'" },
            new[] { "Header-1: value1\r\nHeader-2: value2\n\r\n", @"Invalid request header: 'Header-2: value2\x0A'" },

            // Line folding
            new[] { "Header: line1\r\n line2\r\n\r\n", @"Invalid request header: ' line2\x0D\x0A'" },
            new[] { "Header: line1\r\n\tline2\r\n\r\n", @"Invalid request header: '\x09line2\x0D\x0A'" },
            new[] { "Header: line1\r\n  line2\r\n\r\n", @"Invalid request header: '  line2\x0D\x0A'" },
            new[] { "Header: line1\r\n \tline2\r\n\r\n", @"Invalid request header: ' \x09line2\x0D\x0A'" },
            new[] { "Header: line1\r\n\t line2\r\n\r\n", @"Invalid request header: '\x09 line2\x0D\x0A'" },
            new[] { "Header: line1\r\n\t\tline2\r\n\r\n", @"Invalid request header: '\x09\x09line2\x0D\x0A'" },
            new[] { "Header: line1\r\n \t\t line2\r\n\r\n", @"Invalid request header: ' \x09\x09 line2\x0D\x0A'" },
            new[] { "Header: line1\r\n \t \t line2\r\n\r\n", @"Invalid request header: ' \x09 \x09 line2\x0D\x0A'" },
            new[] { "Header-1: multi\r\n line\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: ' line\x0D\x0A'" },
            new[] { "Header-1: value1\r\nHeader-2: multi\r\n line\r\n\r\n", @"Invalid request header: ' line\x0D\x0A'" },
            new[] { "Header-1: value1\r\n Header-2: value2\r\n\r\n", @"Invalid request header: ' Header-2: value2\x0D\x0A'" },
            new[] { "Header-1: value1\r\n\tHeader-2: value2\r\n\r\n", @"Invalid request header: '\x09Header-2: value2\x0D\x0A'" },

            // CR in value
            new[] { "Header-1: value1\r\r\n", @"Invalid request header: 'Header-1: value1\x0D\x0D\x0A'" },
            new[] { "Header-1: val\rue1\r\n", @"Invalid request header: 'Header-1: val\x0Due1\x0D\x0A'" },
            new[] { "Header-1: value1\rHeader-2: value2\r\n\r\n", @"Invalid request header: 'Header-1: value1\x0DHeader-2: value2\x0D\x0A'" },
            new[] { "Header-1: value1\r\nHeader-2: value2\r\r\n", @"Invalid request header: 'Header-2: value2\x0D\x0D\x0A'" },
            new[] { "Header-1: value1\r\nHeader-2: v\ralue2\r\n", @"Invalid request header: 'Header-2: v\x0Dalue2\x0D\x0A'" },
            new[] { "Header-1: Value__\rVector16________Vector32\r\n", @"Invalid request header: 'Header-1: Value__\x0DVector16________Vector32\x0D\x0A'" },
            new[] { "Header-1: Value___Vector16\r________Vector32\r\n", @"Invalid request header: 'Header-1: Value___Vector16\x0D________Vector32\x0D\x0A'" },
            new[] { "Header-1: Value___Vector16_______\rVector32\r\n", @"Invalid request header: 'Header-1: Value___Vector16_______\x0DVector32\x0D\x0A'" },
            new[] { "Header-1: Value___Vector16________Vector32\r\r\n", @"Invalid request header: 'Header-1: Value___Vector16________Vector32\x0D\x0D\x0A'" },
            new[] { "Header-1: Value___Vector16________Vector32_\r\r\n", @"Invalid request header: 'Header-1: Value___Vector16________Vector32_\x0D\x0D\x0A'" },
            new[] { "Header-1: Value___Vector16________Vector32Value___Vector16_______\rVector32\r\n", @"Invalid request header: 'Header-1: Value___Vector16________Vector32Value___Vector16_______\x0DVector32\x0D\x0A'" },
            new[] { "Header-1: Value___Vector16________Vector32Value___Vector16________Vector32\r\r\n", @"Invalid request header: 'Header-1: Value___Vector16________Vector32Value___Vector16________Vector32\x0D\x0D\x0A'" },
            new[] { "Header-1: Value___Vector16________Vector32Value___Vector16________Vector32_\r\r\n", @"Invalid request header: 'Header-1: Value___Vector16________Vector32Value___Vector16________Vector32_\x0D\x0D\x0A'" },

            // Missing colon
            new[] { "Header-1 value1\r\n\r\n", @"Invalid request header: 'Header-1 value1\x0D\x0A'" },
            new[] { "Header-1 value1\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: 'Header-1 value1\x0D\x0A'" },
            new[] { "Header-1: value1\r\nHeader-2 value2\r\n\r\n", @"Invalid request header: 'Header-2 value2\x0D\x0A'" },
            new[] { "\n", @"Invalid request header: '\x0A'" },

            // Starting with whitespace
            new[] { " Header: value\r\n\r\n", @"Invalid request header: ' Header: value\x0D\x0A'" },
            new[] { "\tHeader: value\r\n\r\n", @"Invalid request header: '\x09Header: value\x0D\x0A'" },
            new[] { " Header-1: value1\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: ' Header-1: value1\x0D\x0A'" },
            new[] { "\tHeader-1: value1\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: '\x09Header-1: value1\x0D\x0A'" },

            // Whitespace in header name
            new[] { "Header : value\r\n\r\n", @"Invalid request header: 'Header : value\x0D\x0A'" },
            new[] { "Header\t: value\r\n\r\n", @"Invalid request header: 'Header\x09: value\x0D\x0A'" },
            new[] { "Header\r: value\r\n\r\n", @"Invalid request header: 'Header\x0D: value\x0D\x0A'" },
            new[] { "Header_\rVector16: value\r\n\r\n", @"Invalid request header: 'Header_\x0DVector16: value\x0D\x0A'" },
            new[] { "Header__Vector16\r: value\r\n\r\n", @"Invalid request header: 'Header__Vector16\x0D: value\x0D\x0A'" },
            new[] { "Header__Vector16_\r: value\r\n\r\n", @"Invalid request header: 'Header__Vector16_\x0D: value\x0D\x0A'" },
            new[] { "Header_\rVector16________Vector32: value\r\n\r\n", @"Invalid request header: 'Header_\x0DVector16________Vector32: value\x0D\x0A'" },
            new[] { "Header__Vector16________Vector32\r: value\r\n\r\n", @"Invalid request header: 'Header__Vector16________Vector32\x0D: value\x0D\x0A'" },
            new[] { "Header__Vector16________Vector32_\r: value\r\n\r\n", @"Invalid request header: 'Header__Vector16________Vector32_\x0D: value\x0D\x0A'" },
            new[] { "Header__Vector16________Vector32Header_\rVector16________Vector32: value\r\n\r\n", @"Invalid request header: 'Header__Vector16________Vector32Header_\x0DVector16________Vector32: value\x0D\x0A'" },
            new[] { "Header__Vector16________Vector32Header__Vector16________Vector32\r: value\r\n\r\n", @"Invalid request header: 'Header__Vector16________Vector32Header__Vector16________Vector32\x0D: value\x0D\x0A'" },
            new[] { "Header__Vector16________Vector32Header__Vector16________Vector32_\r: value\r\n\r\n", @"Invalid request header: 'Header__Vector16________Vector32Header__Vector16________Vector32_\x0D: value\x0D\x0A'" },
            new[] { "Header 1: value1\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: 'Header 1: value1\x0D\x0A'" },
            new[] { "Header 1 : value1\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: 'Header 1 : value1\x0D\x0A'" },
            new[] { "Header 1\t: value1\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: 'Header 1\x09: value1\x0D\x0A'" },
            new[] { "Header 1\r: value1\r\nHeader-2: value2\r\n\r\n", @"Invalid request header: 'Header 1\x0D: value1\x0D\x0A'" },
            new[] { "Header-1: value1\r\nHeader 2: value2\r\n\r\n", @"Invalid request header: 'Header 2: value2\x0D\x0A'" },
            new[] { "Header-1: value1\r\nHeader-2 : value2\r\n\r\n", @"Invalid request header: 'Header-2 : value2\x0D\x0A'" },
            new[] { "Header-1: value1\r\nHeader-2\t: value2\r\n\r\n", @"Invalid request header: 'Header-2\x09: value2\x0D\x0A'" },

            // Headers not ending in CRLF line
            new[] { "Header-1: value1\r\nHeader-2: value2\r\n\r\r", @"Invalid request headers: missing final CRLF in header fields." },
            new[] { "Header-1: value1\r\nHeader-2: value2\r\n\r ", @"Invalid request headers: missing final CRLF in header fields."  },
            new[] { "Header-1: value1\r\nHeader-2: value2\r\n\r \n", @"Invalid request headers: missing final CRLF in header fields." },
        };
    }
}
