(function () {
    window.ResultsView = function (graphViewContainer, listViewContainer, tabsHolder) {
        this._graphViewContainer = graphViewContainer;
        this._listViewContainer = listViewContainer;
        this._tabsHolder = tabsHolder;
    }

    window.ResultsView.prototype = {
        _graphViewContainer: null,
        _listViewContainer: null,
        _tabsHolder: null,

        _listView: null,
        _graphView: null,
        _itemViews: [],

        initialize: function() {
            this.initListView();
            this.initGraphView();
        },

        initListView: function() {
            this._listView = new window.ListView(this, this._listViewContainer);
            this._listView.pinRecipeCallback = this.pinRecipe.bind(this);
            this._listView.initialize();
            this._addEventListeners();
            this._restorePinnedRecipes();
        },

        initGraphView: function () {
            var query = this._getQueryParameterByName('query');
            var exactMatch = this._getQueryParameterByName('exactMatch');

            this._graphView = new window.GraphView(this, this._graphViewContainer);
            this._graphView.showGraph(query, exactMatch);
        },

        _addEventListeners: function() {
            this._tabsHolder.find('[href="#graph"][role="tab"]').on('shown.bs.tab', function() {
                window.scrollTo(0, document.body.scrollHeight);
            });
        },

        pinRecipe: function(recipeId) {
           for (var i = 0; i < this._itemViews.length; ++i) {
               if (this._itemViews[i].recipeId === recipeId) {
                   return;
               }
           }
           this._addItemView(recipeId);
           this._savedPinnedRecipes();
        },

        unpinRecipe: function (recipeId) {
            for (var i = 0; i < this._itemViews.length; ++i) {
                if (this._itemViews[i].recipeId === recipeId) {
                    this._itemViews.splice(i, 1);

                    if (!this._tabsHolder.find('[role="presentation"].active').length) {
                        this._showTab('list');
                    }
                }
            }
            this._savedPinnedRecipes();
        },

        isShownOnGraph: function (recipeId) {
            if (!this._graphView) {
                return false;
            }
            return this._graphView.hasRecipe(recipeId);
        },

        showOnGraph: function (recipeId) {
            if (!this.isShownOnGraph(recipeId)) {
                return;
            }
            this._showTab('graph');
            this._graphView.focusOnRecipe(recipeId);
        },

        onGraphViewInitialized: function() {
            this._listView.initLocateOnGraphButtons();

            for (var i = 0; i < this._itemViews.length; ++i) {
                this._itemViews[i].initShowOnGraphButton();
            }
        },

        _savedPinnedRecipes: function() {
            if (!window.localStorage) {
                return;
            }

            var selectedIds = [];
            for (var i = 0; i < this._itemViews.length; ++i) {
                selectedIds.push(this._itemViews[i].recipeId);
            }
            window.localStorage.setItem('pinnedRecipes', JSON.stringify(selectedIds));
        },

        _restorePinnedRecipes: function() {
            if (!window.localStorage) {
                return;
            }

            var savedData = window.localStorage.getItem('pinnedRecipes');
            if (!savedData) {
                return;
            }

            var recipes = JSON.parse(savedData);

            for (var i = 0; i < recipes.length; ++i) {
                this.pinRecipe(recipes[i]);
            }
        },

        _addItemView: function(recipeId) {
            var newTab = $(
                '<li role="presentation" class="recipe-item-tab">' +
                '   <a href="#recipeTab_' + recipeId + '" role="tab" data-toggle="tab">' +
                '       <span class="glyphicon glyphicon-refresh spinning"></span>' +
                '   </a>' +
                '</li>');
            var newTabContent = $('<div role="tabpanel" class="tab-pane" id="recipeTab_' + recipeId + '">');

            var itemView = new window.ItemView(this, newTabContent, newTab, recipeId);
            this._itemViews.push(itemView);
            itemView.initialize();

            this._tabsHolder.find('[role=tablist]').prepend(newTab);
            this._tabsHolder.find('.tab-content').append(newTabContent);
        },

        _getQueryParameterByName: function(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        },

        _showTab: function(tabId) {
            this._tabsHolder.find('[href="#' + tabId  + '"][role="tab"]').tab('show');
        }
    };

})();