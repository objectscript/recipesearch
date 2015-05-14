using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;

namespace RecipesSearch.SearchEngine.SimilarResults.Builders.Base
{
    public abstract class BaseCacheBuilder
    {
        public bool UpdateInProgress { get; private set; }

        protected string BuilderName;

        protected BaseCacheBuilder()
        {
        }

        public void Build()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    UpdateInProgress = true;

                    BuildAction();

                    UpdateInProgress = false;
                }
                catch (Exception exception)
                {
                    Logger.LogError(String.Format("{0}.Build failed", BuilderName), exception);
                    UpdateInProgress = false;
                }
            }, TaskCreationOptions.AttachedToParent);
        }

        protected abstract void BuildAction();

    }
}
