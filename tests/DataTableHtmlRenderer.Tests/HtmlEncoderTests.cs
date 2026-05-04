using DataTableHtmlRenderer;
using Xunit;

namespace DataTableHtmlRenderer.Tests
{
    public class HtmlEncoderTests
    {
        [Fact]
        public void Encode_ShouldHandleNull()
        {
            Assert.Equal("", HtmlEncoder.Encode(null));
        }

        [Fact]
        public void Encode_ShouldHandleEmptyString()
        {
            Assert.Equal("", HtmlEncoder.Encode(""));
        }

        [Fact]
        public void Encode_ShouldEncodeAmpersand()
        {
            Assert.Equal("&amp;", HtmlEncoder.Encode("&"));
        }

        [Fact]
        public void Encode_ShouldEncodeLessThan()
        {
            Assert.Equal("&lt;", HtmlEncoder.Encode("<"));
        }

        [Fact]
        public void Encode_ShouldEncodeGreaterThan()
        {
            Assert.Equal("&gt;", HtmlEncoder.Encode(">"));
        }

        [Fact]
        public void Encode_ShouldEncodeDoubleQuote()
        {
            Assert.Equal("&quot;", HtmlEncoder.Encode("\""));
        }

        [Fact]
        public void Encode_ShouldEncodeSingleQuote()
        {
            Assert.Equal("&#39;", HtmlEncoder.Encode("'"));
        }

        [Fact]
        public void Encode_ShouldEncodeXssAttack()
        {
            string xss = "<script>alert(1)</script>";
            string encoded = HtmlEncoder.Encode(xss);
            Assert.Equal("&lt;script&gt;alert(1)&lt;/script&gt;", encoded);
        }

        [Fact]
        public void Encode_ShouldEncodeImageXss()
        {
            string xss = "<img src=x onerror=alert(1)>";
            string encoded = HtmlEncoder.Encode(xss);
            Assert.Equal("&lt;img src=x onerror=alert(1)&gt;", encoded);
        }

        [Fact]
        public void Encode_ShouldEncodeOnClickXss()
        {
            string xss = "\" onclick=\"alert(1)\"";
            string encoded = HtmlEncoder.Encode(xss);
            Assert.Equal("&quot; onclick=&quot;alert(1)&quot;", encoded);
        }

        [Fact]
        public void Encode_ShouldEncodeSpecialCharacters()
        {
            string input = "&<>\"'";
            string encoded = HtmlEncoder.Encode(input);
            Assert.Equal("&amp;&lt;&gt;&quot;&#39;", encoded);
        }

        [Fact]
        public void Encode_ShouldHandlePlainText()
        {
            string input = "Hello World";
            Assert.Equal("Hello World", HtmlEncoder.Encode(input));
        }

        [Fact]
        public void Encode_ShouldHandleNumbers()
        {
            string input = "12345";
            Assert.Equal("12345", HtmlEncoder.Encode(input));
        }

        [Fact]
        public void Encode_ShouldEncodeObject()
        {
            Assert.Equal("", HtmlEncoder.Encode(null));
            Assert.Equal("", HtmlEncoder.Encode(System.DBNull.Value));
            Assert.Equal("test", HtmlEncoder.Encode((object)"test"));
        }

        [Fact]
        public void EncodeAttribute_ShouldHandleNull()
        {
            Assert.Equal("", HtmlEncoder.EncodeAttribute(null));
        }

        [Fact]
        public void EncodeAttribute_ShouldHandleEmptyString()
        {
            Assert.Equal("", HtmlEncoder.EncodeAttribute(""));
        }

        [Fact]
        public void EncodeAttribute_ShouldEncodeSameAsEncode()
        {
            string input = "<script>alert(1)</script>";
            Assert.Equal(HtmlEncoder.Encode(input), HtmlEncoder.EncodeAttribute(input));
        }
    }
}
