function getSteps(nodes) {
    var steps = [];

    var n;
    for (n in nodes) {
        var step = {};
        var node = nodes[n];
        var p = {};
        for (c in node.children) {
            var child = node.children[c];
            p[child.Key] = child.Value;
        }
        step["desc"] = node.desc;
        step["ParamBinding"] = p;
        step["name"] = node.name;
        steps.push(step);
    }

    return steps
}