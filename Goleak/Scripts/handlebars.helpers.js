Handlebars.registerHelper('list', function (context, options) {
    var out = "", data = {};

    for (var i = 0; i < context.length; i++) {
        if (options.data) { data = Handlebars.createFrame(options.data); }
        context[i].indice = i;
        out += options.fn(context[i], { data: data });
    }

    return out;
});

Handlebars.registerHelper('empty', function (context, block) {
    if (context.length == 0)
        return block(this);
});