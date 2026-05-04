using System;
using DataTableHtmlRenderer;
using Xunit;

namespace DataTableHtmlRenderer.Tests
{
    public class HtmlAttributesTests
    {
        [Fact]
        public void Empty_ShouldReturnEmptyInstance()
        {
            var attributes = HtmlAttributes.Empty;
            Assert.NotNull(attributes);
            Assert.Equal("", attributes.ToString());
        }

        [Fact]
        public void AddClass_ShouldAddSingleClass()
        {
            var attributes = HtmlAttributes.Empty.AddClass("test");
            Assert.Contains("class=\"test\"", attributes.ToString());
        }

        [Fact]
        public void AddClass_ShouldIgnoreNullOrEmpty()
        {
            var attributes = HtmlAttributes.Empty
                .AddClass(null)
                .AddClass("")
                .AddClass("   ");
            Assert.Equal("", attributes.ToString());
        }

        [Fact]
        public void AddClasses_ShouldAddMultipleClasses()
        {
            var attributes = HtmlAttributes.Empty.AddClasses("class1", "class2", "class3");
            string result = attributes.ToString();
            Assert.Contains("class=\"class1 class2 class3\"", result);
        }

        [Fact]
        public void AddClasses_ShouldHandleNullArray()
        {
            var attributes = HtmlAttributes.Empty.AddClasses((string[])null);
            Assert.Equal("", attributes.ToString());
        }

        [Fact]
        public void Set_ShouldAddAttribute()
        {
            var attributes = HtmlAttributes.Empty.Set("data-test", "value");
            Assert.Contains("data-test=\"value\"", attributes.ToString());
        }

        [Fact]
        public void Set_ShouldEncodeAttributeValue()
        {
            var attributes = HtmlAttributes.Empty.Set("title", "<script>alert(1)</script>");
            string result = attributes.ToString();
            Assert.Contains("title=\"&lt;script&gt;alert(1)&lt;/script&gt;\"", result);
        }

        [Fact]
        public void Set_ShouldRemoveAttributeWhenValueIsNull()
        {
            var attributes = HtmlAttributes.Empty.Set("test", "value").Set("test", null);
            Assert.DoesNotContain("test=", attributes.ToString());
        }

        [Fact]
        public void Set_ShouldHandleIntegerValue()
        {
            var attributes = HtmlAttributes.Empty.Set("data-count", 42);
            Assert.Contains("data-count=\"42\"", attributes.ToString());
        }

        [Fact]
        public void Set_ShouldHandleBooleanValue()
        {
            var attributes = HtmlAttributes.Empty.Set("data-enabled", true);
            Assert.Contains("data-enabled=\"true\"", attributes.ToString());
        }

        [Fact]
        public void Data_ShouldAddDataAttribute()
        {
            var attributes = HtmlAttributes.Empty.Data("component", "grid");
            Assert.Contains("data-component=\"grid\"", attributes.ToString());
        }

        [Fact]
        public void Aria_ShouldAddAriaAttribute()
        {
            var attributes = HtmlAttributes.Empty.Aria("label", "Results");
            Assert.Contains("aria-label=\"Results\"", attributes.ToString());
        }

        [Fact]
        public void Id_ShouldAddIdAttribute()
        {
            var attributes = HtmlAttributes.Empty.Id("myTable");
            Assert.Contains("id=\"myTable\"", attributes.ToString());
        }

        [Fact]
        public void Style_ShouldAddStyleAttribute()
        {
            var attributes = HtmlAttributes.Empty.Style("color: red;");
            Assert.Contains("style=\"color: red;\"", attributes.ToString());
        }

        [Fact]
        public void Title_ShouldAddTitleAttribute()
        {
            var attributes = HtmlAttributes.Empty.Title("My Title");
            Assert.Contains("title=\"My Title\"", attributes.ToString());
        }

