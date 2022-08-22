const path = require('path');
const { CompileSync } = require('./stylesCompile.js');
const { getFilesListByExt } = require('./shopPath.js');
const yargs = require('yargs');
const argv = yargs
    .scriptName('stylesLegacyCompile')
    .usage('$0 <cmd> [args]')
    .option('rootPath', {
        alias: 'r',
        description: 'Root path for find and compile SCSS',
        type: 'string'
    })
    .option('watch', {
        alias: 'w',
        description: 'Enable watch mode',
        type: 'boolean',
        default: false
    })
    .help()
    .alias('help', 'h')
    .argv;

const { rootPath, watch } = argv;

//'areas/landing/**/*.scss',
//'areas/admin/**/*.scss',
//'areas/partners/**/*.scss',

/**
 * 
 * @param {string} rootPath - Начальный каталог. Пример: 'areas/landing/'
 * @param {object} options
 */
function compileStylesLegacy(rootPath, options = { watch: false }) {

    const listSCSS = getFilesListByExt(path.resolve(__dirname, '../', rootPath), /\.scss/);

    const compileSync = new CompileSync(undefined, options);

    listSCSS.forEach(filename => {
        const pathStyle = path.resolve(`${rootPath}\\`, path.dirname(filename));
        const fileNameWithoutExt = path.parse(filename).name;
        compileSync.render(pathStyle, fileNameWithoutExt);
        console.log(`Compile styles legacy ${filename}`);
    });

    if (options != null && options.watch === true) {
        compileSync.watch(rootPath);
    }
}

compileStylesLegacy(rootPath, { watch });
