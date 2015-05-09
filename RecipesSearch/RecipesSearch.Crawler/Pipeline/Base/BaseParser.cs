using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abot.Poco;
using CsQuery;
using HtmlAgilityPack;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline.Base
{
    internal abstract class BaseParser
    {
        protected const string DelimiterFormat = "{0}; ";

        protected readonly Delimiter[] Delimiters =
        {
            new Delimiter
            {
                Value = '.', 
                SpaceAfter = true, 
                SpaceBefore = false,

                // Ignore if it's a delimiter inside number
                IgnoreRuleChecker = (text, idx) => idx != text.Length - 1 && idx != 0 &&
                                                   !(Char.IsNumber(text[idx - 1]) && Char.IsNumber(text[idx + 1]))
            },
            
            new Delimiter
            {
                Value = ',',
                SpaceAfter = true,
                SpaceBefore = false,

                // Ignore if it's a delimiter inside number
                IgnoreRuleChecker = (text, idx) => idx != text.Length - 1 && idx != 0 &&
                                                   !(Char.IsNumber(text[idx - 1]) && Char.IsNumber(text[idx + 1]))
            },
            
            new Delimiter
            {
                Value = '!',
                SpaceAfter = true,
                SpaceBefore = false,

                // Ignore inside mulitple exclamtion marks
                // Ignore before closing bracket
                IgnoreRuleChecker =
                    (text, idx) => idx != text.Length - 1 && text[idx + 1] != '!' && text[idx + 1] != ')'
            },
            
            new Delimiter
            {
                Value = '?',
                SpaceAfter = true,
                SpaceBefore = false,

                // Ignore inside mulitple question marks
                IgnoreRuleChecker = (text, idx) => idx != text.Length - 1 && text[idx + 1] != '?'
            },
            
            new Delimiter {Value = ';', SpaceAfter = true, SpaceBefore = false},
            
            new Delimiter
            {
                Value = ':',
                SpaceAfter = true,
                SpaceBefore = false,

                // Ignore before closing bracket
                IgnoreRuleChecker = (text, idx) => idx != text.Length - 1 && text[idx + 1] != ')'
            },

            new Delimiter {Value = '-', SpaceAfter = true, SpaceBefore = true},
            
            new Delimiter {Value = '–', SpaceAfter = true, SpaceBefore = true},
            
            new Delimiter {Value = '+', SpaceAfter = true, SpaceBefore = true},

            new Delimiter {Value = '(', SpaceAfter = false, SpaceBefore = true},
            
            new Delimiter
            {
                Value = ')',
                SpaceAfter = true,
                SpaceBefore = false,

                // Ignore if it's a last character in sentence
                IgnoreRuleChecker = (text, idx) => idx != text.Length - 1
                                    && (new[] {',', '.', '!', '?', ':', ';'}).All(c => c != text[idx + 1])
            }
        };

        protected readonly string[] EndLineDelimiters = {".", ",", "!", "?", ";", ":"};

        public abstract string Id { get; }

        public abstract void ParseContent(CrawledPage crawledPage, SitePage sitePage);

        protected virtual string GetTextBySelector(CQ queryObject, string selector, string defaultDelimiter = ".")
        {
            var elements = queryObject.Find(selector);
            var itemText = elements
                .Text((idx, text) =>
                {
                    text = text.Trim();

                    foreach (var delimiter in EndLineDelimiters)
                    {
                        if (text.EndsWith(delimiter))
                        {
                            return text;
                        }
                    }
                    if (text.EndsWith(")"))
                    {
                        return text + ",";
                    }

                    return text + defaultDelimiter;
                })
                .Text();

            if (String.IsNullOrWhiteSpace(itemText) && elements.HasAttr("content"))
            {
                itemText = elements.Attr("content");
                itemText = itemText.Replace('\n', '.');
            }

            itemText = itemText ?? String.Empty;

            itemText = itemText.Trim();

            if (itemText.EndsWith(","))
            {
                itemText = itemText.Substring(0, itemText.Length - 1) + ".";
            }

            foreach (var delimiter in Delimiters)
            {
                itemText = NormalizeDelimiter(itemText, delimiter);
            }

            return itemText;
        }

        protected virtual bool IsElementExitsts(CQ queryObject, string selector)
        {
            return queryObject.Find(selector).Any();
        }

        protected virtual string GetImageUrl(CrawledPage crawledPage, CQ queryObject, string selector)
        {
            var element = queryObject.Find(selector).FirstOrDefault();

            if (element == null)
            {
                return String.Empty;
            }

            var imgSrc = element.Attributes["src"];
            
            if (String.IsNullOrEmpty(imgSrc))
            {
                imgSrc = element.Attributes["href"];
            }

            if (String.IsNullOrWhiteSpace(imgSrc))
            {
                return String.Empty;
            }

            imgSrc = imgSrc.Trim();

            if (imgSrc.StartsWith("http"))
            {
                return imgSrc;
            }

            if (imgSrc.StartsWith("//"))
            {
                return crawledPage.Uri.Scheme + ":" + imgSrc;
            }

            if (imgSrc.StartsWith("/"))
            {
                return crawledPage.Uri.GetLeftPart(UriPartial.Authority) + imgSrc;
            }

            return crawledPage.Uri.AbsoluteUri + "/" + imgSrc;
        }

        private string NormalizeDelimiter(string text, Delimiter delimiter)
        {
            var result = new StringBuilder();

            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] == delimiter.Value &&
                    (delimiter.IgnoreRuleChecker == null || delimiter.IgnoreRuleChecker(text, i)))
                {
                    // Remove all spaces before delimiter
                    while (result.Length != 0 && Char.IsWhiteSpace(result[result.Length - 1]))
                    {
                        result = result.Remove(result.Length - 1, 1);
                    }

                    if (delimiter.SpaceBefore)
                    {
                        result.Append(" ");
                    }

                    result.Append(text[i]);

                    if (delimiter.SpaceAfter)
                    {
                        result.Append(" ");
                    }

                    // Skip all spaces after delimiter
                    while (i + 1 < text.Length && Char.IsWhiteSpace(text[i + 1]))
                    {
                        i++;
                    }
                }
                else
                {
                    result.Append(text[i]);
                }
            }

            return result.ToString();
        }

        protected class Delimiter
        {
            public char Value { get; set; }

            public bool SpaceAfter { get; set; }

            public bool SpaceBefore { get; set; }

            public Func<string, int, bool> IgnoreRuleChecker { get; set; }

        }
    }
}
