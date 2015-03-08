using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.SitePagesImporter.Pipeline.Parsers;

namespace RecipesSearch.SitePagesImporter.Pipeline.Base
{
    public static class ParsersResolver
    {
        private static List<BaseParser> Parsers
        {
            get
            {
                return new List<BaseParser>(new BaseParser[]
                {
                    new RecipeSchemaParser(),
                    new GotovimDoma(),
                });
            }
        }

        public static List<string> GetAvailableParsers()
        {
            return Parsers.Select(parser => parser.Id).ToList();
        }

        internal static BaseParser GetParserById(string id)
        {
            return Parsers.FirstOrDefault(parser => parser.Id == id);
        }
    }
}
