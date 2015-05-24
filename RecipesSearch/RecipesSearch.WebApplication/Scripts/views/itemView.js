(function () {
    window.ItemView = function (container, tab, recipeId) {
        this._container = container;
        this._tab = tab;
        this.recipeId = recipeId;
    }

    window.ItemView.prototype = {
        _tab: null,
        _container: null,

        recipeId: null,
        recipe: null,

        removeTabCallback: null,

        initialize: function () {
            var self = this;

            this._fetchData(function() {
                self._tab.find('a').html(self.recipe.RecipeName + '<i class="glyphicon glyphicon-remove icon-remove-recipe"></i>');
                self._addListeners();
            });
        },

        _fetchData: function (callback) {
            var self = this;

            $.ajax({
                method: "GET",
                url: "/Home/GetRecipe?recipeId=" + this.recipeId,
            }).done(function (recipe) {
                self.recipe = recipe;
                callback();
            });
        },

        _addListeners: function () {
            var self = this;

            this._tab.find('.icon-remove-recipe').on('click', function () {
                self.dispose();
                self._tab.remove();
                self._container.remove();

                if (!!self.removeTabCallback) {
                    self.removeTabCallback(self.recipeId);                    
                }

                return false;
            });
        },

        dispose: function() {
            
        }
    }

})();