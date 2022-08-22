const webpack = require('webpack');
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const WebpackAssetsManifest = require('webpack-assets-manifest');
const CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin');
const CircularDependencyPlugin = require('circular-dependency-plugin');
const { getBundlePath } = require('./node_scripts/shopPath.js');
let rules = require('./webpack.config.rules.js');


function getRuntimeData(entries) {
    return { file: entries['runtime.js'], name: 'runtime' };
}
/**
 * Возвращает имя файла runtime`а js  в зависимости от названии шаблона или модуля
 * @param {string} area Desctop или mobile
 * @param {string} name Название шаблона или модуля
 * @param {string} parent Название шаблона если есть мобильная версия
 * @param {object} entrypoint Объект точки входа webpack`а
 * @returns {string} Название файла runtime js
 */
function getRuntimeName(area, name, parent, entrypoint) {
    //name: nameTemplate, parent: nameTemplate
    return area === 'module' || (name != null && name.length > 0) || (parent != null && parent.length > 0) || entrypoint.name === 'head' ? false : 'runtime';
}

module.exports = env => {
    const area = env.area;
    const name = env.name;
    const parent = env.parent;
    const pages = env.pages;
    const project = env.project;
    const directoryWork = env.directoryWork;
    const publicPath = env.publicPath;

    let plugins = [
        new CaseSensitivePathsPlugin(),
        new webpack.DefinePlugin({
            WEBPACK_BASE_URL: JSON.stringify(getBundlePath(area, name, parent, project))
        }),
        new CircularDependencyPlugin({
            // exclude detection of files based on a RegExp
            exclude: /node_modules/,
            // add errors to webpack instead of warnings
            failOnError: true,
            // allow import cycles that include an asyncronous import,
            // e.g. via import(/* webpackMode: "weak" */ './file.js')
            allowAsyncCycles: false,
            // set the current working directory for displaying module paths
            cwd: process.cwd(),
        }),
        new MiniCssExtractPlugin({
            filename: '[name].[contenthash].css'
        }),
        //генерируем файл для подключения бандлов
        new WebpackAssetsManifest({
            output: path.resolve(directoryWork, 'dist/bundles.json'),
            entrypoints: true,
            transform(entries, manifest) {
                const runtimeData = getRuntimeData(entries);
                const runtimeFile = runtimeData.file;
                const runtimeName = runtimeData.name;

                //генерируем entrypoints
                Object.keys(entries.entrypoints).forEach(key => {
                    if (entries.entrypoints[key].assets.js != null && entries.entrypoints[key].assets.js.length > 0) {
                        let index = entries.entrypoints[key].assets.js.indexOf(runtimeFile);
                        if (index !== -1) {
                            entries.entrypoints[key].assets.js.splice(index, 1);
                        }
                    }
                });

                //выводим в отдельный entrypoints скрипт runtime, так как он один на всю сборку (runtimeChunk: 'single')
                if (runtimeName != null && runtimeFile != null) {
                    entries.entrypoints[runtimeName] = {
                        assets: {
                            js: [runtimeFile]
                        }
                    };
                }

                const keysFiles = Object.keys(entries).filter(key => key !== 'entrypoints');

                //формируем список всех файлов, для контроля актуальности и необходимости
                entries.files = keysFiles.map(key => entries[key]);

                //удаляем из автоматически сгенерированого json не нужные строки
                keysFiles.forEach(key => delete entries[key]);

                //кастомное поле служит подсказкой для бэка где искать файлы в случае если в текущем bundles.json нет entry, которое запросили
                if (name != null && area !== 'module') {
                    entries.defaultArea = area;
                }

                //режим сборки
                entries.mode = manifest.compiler.options.mode;

                return entries;
            }
        })
    ]


    return {
        entry: pages.getEntries(),
        output: {
            path: path.resolve(directoryWork, `dist/`),
            filename: `[name].[contenthash].js`,
            publicPath: publicPath + `dist/`,
            chunkLoadingGlobal: `${name || 'default'}_webpackChunkwebpack`,
            clean: true
        },
        module: { rules: rules(env) },
        plugins: plugins,
        optimization: {
            moduleIds: 'named',
            runtimeChunk: {
                name: entrypoint => getRuntimeName(area, name, parent, entrypoint)
            },
            splitChunks: {
                chunks: 'all',
                minChunks: Infinity,
                //чтобы рендерился один файл
                minSize: Infinity,
                cacheGroups: {
                    defaultVendors: {
                        name: 'common',
                        test: /common\.js/,
                        //enforce: true
                    }
                }
            }
        },
        resolve: {
            symlinks: false,
            roots: [path.resolve(`images`), path.resolve(`fonts`)],
            alias: {
                'angular': path.resolve(path.join(__dirname, 'node_modules', 'angular'))
            }
        },
        cache: true
    };
};