(function () {
    window.ItemView = function (resultsView, container, tab, recipeId) {
        this._container = container;
        this._tab = tab;
        this.recipeId = recipeId;
        this._resultsView = resultsView;
    }

    window.ItemView.prototype = {
        _tab: null,
        _container: null,
        _resultsView: null,

        recipeId: null,
        recipe: null,

        initialize: function () {
            var self = this;

            this._fetchData(function() {
                self._tab.find('a').html(self.recipe.RecipeName + '<i class="glyphicon glyphicon-remove icon-remove-recipe" title="Remove from pinned" ></i>');
                self._addListeners();
                self._buildItemView();
                self.initShowOnGraphButton();
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

                self._resultsView.unpinRecipe(self.recipeId);

                return false;
            });
        },

        _buildItemView: function () {
            var elementHtml = '<div class="recipe-item-view clearfix">';

            elementHtml +=
                '<h4 class="recipe-name">' +
                    '<i class="glyphicon glyphicon-screenshot show-on-graph" title="Show on graph" style="display: none;"></i>' + 
                    this.recipe.RecipeName +
                '</h4>';

            if (!!this.recipe.ImageUrl) {
                elementHtml += '<img class="image" src="' + this.recipe.ImageUrl + '"></img>';
            }
            if (!!this.recipe.Description) {
                elementHtml += '<div class="recipe-item">' + this.recipe.Description + '</div>';
            }

            elementHtml += '<b>Ingredients:</b><br />';
            elementHtml += '<div class="recipe-item">' + this.recipe.Ingredients + '</div>';

            elementHtml += '<b>Instruction:</b><br />';
            elementHtml += '<div class="recipe-item">' + this.recipe.RecipeInstructions + '</div>';

            if (!!this.recipe.AdditionalData) {
                elementHtml += '<b>Additional:</b><br />';
                elementHtml += '<div class="recipe-item">' + this.recipe.AdditionalData + '</div>';
            }

            elementHtml += '<a class="recipe-url" target="_blank" href="' + this.recipe.URL + '">Show in source</a>';

            elementHtml += '</div>';

            this._container.html(elementHtml);
        },

        initShowOnGraphButton: function () {
            var self = this;

            if (!this._resultsView.isShownOnGraph(this.recipeId)) {
                return;
            }

            this._container.find('.show-on-graph').show().off('click').on('click', function () {
                self._resultsView.showOnGraph(self.recipeId);
            });
        },

        dispose: function() {
            
        }
    }

})();