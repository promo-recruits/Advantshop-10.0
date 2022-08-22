const { Cratocss } = require('./cratocss');
const http = require('http');
const fs = require('fs');
const fsPromises = fs.promises;
const path = require('path');
const yargs = require('yargs');
const chalk = require('chalk');
const _listURLs = new Map(require('./criticalURLs.js').list);
const { removeAllFiles } = require('./shopPath.js');
const DEFAULT_TEMPLATE = '_default';
const argv = yargs
    .scriptName('critical-css')
    .usage('$0 <cmd> [args]')
    .option('baseUrl', {
        alias: 'b',
        description: 'Base URI to site',
        type: 'string'
    })
    .option('template-name', {
        alias: 'n',
        description: 'Template name',
        type: 'string'
    })
    .option('screenshots', {
        alias: 's',
        description: 'Enable generate screenshot page',
        type: 'boolean',
        default: false
    })
    .option('grab-font-face', {
        alias: 'g',
        description: 'Enable grab font-face css text',
        type: 'boolean',
        default: false
    })
    .help()
    .alias('help', 'h')
    .argv;
let __baseUrl = argv.baseUrl || 'http://localhost:8825/';
const baseUrl = new URL(__baseUrl);
const { templateName, screenshots, grabFontFace } = argv;

if (__baseUrl[__baseUrl.length - 1] != '/') {
    __baseUrl += '/';
}
/**
 * Зпускает несколько job-ов для генерации стилей
 * @returns {Promise} Возвращает промис
 */
async function generate(pathData, listURLs) {
    console.log(chalk.green(`Start generate critical css for ${listURLs.size} pages in ${pathData.templateName || 'default'} template `));

    const isExistMobile = fs.existsSync(pathData.mobile);

    process.stdout.write(`Process desktop version\n\r`);
    
     const options = {
         width: 1300,
         height: 1300,
         output: pathData.critical.desktop,
         baseURL: __baseUrl,
         screenshots,
         grabFontFace,
     }
    
     const cratocss = new Cratocss(options);
    
     await cratocss.generate(listURLs);

    if (isExistMobile) {
        
        process.stdout.write(`Process mobile version\n\r`);
        
        const optionsMobile = {
            output: pathData.critical.mobile,
            baseURL: __baseUrl,
            screenshots,
            grabFontFace,
            height: 1000,
            device: `iPhone 13 Pro Max`,
        }
        const cratocssMobile = new Cratocss(optionsMobile);

        await cratocssMobile.generate(listURLs);
    }
}

/**
 * Создает или очищает директорию
 * @param {string} directoryForCheck - Путь директории
 * @returns {Promise}
 */
async function folderCreateOrClean(directoryForCheck) {
    return fsPromises.access(directoryForCheck)
        .then(() => removeAllFiles(directoryForCheck))
        .catch(() => fsPromises.mkdir(directoryForCheck))
}

/**
 * Возвращает данные о директориях
 * @param {string | undefined} templateName
 * @returns {Object}
 */
function getPathData(templateName) {

    const templatePath = templateName != null ? `Templates/${templateName}/` : '';

    return {
        templateName: templateName,
        desktop: path.resolve(__dirname, `../${templatePath}/`),
        mobile: path.resolve(__dirname, `../${templatePath}Areas/Mobile/`),
        dist: {
            desktop: path.resolve(__dirname, `../${templatePath}dist/`),
            mobile: path.resolve(__dirname, `../${templatePath}Areas/Mobile/dist/`)
        },
        critical: {
            desktop: path.resolve(__dirname, `../${templatePath}_criticalcss/`),
            mobile: path.resolve(__dirname, `../${templatePath}Areas/Mobile/_criticalcss/`)
        }
    }
}
/**
 * Проверка, что проект собран в режиме продакшена
 * @param {object} pathData
 * @returns {boolean} Возвращает истину, если проект собран в режимене продакшена 
 */
async function checkMode(pathData) {

    const bundlePath = path.resolve(pathData.dist.desktop, `bundles.json`);

    return fsPromises.access(bundlePath)
        .then(() => require(bundlePath).mode === `production`)
        //старый шаблон
        .catch(() => true);
}

/**
 * 
 * @param {any} name
 */
async function iteration(name, listURLs) {
    const pathData = getPathData(name);

    return await checkMode(pathData)
        .then((isProduction) => {

            if (isProduction === false) {
                return Promise.reject({ templateName: pathData.templateName, version: 'desktop', error: 'Для генерации критических стилей необходимо, чтобы проект был собран в режиме продакшена' });
            }

            return true;
        })
        .then(() => folderCreateOrClean(pathData.critical.desktop))
        .then(() => {
            return fsPromises.access(pathData.mobile)
                .then(() => folderCreateOrClean(pathData.critical.mobile))
                .catch(() => { /*ignore*/ })

        })
        .then(() => generate(pathData, listURLs));
}

/**
 * Запуск генерации
 * @param {string[] | undefined} templatesNameList Список названий шаблона
 * @returns {Promise}
 */
