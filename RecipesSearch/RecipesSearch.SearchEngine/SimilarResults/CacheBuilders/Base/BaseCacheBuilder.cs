using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;

namespace RecipesSearch.SearchEngine.SimilarResults.CacheBuilders.Base
{
    public abstract class BaseCacheBuilder
    {
        public bool UpdateInProgress { get; private set; }
        public bool PreviousBuildFailed { get; private set; }

        protected string BuilderName;

        protected BaseCacheBuilder()
        {
        }

        public Task Build()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    UpdateInProgress = true;
                    PreviousBuildFailed = false;

                    BuildAction();

                    UpdateInProgress = false;
                }
                catch (Exception exception)
                {
                    Logger.LogError(String.Format("{0}.Build failed", BuilderName), exception);
                    UpdateInProgress = false;
                    PreviousBuildFailed = true;
                }
            }, TaskCreationOptions.AttachedToParent);
        }

        protected abstract void BuildAction();

    }
}
