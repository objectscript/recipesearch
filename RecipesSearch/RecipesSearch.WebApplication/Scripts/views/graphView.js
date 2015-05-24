(function () {

    window.GraphView = function (resultsView, container) {
        this._container = container;
        this._resultsView = resultsView;
    }

    var graphOptions = {
        width: '100%',
        height: 'calc(100vh - 167px)',
        autoResize: true,
        configure: {
            enabled: false,
            filter: 'physics',
            showButton: true
        },
        edges: {
            physics: true,
            smooth: false,
            color: {
                color: 'rgba(0, 0, 0, 0)',
                hover: 'rgba(0, 0, 0, 0)',
                highlight: 'rgba(147, 197, 75, 0.75)',
                inherit: false               
            },
            width: 0.1
        },
        nodes: {
            shape: 'box',
            borderWidth: 0.1
        },
        interaction: {
            dragNodes: false,
            hideEdgesOnDrag: false,
            hover: true,
            navigationButtons: true,
            selectable: false,
            selectConnectedEdges: true
        },
        physics: {
            barnesHut: {
                gravitationalConstant: -20000,
                springLength: 1,
                centralGravity: 0.1,
                springConstant: 0.0002,
                damping: 0.04,
                avoidOverlap: 0.7
            },
            stabilization: {
                enabled: true,
                iterations: 500,
                updateInterval: 1,
                onlyDynamicEdges: false,
                fit: true
            }
        }
    };

    GraphView.prototype = {
        _container: null,
        _resultsView: null,

        _recipes: null,
        _network: null,
        _canvasContext: null,

        _graphData: null,
        _centralRecipeId: null,

        _currentSelectedNodeId: null,
        _idToRecipeMap: {},

        _showAllEdges: false,

        showGraph: function (query, exactMatch) {
            var self = this;

            if (!!this.network) {
                this.dispose();
            }

            if (!query) {
                return;
            }

            this._toggleProgress(true);

            self._setProgressText('Fetching data...');
            this._fetchData(query, exactMatch, function () {                
                self._prepareGraphData();
                self._setProgressText('Preparing graph...');
                self._initNetwork();
            });
        },

        hasRecipe: function(recipeId) {
            return !!this._idToRecipeMap[+recipeId];
        },

        focusOnRecipe: function (recipeId) {
            return this._focusOnNode(recipeId, 250, false);
        },

        _fetchData: function (query, exactMatch, callback) {
            var self = this;
            exactMatch = exactMatch || 'true';

            $.ajax({
                method: "GET",
                url: "/Home/GetGraphData?query=" + window.encodeURIComponent(query) + '&exactMatch=' + exactMatch,
            }).done(function (recipes) {
                self._recipes = recipes;
                callback();
            });
        },

        _toggleProgress: function (show) {
            if (show) {
                this._container.addClass('loading');
            } else {
                this._container.removeClass('loading');
            }
        },

        _setProgressText: function (text) {
            this._container.find('.loading-holder .note').text(text);          
        },

        _prepareGraphData: function () {
            var self = this;

            var edgeCount = {};
            var nodes = [];
            var edges = [];

            for (var i = 0; i < this._recipes.length; ++i) {
                ensureRecipeAdded(this._recipes[i], true);
            }

            for (var i = 0; i < this._recipes.length; ++i) {
                var recipe = this._recipes[i];

                for (var j = 0; j < recipe.SimilarResults.length; ++j) {
                    var similarRecipe = recipe.SimilarResults[j];
                    ensureRecipeAdded(similarRecipe);

                    edgeCount[recipe.Id] = !edgeCount[recipe.Id] ? 1 : edgeCount[recipe.Id] + 1;
                    edgeCount[similarRecipe.Id] = !edgeCount[similarRecipe.Id] ? 1 : edgeCount[similarRecipe.Id] + 1;

                    edges.push({
                        from: recipe.Id,
                        to: similarRecipe.Id,
                        length: 100 + Math.sqrt(similarRecipe.SimilarRecipeWeight) * 100
                    });
                }
            }

            var maxEdgeCount = -1;
            var maxEdgeCountRecipeId = -1;

            for (var key in edgeCount) {
                if (edgeCount[key] > maxEdgeCount) {
                    maxEdgeCount = edgeCount[key];
                    maxEdgeCountRecipeId = key;
                }
            }

            this._centralRecipeId = maxEdgeCountRecipeId;
            this._graphData = {
                nodes: nodes,
                edges: edges
            };

            function ensureRecipeAdded(recipeToAdd, isMain) {

                if (!self._idToRecipeMap[recipeToAdd.Id]) {
                    self._idToRecipeMap[recipeToAdd.Id] = recipeToAdd;

                    var nodeColor = !!isMain ? 'rgba(147, 197, 75, 0.75)' : 'rgba(194, 194, 192, 0.75)';
                    nodes.push({
                        id: recipeToAdd.Id,
                        label: recipeToAdd.Name,
                        color: {
                            background: nodeColor,
                            border: nodeColor,
                            highlight: {
                                border: '#D2E5FF',
                                background: '#D2E5FF'
                            }
                        },
                        tooltip: recipeToAdd.Name
                    });
                }
            }
        },

        _initNetwork: function () {
            
            var container = this._container.find('#graphContainer').get(0);

            this.__newtorkStartTime = performance.now();
            this._network = new vis.Network(container, this._graphData, graphOptions);

            this._canvasContext = $(container).find('canvas').get(0).getContext('2d');

            this._addNetworkEvents();
        },

        _addNetworkEvents: function () {
            var self = this;

            this._network.on('stabilizationIterationsDone', function () {
                self._network.stopSimulation();
                self._focusOnNode(self._centralRecipeId, 600);

                self._toggleProgress(false);
                self._createShowAllEdgesButton();
                self._resultsView.onGraphViewInitialized();

                console.log('stabilization time:', performance.now() - self.__newtorkStartTime);
            });

            this._network.on('click', function (object) {
                self._handleCanvasClick(object);
            });

            this._network.on('dragStart', function () {
                self._hideCustomElements();
            });

            this._network.on('zoom', function () {
                self._hideCustomElements();
            });

            this._network.on('hoverNode', function (object) {
                self._showNodeTooltip(object.node);
            });

            this._network.on('blurNode', function (object) {
                self._removeNodeTooltip(object.node);
            });
        },

        _handleCanvasClick: function (clickEventData) {
            var self = this;
            var node = self._network.getNodeAt(clickEventData.pointer.DOM);

            if (!node) {
                self._network.unselectAll();
                self._currentSelectedNodeId = null;
            } else {
                var boundingBox = node.shape.boundingBox;
                var tooltipHeight = boundingBox.bottom - boundingBox.top;
                var tooltipWidth = tooltipHeight;
                var clickPositon = clickEventData.pointer.canvas;
                var openExpanded = false;

                if (clickPositon.x <= boundingBox.right && clickPositon.x >= boundingBox.right - tooltipWidth
                    && clickPositon.y <= boundingBox.bottom && clickPositon.y >= boundingBox.top) {
                    openExpanded = true;                   
                }

                self._focusOnNode(node.id, 250, openExpanded);
            }           
        },

        _focusOnNode: function (nodeId, animationDuration, openExpanded) {
            var self = this;

            if (nodeId == self._currentSelectedNodeId) {

                if (!!openExpanded) {
                    self._expandNode(nodeId);
                }

                return;
            }

            this._deselectEdges();
            this._hideCustomElements();            

            animationDuration = animationDuration || 1000;
            this._network.focus(nodeId, {
                scale: 0.75,
                locked: false,
                animation: {
                    duration: animationDuration
                }
            });

            this._runAfterAnimation(function () {
                self._removeNodeTooltip();
                self._network.selectNodes([nodeId], true);
                self._currentSelectedNodeId = nodeId;
                self._network.redraw();

                if (!!openExpanded) {
                    self._network.once('afterDrawing', function () {
                        self._expandNode(nodeId);                       
                    });
                }
            });           
        },

        _deselectEdges: function () {
            this._network.selectEdges([]);
        },

        _showNodeTooltip: function (nodeId) {
            var self = this;

            var scale = this._network.getScale();

            if (scale < 0.5) {
                return;
            }

            this._network.on('afterDrawing', function() {
                var nodePosition = self._network.getBoundingBox(nodeId);
                var height = nodePosition.bottom - nodePosition.top;
                var width = height;

                self._canvasContext.fillStyle = "rgba(249, 249, 249, 0.85)";
                self._canvasContext.fillRect(nodePosition.right - width, nodePosition.top, width, height);

                self._canvasContext.font = (width - 4) + 'px \'Glyphicons Halflings\'';
                self._canvasContext.fillStyle = 'black';
                self._canvasContext.fillText(String.fromCharCode(0xe096), nodePosition.right - width + width/2, nodePosition.top + width/2);
            });
        },

        _removeNodeTooltip: function() {
            this._network.off('afterDrawing');
        },

        _expandNode: function (nodeId) {
            var self = this;

            if (this._recipeModalOpenedId == nodeId) {
                this._removeExpandedModal();
                return;
            }

            this._removeExpandedModal();

            var nodePosition = self._network.getBoundingBox(nodeId);
            var domPostionTopLeft = self._network.canvasToDOM({ y: nodePosition.top, x: nodePosition.left });
            
            var recipe = this._idToRecipeMap[nodeId];

            var elementHtml = '<div class="recipe-expanded-modal" data-id="' + nodeId + '">';

            elementHtml +=
                '<div class="header">' +
                    recipe.Name +
                    '<i class="glyphicon glyphicon-remove close-icon" title="Pin recipe"></i>' +
                    '<i class="glyphicon glyphicon-pushpin pin-icon" title="Close modal"></i>' +
                '</div>';

            elementHtml += '<div class="content">';

            if (!!recipe.ImageUrl) {
                elementHtml += '<img class="image" src="' + recipe.ImageUrl + '"></img>';
            }
            if (!!recipe.Description) {
                elementHtml += '<div class="recipe-item">' + recipe.Description + '</div>';
            }

            elementHtml += '<b>Ингредиенты:</b><br />';
            elementHtml += '<div class="recipe-item">' + recipe.Ingredients + '</div>';

            elementHtml += '<b>Инструкция по приготовлению:</b><br />';
            elementHtml += '<div class="recipe-item">' + recipe.RecipeInstructions + '</div>';

            if (!!recipe.AdditionalData) {
                elementHtml += '<b>Дополнительная информация:</b><br />';
                elementHtml += '<div class="recipe-item">' + recipe.AdditionalData + '</div>';
            }

            elementHtml += '<a class="recipe-url" target="_blank" href="' + recipe.URL + '">' + recipe.URL  + '</a>';

            elementHtml += '</div></div>';

            var element = $(elementHtml);

            element.css({
                position: 'absolute',
                top: domPostionTopLeft.y + $('#graphContainer').offset().top - 200 - 3,
                left: domPostionTopLeft.x + $('#graphContainer').offset().left,
            });

            $('body').append(element);
            self._recipeModalOpenedId = nodeId;

            element.on('mousemove', function() {
                self._removeNodeTooltip();
                self._network.redraw();
            });

            element.find('.close-icon').on('click', function() {
                self._removeExpandedModal();
            });

            element.find('.pin-icon').on('click', function () {
                self._resultsView.pinRecipe(nodeId);
                self._removeExpandedModal(nodeId);
                self._expandNode(nodeId);
            });

            window.setTimeout(function () {
                $('body').on('click.modalOutsideClick', self._handleClickOutsideModal.bind(self));
            }, 0);
            
        },

        _removeExpandedModal: function () {
            $('body').off('click.modalOutsideClick');

            this._recipeModalOpenedId = null;
            $('.recipe-expanded-modal').remove();
        },

        _handleClickOutsideModal: function(event) {
            var target = $(event.target);
            if (target.parents('.recipe-expanded-modal').length > 0 || target.is('.recipe-expanded-modal')) {
                return;
            }
            this._removeExpandedModal();
        },

        _runAfterAnimation: function (callback) {
            var self = this;
            this._network.on('animationFinished', function () {
                self._network.off('animationFinished');
                callback();
            });
        },

        _hideCustomElements: function () {
            this._removeNodeTooltip();
            this._removeExpandedModal();
        },

        _createShowAllEdgesButton: function () {
            var allEdgesButton = $('<div class="vis-button showAllEdges" style="touch-action: none; -webkit-user-select: none; -webkit-user-drag: none; -webkit-tap-highlight-color: rgba(0, 0, 0, 0);"></div>');
            this._container.find('.vis-navigation').append(allEdgesButton);

            allEdgesButton.on('click', this._toggleShowAllEdges.bind(this));
        },

        _toggleShowAllEdges: function() {
            if (!this._showAllEdges) {
                this._network.setOptions({
                    edges: {
                        color: {
                            inherit: true
                        }
                    }
                });
                this._network.stopSimulation();
            } else {
                this._network.setOptions({
                    edges: {
                        color: graphOptions.edges.color
                    }
                });
                this._network.stopSimulation();
            }

            this._showAllEdges = !this._showAllEdges;
        },

        dispose: function () {
            this._network.destroy();
            this._network = null;
            $('body').off('click.modalOutsideClick');
        }
    };
})();