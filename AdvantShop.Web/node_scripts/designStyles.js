const { getDirectories } = require('./shopPath.js');
const { CompileSync } = require('./stylesCompile.js');
const FILE_NAME = `styles`;

function compileDesignStyles(rootPath, variables, options = { watch: false }) {
    const backgrounds = getDirectories(`${rootPath}\\backgrounds\\`);
    const colors = getDirectories(`${rootPath}\\colors\\`);
    const themes = getDirectories(`${rootPath}\\themes\\`);

    const compileSync = new CompileSync({ ...variables, cdnDesign: false }, { ...options, cssFilename: `[name]`, ignore : [/styles\.css$/] });
    const compileSyncOnlyColors = new CompileSync(variables, { ...options, ignore : [/styles\.css$/]});

    doType(rootPath, 'backgrounds', backgrounds, pathStyle => { compileSync.render(pathStyle, FILE_NAME); });
    doType(rootPath, 'colors', colors, pathStyle => { compileSyncOnlyColors.render(pathStyle, FILE_NAME) });
    doType(rootPath, 'themes', themes, pathStyle => { compileSync.render(pathStyle, FILE_NAME); });

    if (options != null && options.watch === true) {
        compileSync.watch(rootPath);
        compileSyncOnlyColors.watch(rootPath);
    }
}

function doType(rootPath, type, listNames, compilator) {
    listNames.forEach(name => {
        compilator(`${rootPath}\\${type}\\${name}\\styles\\`)
    });
}

exports.compileDesignStyles = compileDesignStyles;