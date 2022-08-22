const { writeFile, mkdir } = require('fs/promises');
const { existsSync } = require('fs');
const path = require('path');
const { chromium, devices } = require('playwright');
const http = require('http');
const https = require('https');
const postcss = require('postcss');
const postcssUrl = require('postcss-url');
const postcssDiscard = require('postcss-discard');

class Cratocss {
    static REGEXP_DOMAIN = /(http|https):\/\/(\S{1,})\.([\w\d]{2,})?\//;
    /**
     * 
     * @param {Object} options 
     * @param {string} options.baseURL BaseURL site from source critical css
     * @param {string} options.output Path for save file critical css
     * @param {string} options.filename Filename critical css. Default [name].critical.css
     * @param {string} options.device https://playwright.dev/docs/api/class-playwright#playwright-devices
     * @param {number} options.width
     * @param {number} options.height
     * @param {boolean} options.grabFontFace
     * @param {boolean} options.screenshots
     * @param {number} options.parallelStreamsCount
     * @param {Object} options.discardOptions https://github.com/bezoerb/postcss-discard
     * @param {String|RegExp|Function} options.discardOptions.atrule Match atrule like @font-face
     * @param {String|RegExp|Function} options.discardOptions.rule Match rule like .big-background-image {...}
     * @param {String|RegExp|Function} options.discardOptions.decl Match declarations
     * @param {String} options.discardOptions.css CSS String or path to file containing css
     * @param {Object} options.cssUrlOptions https://github.com/postcss/postcss-url
     */
    constructor(options) {
        if (options.baseURL == null) {
            throw Error(`Option "baseURL" is required`);
        }
        this.baseURLRaw = options.baseURL;
        this.baseURL = typeof options.baseURL === 'string' ? new URL(options.baseURL) : options.baseURL;
        this.output = options.output;
        this.filename = options.filename || `[name].critical.css`;
        this.device = options.device;
        this.width = options.width;
        this.height = options.height;
        this.grabFontFace = options.grabFontFace || false;
        this.screenshots = options.screenshots || false;
        this.parallelStreamsCount = options.parallelStreamsCount || 3;
        this.discardOptions = options.discardOptions || {
            //atrule: ['@font-face', /print/],
            decl: [/(.*)transition(.*)/, 'cursor', 'pointer-events', /(-webkit-)?tap-highlight-color/, /(.*)user-select/]
        };
        this.cssUrlOptions = options.cssUrlOptions;
        this.timeout = 60000;
        this.httpService = this.baseURL.protocol.startsWith('https://') ? https : http;
        this.cacheCSSRules = new Map();
    }
    /**
     * Generate file name critical css
     * @param {string} name 
     * @returns {string} file name
     */
    #getFilenameCriticalCSS(name) {
        return this.filename.replace(`[name]`, name);
    }

    /**
     * Return list visible elements
     * @param {*} page Page object (playwright)
     * @returns 
     */
    async getElementsVisible(page) {
        //let result = await page.evaluateHandle(() => {
        //    return new Promise((resolve) => {
        //        const itemsInViewport = new Set();

        //        function getObserver() {
        //            const threshold = 0;
        //            return new IntersectionObserver((entries) => {
        //                entries.forEach(entry => (threshold === 1 ? entry.intersectionRatio === 1 : entry.intersectionRatio > threshold) ? itemsInViewport.add(entry.target) : null);
        //                observer.disconnect();
        //                resolve(itemsInViewport);
        //            }
        //            )
        //        };

        //        const observer = getObserver();

        //        const walker = document.createTreeWalker(document, NodeFilter.SHOW_ELEMENT, () => NodeFilter.FILTER_ACCEPT, true);

        //        while (walker.nextNode()) {
        //            observer.observe(walker.currentNode);
        //        }

        //        requestAnimationFrame(() => { });
        //    });
        //});

        const result = [];
        const elements = await page.$$(`html, body, body *:not(link):not(style):not(script)`);

        let boundingBox;

        for (let el of elements) {
            boundingBox = await el.boundingBox();
            if (boundingBox == null) {
                await el.evaluate((domEl) => { domEl.style.setProperty('display', 'block', 'important'); domEl.style.setProperty('visibility', 'visible', 'important'); });
                boundingBox = await el.boundingBox();
            }

            if (boundingBox != null && boundingBox.y <= page._viewportSize.height) {
                result.push(el);
            }
        }

        return result;
    }
    /**
     * @param {*} page Page object (playwright)
     * @returns 
     */
    async getCSSData(page) {
        const result = await page.evaluate(function ({ cacheFilesNameCSS, grabFontFace }) {
            const siteHostname = window.location.hostname;
            const styleSheetsList = document.styleSheets;
            let data = null;
            //правильный порядок link
            const linksHref = [];
            let rules;
            let href;
            for (let index in styleSheetsList) {
                href = styleSheetsList[index].href;
                if (href != null && siteHostname === (new URL(href)).hostname) {
                    linksHref.push(href);
                    if (cacheFilesNameCSS.includes(href) === false) {
                        rules = Array.from(styleSheetsList[index].cssRules || styleSheetsList[index].rules);
                        data = data || {};
                        data[href] = rules.reduce((prev, current) => {
                            if (current instanceof CSSMediaRule) {
                                prev.push({ conditionText: current.conditionText, rulesMedia: Array.from(current.cssRules).map((subItem) => { return { cssText: subItem.cssText, selectorText: subItem.selectorText } }) });
                            } else if (current.selectorText != null) {
                                prev.push({ cssText: current.cssText, selectorText: current.selectorText });
                            } else if (grabFontFace === true && current.constructor.name === 'CSSFontFaceRule') {
                                prev.push({ cssText: current.cssText, selectorText: 'html' });
                            }

                            return prev;
                        }, []);
                    }
                }
            }
            return { data, linksHref };

        }, { cacheFilesNameCSS: Array.from(this.cacheCSSRules.keys()), grabFontFace: this.grabFontFace });

        if (result.data != null) {
            let postcssPlugins = [];
            let postcssObj;
            const listPromises = [];
            let _cssUrlOptionsNew;

            for (const href of result.linksHref) {
                if (result.data[href] != null) {

                    if (this.discardOptions || this.cssUrlOptions) {


                        if (this.discardOptions) {
                            postcssPlugins.push(postcssDiscard(this.discardOptions));
                        }

                        if (this.cssUrlOptions) {
                            _cssUrlOptionsNew = { ...this.cssUrlOptions };
                        } else {
                            _cssUrlOptionsNew = {
                                //TODO: перенести логику в criticalcssProcess
                                //Проблема: не получается передать параметр "href" 
                                url: (asset, dir, options, decl, warn, result) => {
                                    let newUrl = ``;

                                    if(asset.pathname != null){
                                        let rootPath = href.toLowerCase().replace(this.baseURLRaw.toLowerCase(), '');
                                        let indexStart = rootPath.lastIndexOf('/');
                                        let start = rootPath.substring(0, indexStart);
                                        newUrl = path.join(start, asset.url).replace(/\\/g, '/');
                                    }else{
                                        newUrl = asset.url;
                                    }

                                    return newUrl;
                                }
                            }
                        }

                        postcssPlugins.push(postcssUrl(_cssUrlOptionsNew));

                        postcssObj = postcss(postcssPlugins);

                        for (const item of result.data[href]) {
                            if (item.rulesMedia != null) {
                                listPromises.push(Promise.all(item.rulesMedia.map(x => new Promise((resolve, reject) => {
                                    postcssObj.process(x.cssText, { from: undefined })
                                        .then(({ css }) => resolve({ selectorText: x.selectorText, cssText: css }))
                                        .catch(err => { console.error(err); reject(err); });
                                })))
                                    .then(data => {
                                        return { conditionText: item.conditionText, rulesMedia: data };
                                    })
                                    .catch(err => { console.error(err); reject(err); }));
                            } else {
                                listPromises.push(new Promise((resolve, reject) => {
                                    postcssObj.process(item.cssText, { from: undefined})
                                        .then(({ css }) => { resolve({ selectorText: item.selectorText, cssText: css }) })
                                        .catch(err => { console.error(err); reject(err) });
                                }));
                            }

                        }

                        result.data[href] = await Promise.all(listPromises);

                        this.cacheCSSRules.set(href, result.data[href]);
                    }

                    postcssPlugins.length = 0;
                    listPromises.length = 0;
                }
            }
        }

        return { linksHref: result.linksHref, rules: result.linksHref.map(linksHrefItem => this.cacheCSSRules.get(linksHrefItem)).flat() };
    }
    /**
     * Main method for generate CSS files critical css
     * @param {Map<String, String>|Map<String, String[]>|Map<String, Object>} data
     */
    async generate(data) {
        const _dataCopy = new Map(data);
        const browser = await chromium.launch();
        let contextOptions = {};
        let deviceData;
        let viewport = null;

        if (this.width != null && this.height != null) {
            viewport = { width: this.width, height: this.height };
        }

        if (this.device != null) {
            deviceData = devices[this.device];

            if (viewport == null) {
                viewport = { ...deviceData.viewport };
            }
        }

        contextOptions = { ...deviceData };

        if (viewport != null) {
            contextOptions.viewport = viewport;
        }

        while (_dataCopy.size > 0) {
            await Promise.all(this.runInParallel(Math.min(_dataCopy.size, this.parallelStreamsCount), this.#runItem.bind(this, _dataCopy, browser, contextOptions)));
        }
        await browser.close();
    }

    runInParallel(count, fn) {
        let list = [];
        for (let i = 0; i < count; i++) {
            list.push(fn());
        }
        return list;
    }

    async #runItem(data, browser, contextOptions) {
        if (data == null || data.size === 0) {
            return '';
        }
        let context = await browser.newContext(contextOptions);

        context.setDefaultTimeout(this.timeout);
        
        let _urls;
        let [name, urlList] = data.entries().next().value;

        data.delete(name);

        if (Array.isArray(urlList)) {
            _urls = urlList;
        } else if (typeof urlList === 'string') {
            _urls = [urlList];
        } else if (typeof urlList === 'object') {
            if (urlList.cookies != null) {
                await context.addCookies(urlList.cookies);
            }
            _urls = Array.isArray(urlList.url) ? urlList.url : [urlList.url];
        }

        await this.processItem(context, name, [..._urls]);

        await context.close();
    }
    
    async #processItemGrab(page, name) {

        const { rules } = await this.getCSSData(page);

        const elementsVisible = await this.getElementsVisible(page);

        if (this.screenshots) {
            await page.screenshot({ path: `${this.output}/${name}.png` });
        }

        let criticalcss = await page.evaluate(({ elementsVisible, rules }) => {
            
            function removeSelectors(selectorText){
                return selectorText == null ? '' : selectorText.split(',').filter(x => /(:?:before|:?:after)/g.test(x) === false).join(',');
            }
            
            const result = new Set();
            for (let ruleItem of rules) {
                for (let element of elementsVisible) {
                    if (ruleItem.selectorText == null && window.matchMedia(ruleItem.conditionText).matches === true) {
                        const mediaRules = new Set();

                        for (let { selectorText, cssText } of ruleItem.rulesMedia) {
                            const clearSelector = removeSelectors(selectorText); 
                            if (clearSelector.length > 0 && element.matches(clearSelector)) {
                                mediaRules.add(cssText);
                            }
                        }

                        if (mediaRules.size > 0) {
                            result.add(`@media ${ruleItem.conditionText}{${Array.from(mediaRules).join()}}`);
                        }
                    } else {
                        const clearSelector = removeSelectors(ruleItem.selectorText);
                        if(clearSelector.length > 0 && element.matches(clearSelector)){
                            result.add(ruleItem.cssText);
                        }
                    }
                }
            }
            return Array.from(result).join('');
        }, { elementsVisible, rules });

        if (!existsSync(this.output)) {
            await mkdir(this.output, { recursive: true });
        }

        await writeFile(path.resolve(this.output, this.#getFilenameCriticalCSS(name)), criticalcss);
    }

    async #getPageWorking(context, urlList) {
        const page = await context.newPage();

        let urlItem;

        if (urlList == null || urlList.length === 0) {
            throw Error(`Not finded working page`);
        } else if (Array.isArray(urlList)) {
            urlItem = urlList.pop();
        } else if (typeof urlList === 'string') {
            urlItem = urlList;
        }

        const [_, response] = await Promise.all([
            page.goto(urlItem),
            page.waitForEvent('response', response => response.request().resourceType() === 'document')
        ]);

        if (response.status() > 299) {
            process.stdout.write(`Url "${urlItem}" return status code ${response.status()}\n\r`);
            await page.close();
            return await this.#getPageWorking(context, urlList);
        }
        return page;
    }

    /**
     * 
     * @param {*} page Page object (playwright)
     * @param {string} name 
     * @param {string|string[]|object} urlList 
     */
    async processItem(context, name, urlList) {

        try {
            const page = await this.#getPageWorking(context, urlList);

            await page.waitForLoadState();

            await this.#processItemGrab(page, name);
        } catch (err) {
            process.stderr.write(`Page: ${name}\n\r${err.message}\n\r${err.stack}`);
            process.exit(1);
        }
    }
}

module.exports = {
    Cratocss
}

