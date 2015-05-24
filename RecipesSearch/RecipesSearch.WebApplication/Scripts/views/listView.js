(function() {
    window.ListView = function(container) {
        this._container = container;
    }

    window.ListView.prototype = {
        _container: null,

        pinRecipeCallback: null,

        initialize: function() {
            this._initPagination(
                window.paginationData.resultsOnPage, 
                window.paginationData.totalCount,
                window.paginationData.currentPage,
                window.paginationData.currentQuery);

            this._initExpanders();
            this._initBackToTop();
            this._initPinButtons();
        },

        _initPagination:  function (itemsOnPage, items, currentPage, currentQuery) {
            this._container.find('.pagination-holder').pagination({
                items: items,
                itemsOnPage: itemsOnPage,
                currentPage: currentPage,
                cssStyle: 'compact-theme',
                selectOnClick: false,
                onPageClick: function (pageNumber) {
                    window.location.href = '/Home/Index?query=' + currentQuery + '&pageNumber=' + pageNumber;
                    return false;
                }
            });
        },

        _initExpanders: function () {
            var self = this;
            var expandButtons = this._container.find('[data-expander]');

            expandButtons.each(function(idx, item) {
                self._initExpander($(item));
            });
        },

        _initExpander: function ($item) {
            var self = this;
            var id = $item.data('expander');

            $item.on('click', function() {
                self._toggleRecipe(id, $(this));
                return false;
            });
        },

        _toggleRecipe: function (id, $item) {
            var $recipeHolder = this._container.find('#' + id);
            var showText = $item.data('showText');
            var hideText = $item.data('hideText');
            var changeText = !!showText && !!hideText;

            if ($recipeHolder.hasClass('expanded')) {
                $recipeHolder.removeClass('expanded');
                if (changeText) {
                    $item.text(showText);
                }               
            } else {
                $recipeHolder.addClass('expanded');
                if (changeText) {
                    $item.text(hideText);
                }                
            }
        },

        _initBackToTop: function() {
            this._container.find('.back-to-top').on('click', this._backToTop);
        },

        _backToTop: function () {
            $("html, body").animate({ scrollTop: 0 }, "fast");
            return false;
        },

        _initPinButtons: function () {
            var self = this;
            var pinButtons = this._container.find('.recipe-name .pin-icon');

            pinButtons.on('click', function() {
                var pinButton = $(this);
                var recipeId = pinButton.data('id');

                if (self.pinRecipeCallback) {
                    self.pinRecipeCallback(Number(recipeId));
                }
            });
        }
    }

})();