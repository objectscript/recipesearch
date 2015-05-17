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

            edges.push({
                from: recipe.Id,
                to: similarRecipe.Id,
                length: 200 + similarRecipe.SimilarRecipeWeight,
                title: function() {
                    return '' + similarRecipe.SimilarRecipeWeight;
                }
            });
        }
    }

    var container = document.getElementById('graphContainer');
    var data = {
        nodes: nodes,
        edges: edges,
    };
    var options = {
        width: '100%',
        height: '600px',
        clickToUse: true,
        configurePhysics: false,
        smoothCurves: {
            dynamic: false
        },
        edges: {
           // color: 'white'
        },
        stabilize: true
    };
    network = new vis.Network(container, data, options);
}

function ensureRecipeAdded(map, nodes, recipe, isMain) {

    if (!map[recipe.Id]) {
        map[recipe.Id] = true;

        nodes.push({
            id: recipe.Id,
            label: '' + recipe.Id,
            title: recipe.Name,
            color: {
                background: !!isMain ? 'red' : 'yellow'
            }
        });
    }
}
