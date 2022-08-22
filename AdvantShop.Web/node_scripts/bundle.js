const fs = require('fs');
const path = require('path');
const yargs = require('yargs');
const webpack = require('webpack');
const chalk = require('chalk');
const { getBundlePath, getDirectories, getPathPages } = require('./shopPath');
const { compileDesignStyles } = require('./designStyles');
const { projectRootsList } = require('./shopVariables');
const TEMPLATE_NAME_DEFAULT = '_default';
const argv = yargs
    .scriptName('bundle')
    .usage('$0 <cmd> [args]')
    .option('mode', {
        alias: 'm',
        description: 'Bundle mode: development or production',
        type: 'string',
        choices: ['dev', 'prod'],
        default: 'prod'
    })
    .option('watch', {
        alias: 'w',
        description: 'Enable watch mode',
        type: 'boolean',
        default: false
    })
    .option('templates-list', {
        alias: 'l',
        description: 'Templates list name for build',
        type: 'array'
    })
    .option('templates-all', {
        alias: 'a',
        description: 'Build all templates',
        type: 'boolean',
        default: false
    })
    .option('modules-list', {
        alias: 'c',
        description: 'Modules list name for build',
        type: 'array'
    })
    .option('modules-all', {
        alias: 'b',
        description: 'Build all modules',
        type: 'boolean',
        default: false
    })
    .option('profile', {
        alias: 'p',
        description: 'Generate json-stats file',
        type: 'boolean',
        default: false
    })
    .option('cdnDesign', {
        alias: 'd',
        description: 'Rewrite url in css on cdn for design',
        type: 'boolean',
        default: false
    })
    .option('project', {
        alias: 'j',
        description: 'Project type',
        type: 'string',
        choices: Object.keys(projectRootsList),
        default: 'store'
    })
    .help()
    .alias('help', 'h')
    .argv;

const { mode, watch, templatesList, templatesAll, modulesList, modulesAll, profile, cdnDesign, project } = argv;

const webpackConfigName = `webpack.config.${mode}.js`;

const type = {
    main: 'main',
    template: 'template',
    module: 'module'
};

const projectRoot = projectRootsList[project];

let pathsConfigs = {};

if (project === 'store') {
    if (templatesAll === true || templatesList != null) {
        if (templatesAll) {
            let tplNameList = getDirectories(projectRoot + 'Templates/').filter(name => {
                try {
                    fs.accessSync(path.resolve(projectRoot, 'Templates/', name, 'bundle_config'));
                    return true;
                } catch (err) {
                    return false;
                }
            });

            for (let i = 0; i < tplNameList.length; i++) {
                installItemInChildProcess(tplNameList[i], type.template, { mode, watch, profile, project });
            }

        } else if (templatesList != null && templatesList.length > 0) {
            templatesList.forEach(name => pathsConfigs[name] = aggregateParameter(name, webpackConfigName, project, type.template));
        }
    } else if (modulesAll === true || modulesList != null) {
        if (modulesAll) {

            let tplNameList = getDirectories('Modules/');

            Promise.allSettled(tplNameList.map(name => fs.promises.access(path.resolve('Modules/', name, 'bundle_config')).then(() => name)))
                .then(results => results.forEach(resultsItem => resultsItem.status === `fulfilled` ? installItemInChildProcess(resultsItem.value, type.module, { mode, watch, profile }) : null))

        } else if (modulesList != null && modulesList.length > 0) {
            modulesList.forEach(name => pathsConfigs[name] = aggregateParameter(name, webpackConfigName, project, type.module));
        }
    } else {
        pathsConfigs[TEMPLATE_NAME_DEFAULT] = aggregateParameter('', webpackConfigName, project, type.main);
    }
} else if (project === 'admin') {
    pathsConfigs['adminMobile'] = aggregateParameter('Mobile', webpackConfigName, project, type.template);
}

loopWork(pathsConfigs)
    .catch(error => {
        //для сообщения об ошибки в Teamcity
        process.exitCode = 1;

        if (error != null) {
            console.error(chalk`{red Error: ${error.stack || error}}`);
        }
    });

async function loopWork(objWithAllPathsConfigs) {
    for (const key in objWithAllPathsConfigs) {
        await loopWorkIteartion(key, objWithAllPathsConfigs[key]);
    }
}

function loopWorkIteartion(name, objData) {
    console.time(chalk`{cyan Time build}`);

    return Promise.allSettled(processGetConfig(objData))
        .then(results => results.map(resultItem => {
            if (resultItem.status === 'fulfilled') {
                return { ...resultItem.value, watch, stats: profile === true ? 'detailed' : 'errors-warnings', profile };
            } else {
                console.warn(chalk`{yellow Warning}: ${resultItem.reason}`);
                return undefined;
            }
        }))
        .then(result => result.filter(item => item !== undefined))
        .then(result => { return result.length > 0 ? start(name, result, objData) : true })
        .catch(err => Promise.reject(err))
        .finally(() => { console.timeEnd(chalk`{cyan Time build}`); });
}

function aggregateParameter(direntName, webpackConfigName, project, typeArea) {

    const objItem = [];

    if (typeArea !== type.module) {

        objItem.push(addParameters('desktop', direntName, null, webpackConfigName, project));

        objItem.push(addParameters('mobile', null, direntName, webpackConfigName, project));

    } else if (typeArea === type.module) {
        const dirModules = 'Modules/' + direntName;
        if (fs.existsSync(dirModules)) {
            objItem.push(addParameters('module', direntName, null, webpackConfigName, project));
        } else {
            console.error(chalk`{red Error: Not found module: ${direntName}}`);
        }
    }

    return objItem;
}

