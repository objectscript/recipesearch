var network;

function showGraph(query, exactMatch) {
    if (!query) {
        return;
    }

    if (!!network) {
        network.destroy();
    }

    $('#graph').addClass('loading');

    $.ajax({
        method: "GET",
        url: "/Home/GetGraphData?query=" + window.encodeURIComponent(query) + '&exactMatch=' + exactMatch,
    }).done(function (results) {
        if (!results.length) {
            return;
        }
        prepareGraphData(results);

        $('#graph').removeClass('loading');
    });
}

function prepareGraphData(results) {
    var recipeMap = {};
    var edgeCount = {};
    var nodes = [];
    var edges = [];

    for (var i = 0; i < results.length; ++i) {
        ensureRecipeAdded(recipeMap, nodes, results[i], true);
    }

    for (var i = 0; i < results.length; ++i) {
        var recipe = results[i];

        for (var j = 0; j < recipe.SimilarResults.length; ++j) {
            var similarRecipe = recipe.SimilarResults[j];
            ensureRecipeAdded(recipeMap, nodes, similarRecipe);

            edgeCount[recipe.Id] = !edgeCount[recipe.Id] ? 1 : edgeCount[recipe.Id] + 1;
            edgeCount[similarRecipe.Id] = !edgeCount[similarRecipe.Id] ? 1 : edgeCount[similarRecipe.Id] + 1;

            edges.push({
                from: recipe.Id,
                to: similarRecipe.Id,
                length: 100 + Math.sqrt(similarRecipe.SimilarRecipeWeight)*100
            });
        }
    }

    var maxEdgeCount = -1;
    var maxEdgeCountRecipeId;

    for (var key in edgeCount) {
        if (edgeCount[key] > maxEdgeCount) {
            maxEdgeCount = edgeCount[key];
            maxEdgeCountRecipeId = key;
        }
    }

    var container = document.getElementById('graphContainer');
    var data = {
        nodes: nodes,
        edges: edges,
    };
    var options = {
        width: '100%',
        height: 'calc(100vh - 190px)',
        hover: false,
        navigation: true,
        configurePhysics: false,
        nodes: {
            shape: 'box',
            borderWidth: 1,
            radius: 3
            //color: {
            //    background: '#fff'
            //}
        },
        smoothCurves: {
            dynamic: false
        },
        edges: {
            color: {
                color: 'rgba(0,0,0,0)',
                highlight: 'rgba(147, 197, 75, 0.75)',
                hover: 'rgba(0,0,0,0)'
            },
            width: 0.3
        },
        physics: {
            barnesHut: {
                gravitationalConstant: -20000,
                springLength: 400,
                centralGravity: 0.1,
                springConstant: 0.0002,
                damping: 0.04
            }
        },
        stabilize: true,
        stabilizationIterations: 1000
    };
    network = new vis.Network(container, null, options);

    network.on('stabilizationIterationsDone', function () {
        network.freezeSimulation(true);
        focusOnNode(maxEdgeCountRecipeId);
        network.selectNodes([maxEdgeCountRecipeId]);
        //showNodeTooltip(maxEdgeCountRecipeId);
    });

    network.on('select', function (selected) {
        if (!selected.nodes.length) {
            deselectEdges();
        } else {
            focusOnNode(selected.nodes[0]);
            //showNodeTooltip(selected.nodes[0]);
        } 
        
    });

    network.on('stabilized', function (iterations) {
        console.log('stabilized', iterations);
    });

    network.setData(data, false);
}

function focusOnNode(nodeId) {
    network.focusOnNode(nodeId, {
        scale: 0.75,
        locked: false,
        animation: true
    });
}

function showNodeTooltip(nodeId) {
    $('.recipe-modal').remove();

    var position = network.getPositions([nodeId])[nodeId];
    position = network.canvasToDOM(position);
    console.log(position);

    var element = $('<div class="recipe-modal">expand</div>');
    element.css({
        position: 'absolute',
        top: position.y + $('#graphContainer').offset().top,
        left: position.x + $('#graphContainer').offset().left
    });
    $('body').append(element);
}

function deselectEdges() {
    network.selectEdges([]);
}

function ensureRecipeAdded(map, nodes, recipe, isMain) {

    if (!map[recipe.Id]) {
        map[recipe.Id] = true;

        nodes.push({
            id: recipe.Id,
            label: recipe.Name,
            color: {
                background: !!isMain ? 'rgba(147, 197, 75, 0.75)' : 'rgba(194, 194, 192, 0.75)'
            },
            tooltip: recipe.Name
        });
    }
}