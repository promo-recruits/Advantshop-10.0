const path = require('path');
const fs = require('fs');
const { projectRootsList } = require('./shopVariables');
/**
 * 
 * @param {('desktop'|'mobile'|'module')} areaName - Область приложения.
 * @param {string} name - Название шаблона в случае, когда area передано desktop/mobile или название модуля
 *  * @param {string} parent - Название шаблона для десктопа. Используется когда переопределена в нем мобильная версия. 
 */
function getBundlePath(area, name, parent, project) {

    let result;

    const root = projectRootsList[project];

    switch (area) {
        case 'desktop':
            result = root + (name != null && name.length > 0 ? 'Templates/' + name + '/' : '');
            break;
        case 'mobile':
            result = root + (parent != null && parent.length > 0 ? 'Templates/' + parent + '/' : '') + 'Areas/Mobile/';
            break;
        case 'module':
            result = root + 'Modules/' + name + '/';
            break;
        default:
            throw new Error(`Area "${area}" empty or not valid`);
    }

    return result;
}

function getPathPages(area, name, parent, project) {

    const pathPages = path.resolve(getBundlePath(area, name, parent, project), 'bundle_config/_pages.js');

    const original = require(pathPages);

    delete require.cache[pathPages];

    return Object.assign(Object.create(Object.getPrototypeOf(original)), original);
}

function getDirectories(source) {
    return fs.readdirSync(path.resolve(source), { withFileTypes: true })
        .filter(dirent => dirent.isDirectory())
        .map(dirent => dirent.name)
}

function removeAllFilesSync(directory) {
    const files = fs.readdirSync(directory);

    for (const file of files) {
        fs.unlinkSync(path.join(directory, file));
    }

}

function removeAllFiles(directory) {
    return fs.promises.readdir(directory)
        .then(files => {
            let promises = files.map(filesItem => fs.promises.unlink(path.join(directory, filesItem)));
            return Promise.all(promises);
        });
}

/**
 * Поиск файлов по расширению
 * @param {string} startPath Начальный каталог поиска
 * @param {RegExp} filter Регулярное выражение поиска
 * @param {Function} callback Функция обратного вызова
 */
function getFilesListByExt(startPath, filter) {

    let list = [];

    (function findItem(startPath, filter) {
        if (!fs.existsSync(startPath)) {
            console.log("no dir ", startPath);
            return;
        }

        var files = fs.readdirSync(startPath);
        for (var i = 0; i < files.length; i++) {
            var filename = path.join(startPath, files[i]);
            var stat = fs.lstatSync(filename);
            if (stat.isDirectory()) {
                findItem(filename, filter); //recurse
            }
            else if (filter.test(filename)) list.push(filename);
        };
    })(startPath, filter);

    return list;
};

exports.getBundlePath = getBundlePath;
exports.getDirectories = getDirectories;
exports.getPathPages = getPathPages;
exports.removeAllFilesSync = removeAllFilesSync;
exports.removeAllFiles = removeAllFiles;
exports.getFilesListByExt = getFilesListByExt;