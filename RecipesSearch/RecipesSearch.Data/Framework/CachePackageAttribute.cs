using System;

namespace RecipesSearch.Data.Framework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CachePackageAttribute : Attribute
    {
        private string _packageName;

        public CachePackageAttribute(string packageName)
        {
            _packageName = packageName;
        }

        public string GetPackageName()
        {
            return _packageName;
        }
    }
}