async function start(listURLs) {
    await installTemplate(templateName);
    await iteration(templateName != DEFAULT_TEMPLATE ? templateName : null, listURLs);
};

/**
 * Устанавливает шаблон
 * @param {string} templateId Название шаблона
 * @returns {Promise} 
 */
async function getAuthorizeCookie() {
    
    let urlObj = new URL(__baseUrl + 'user/logintoken');

    urlObj.searchParams.append('email', 'admin');
    urlObj.searchParams.append('hash', '639cb78c07b7d6e1b431ca06d97111076c07df8232a0ad785293631069d8fde8');

    const options = {
        headers: {
            'Content-Type': 'text/html',
        }
    }

    return new Promise((resolve, reject) => {
        let data = '';
        const request = http.get(urlObj.toString(), options, response => {
            if (response.statusCode >= 400) {
                reject({ error: `Server return status code ${response.statusCode} on getAuthorizeCookie` });
            } else {
                response.on('data', chunk => {
                    data += chunk;
                })

                request.on('error', error => {
                    reject(error);
                });

                response.on('end', () => {
                    resolve(response.headers['set-cookie']);
                });
            }
        });

        request.end();
    });
}

/**
 * Устанавливает шаблон
 * @param {string} templateId Название шаблона
 * @returns {Promise} 
 */
async function installTemplate(templateId) {

    let urlObj = new URL(__baseUrl + 'adminv3/design/ApplyTemplate');

    urlObj.searchParams.append('templateId', templateId);

    const authCookie = await getAuthorizeCookie();

    const options = {
        headers: {
            'Content-Type': 'text/html',
            'Cookie': authCookie
        }
    }

    return new Promise((resolve, reject) => {
        const request = http.get(urlObj.toString(), options, response => {

            let data = '';

            if (response.statusCode >= 400) {
                reject({ error: `Server return status code ${response.statusCode} on apply template` });
            } else {
                response.on('data', chunk => {
                    data += chunk;
                })

                request.on('error', error => {
                    reject(error);
                });

                response.on('end', () => {
                    resolve(data);
                });
            }
        }).end();
    });
}

let attempt = 0;

process.on('uncaughtException', error => {
    if (attempt === 3) {
        console.error(chalk.red(error.stack));
        process.exitCode = 1;
    } else {
        console.log(`uncaughtException in ${templateName || 'default'}`);
        attempt += 1;
        console.log(`attempt ${attempt} for ${templateName || 'default'}`);
        (async () => await go(new Map(_listURLs)))();
    }
});

async function go(listURLs) {
    try {
        await start(listURLs);
        console.log(chalk.green(`Finish ${templateName || 'default'} template`));
    } catch (err) {
        let errorText;

        if (err instanceof Error) {
            errorText = err.message + '\n\r' + err.stack;
        } else if (typeof err === 'string') {
            errorText = err;
        } else {
            errorText = Object.keys(err).filter(key => err[key] != null).map(key => `${key + ': ' + err[key]}`).join(`\n\r`);
        }

        console.error(chalk.red(`Finish with error`));
        console.error(chalk.red(errorText));
        process.exitCode = 1;
    }
}

async function processData(listURLs) {
    const authCookie = await getAuthorizeCookie();

    function searchParamsAppend(urlList) {
        return urlList.map(urlItem => {
            let url = new URL(urlItem, __baseUrl);
            url.searchParams.append(`debugmode`, `criticalcss`);
			//depricated code for 10 version
			url.searchParams.append(`excludeCriticalCSS`, `true`);
            return url.href;
        });
    }
    let name, data;
    return Array.from(listURLs).map(listURLsItem => {
        name = listURLsItem[0];
        data = listURLsItem[1];
        if (typeof data === 'object' && Array.isArray(data) === false) {
            if (data.admin === true) {
                data.cookies = parseCookie(authCookie);
            }
            data.url = searchParamsAppend(Array.isArray(data.url) ? data.url : [data.url]);
        } else if (Array.isArray(data)) {
            data = searchParamsAppend(data);
        } else if (typeof data === 'string') {
            data = searchParamsAppend([data]);
        }

        return [name, data]
    });
}

function parseCookie(cookies) {
    const baseUrl = new URL(__baseUrl);
    const cookiesParsed = [];
    let itemAsObj;
    for (let item of cookies) {
        itemAsObj = item.split('; ').reduce((prev, current, index) => {
            let [key, value] = current.split('=');

            if (index === 0) {
                prev['name'] = key;
                prev['value'] = value;
            } else {
                let _key;
                let _val;

                if (key === 'expires') {
                    _val = Math.round((new Date(value)).getTime()/1000);
                } else if (key === 'SameSite') {
                    _key = 'sameSite';
                } else if (key === 'HttpOnly') {
                    _key = 'httpOnly';
                    _val = true;
                }

                prev[_key || key] = _val || value;
            }

            return prev;
        }, {});
        itemAsObj['domain'] = baseUrl.hostname;
        cookiesParsed.push(itemAsObj);
    }

    return cookiesParsed;
}



(async () => {

    let list = await processData(_listURLs);

    go(new Map(list));
})();