        [Fact]
        public void Lang_ShouldAddLangAttribute()
        {
            var attributes = HtmlAttributes.Empty.Lang("en-US");
            Assert.Contains("lang=\"en-US\"", attributes.ToString());
        }

        [Fact]
        public void Merge_ShouldCombineAttributes()
        {
            var attrs1 = HtmlAttributes.Empty.Set("data-a", "1").AddClass("class1");
            var attrs2 = HtmlAttributes.Empty.Set("data-b", "2").AddClass("class2");
            var merged = attrs1.Merge(attrs2);

            string result = merged.ToString();
            Assert.Contains("data-a=\"1\"", result);
            Assert.Contains("data-b=\"2\"", result);
            Assert.Contains("class=\"class1 class2\"", result);
        }

        [Fact]
        public void Merge_ShouldHandleNull()
        {
            var attributes = HtmlAttributes.Empty.Set("test", "value");
            var merged = attributes.Merge(null);
            Assert.Equal(attributes.ToString(), merged.ToString());
        }

        [Fact]
        public void Clone_ShouldCreateDeepCopy()
        {
            var original = HtmlAttributes.Empty.Set("test", "value").AddClass("myclass");
            var clone = original.Clone();

            Assert.Equal(original.ToString(), clone.ToString());

            // Modify clone and ensure original is unchanged
            clone.Set("new", "attr");
            Assert.DoesNotContain("new", original.ToString());
        }

        [Fact]
        public void Set_ShouldThrowForForbiddenAttributeNames()
        {
            var attributes = HtmlAttributes.Empty;

            Assert.Throws<ArgumentException>(() => attributes.Set("onclick", "alert(1)"));
            Assert.Throws<ArgumentException>(() => attributes.Set("ONCLICK", "alert(1)"));
            Assert.Throws<ArgumentException>(() => attributes.Set("onload", "alert(1)"));
            Assert.Throws<ArgumentException>(() => attributes.Set("javascript:", "alert(1)"));
        }

        [Fact]
        public void Set_ShouldThrowForJavaScriptUriInValue()
        {
            var attributes = HtmlAttributes.Empty;

            Assert.Throws<ArgumentException>(() => attributes.Set("href", "javascript:alert(1)"));
            Assert.Throws<ArgumentException>(() => attributes.Set("href", "JAVASCRIPT:alert(1)"));
            Assert.Throws<ArgumentException>(() => attributes.Set("src", "vbscript:MsgBox(1)"));
            Assert.Throws<ArgumentException>(() => attributes.Set("href", " javascript:alert(1)"));
            Assert.Throws<ArgumentException>(() => attributes.Set("href", "\tjavascript:alert(1)"));
        }

        [Fact]
        public void Set_ShouldThrowForInvalidAttributeNameCharacters()
        {
            var attributes = HtmlAttributes.Empty;

            Assert.Throws<ArgumentException>(() => attributes.Set("test@attr", "value"));
            Assert.Throws<ArgumentException>(() => attributes.Set("test attr", "value"));
        }

        [Fact]
        public void Set_ShouldAllowValidAttributeNames()
        {
            var attributes = HtmlAttributes.Empty;

            // These should not throw
            attributes.Set("data-test", "value");
            attributes.Set("aria-label", "value");
            attributes.Set("custom-attr", "value");
            attributes.Set("attr_name", "value");
            attributes.Set("attr.name", "value");
            attributes.Set("attr:name", "value");
        }

        [Fact]
        public void ToString_ShouldEncodeSpecialCharactersInClasses()
        {
            var attributes = HtmlAttributes.Empty.AddClass("<script>");
            string result = attributes.ToString();
            Assert.Contains("&lt;script&gt;", result);
        }

        [Fact]
        public void ToString_ShouldEncodeSpecialCharactersInValues()
        {
            var attributes = HtmlAttributes.Empty.Set("title", "<>&\"'");
            string result = attributes.ToString();
            Assert.Contains("&lt;&gt;&amp;&quot;&#39;", result);
        }
    }
}
