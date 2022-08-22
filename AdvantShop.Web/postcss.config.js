module.exports = (api) => {

    const variables = api.options.variables;
    let fontsRoot;

    //десктоп-шаблон 
    if (variables.area === 'desktop' && variables.name != null && variables.name.length > 0) {
        fontsRoot = '../../../';
    }
    //мобилка-шаблон
    else if (variables.area === 'mobile' && variables.parent != null && variables.parent.length > 0) {
        fontsRoot = '../../../../../';
    }
    //мобилка-стандартный
    else if (variables.area === 'mobile') {
        fontsRoot = '../../../';
    }
    //десктоп-стандартный
    else if (variables.area === 'desktop') {
        fontsRoot = '../';
    }

    return {
        plugins: [
            require('postcss-urlrewrite')({
                properties: ['src'],
                rules: [{ from: /^\/fonts\//, to: fontsRoot + 'fonts/' }]
            }),
            require('autoprefixer')
        ]
    }
}