function addParameters(area, name, parent, webpackConfigName, project) {
    return { area, name, parent, cdnDesign, webpackConfigName, project };
}

function processGetConfig(storage) {
    return storage.map(item => getConfig(item.area, item.name, item.parent, item.webpackConfigName, item.project));
}

function getConfig(area, name, parent, webpackConfigName, project) {
    const bundlePath = getBundlePath(area, name, parent, project);

    let webpackConfigFilename = path.resolve(bundlePath, webpackConfigName);
    let useDefaultConfig = false;

    if (fs.existsSync(webpackConfigFilename) === false) {
        useDefaultConfig = true;
        webpackConfigFilename = path.resolve(projectRoot, webpackConfigName);
    }

    const directoryWork = path.resolve(__dirname, '../', bundlePath);
    const pathPages = path.resolve(directoryWork, 'bundle_config/_pages.js');
    const publicPath = bundlePath;

    if (fs.existsSync(pathPages) === false) {
        return Promise.reject(`Not found pathPages: ${pathPages}`);
    } else if (useDefaultConfig === false) {
        if (area !== 'module' && ((name != null && name.length > 0) || (parent != null && parent.length > 0))) {
            console.warn(chalk`{yellow Warning}: Use custom webpack config.`);
        }
    }

    const pages = getPathPages(area, name, parent, project);
    const config = require(webpackConfigFilename)({ area, name, parent, pages, directoryWork, publicPath, cdnDesign, project });
    config.name = `${project}_${area || 'N\\A'}_${parent || 'N\\A'}_${name || 'N\\A'}`;
    return Promise.resolve(config);
}

function start(name, config, objData) {
    const date = new Date();
    console.log(chalk`{green Start build "${name}" for ${config.length} items in ${date.toLocaleTimeString()} ${watch ? 'with watch mode' : ''}}`);

    if (name !== TEMPLATE_NAME_DEFAULT) {
        const itemWhereMaybePackageJson = objData.find(x => x.area === 'module' || x.area === 'desktop');

        if (itemWhereMaybePackageJson != null) {
            installPackagesFromCustomPackageJson(getBundlePath(itemWhereMaybePackageJson.area, itemWhereMaybePackageJson.name, itemWhereMaybePackageJson.parent, itemWhereMaybePackageJson.project));
        }
    }

    return processWebpack(config)
        .then(() => {
            if (project !== 'store') {
                return;
            }

            const desktopData = objData.find(x => x.area === 'desktop');
            if (desktopData != null) {
                const pathToBundle = path.resolve(getBundlePath(desktopData.area, desktopData.name, desktopData.parent, desktopData.project));
                //compile design styles
                compileDesignStyles(`${pathToBundle}\\design\\`, { cdnDesign, ...desktopData }, { watch });
                console.log(chalk`{green Success design styles}`)
            }
        })
        .then(() => console.log(chalk`{green Finish build!!!}`))
        .catch(err => Promise.reject(err));
}

function processWebpack(config) {
    return new Promise((resolve, reject) => {
        webpack(config, (err, stats) => {
            processResultWebpack(err, stats)
                .then(resolve)
                .catch(reject);
        });
    })
}

function processResultWebpack(err, data) {
    return new Promise((resolve, reject) => {
        if (err) {
            console.error(chalk`{red Error: ${err.stack || err}}`);
            if (err.details) {
                console.error(err.details);
            }

            return reject(err);
        }

        const tpl = item => `${item.moduleName} \n\r Message: ${item.message} \n\r Stack: ${item.moduleIdentifier}`;

        for (let i = 0; i < data.stats.length; i++) {
            const stats = data.stats[i];
            const info = stats.toJson(profile === true ? 'detailed' : 'errors-warnings');

            if (stats.hasErrors()) {
                info.errors.forEach(error => console.error(chalk`{red Error: ${tpl(error)} } `));
                return reject();
            }

            if (stats.hasWarnings()) {
                info.warnings.forEach(warn => {
                    console.warn(chalk`{yellow Warning: ${tpl(warn)} }`);
                })
            }

            if (info.errorsCount > 0) {
                console.error(chalk`{red Failed: ${info.name} } `)
            } else {
                console.log(chalk`{green Success: ${info.name} } `)
            }

            if (profile) {
                fs.writeFileSync(path.resolve(stats.compilation.outputOptions.path, 'stats.json'), JSON.stringify(info));
            }
        }

        return resolve();
    })
}

function installPackagesFromCustomPackageJson(workingDir) {
    if (fs.existsSync(`${workingDir}package.json`)) {
        const execSync = require('child_process').execSync;
        execSync(`npm i`, { cwd: workingDir });
    }
}

function installItemInChildProcess(direntName, itemType, { mode, watch, profile }) {
    const execSync = require('child_process').execSync;
    execSync(`npm run ${mode === 'dev' ? 'dev' : 'build'} ${itemType === type.template ? '-- -l' : '-- -c'} ${direntName} ${watch ? '-- -w' : ''} ${profile ? '-- -p' : ''}`, { stdio: 'inherit' });
}

