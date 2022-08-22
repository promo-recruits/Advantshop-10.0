const webpack = require('webpack');
const { checkFileForReplace } = require('../../node_scripts/templateHelper.js');

let replaceFiles = new Map();

replaceFiles.set(new RegExp('../../styles/views/news.scss$'), checkFileForReplace(__dirname + '../../../node_scripts/noop.js'));
replaceFiles.set(new RegExp('../../styles/views/contacts.scss$'), checkFileForReplace(__dirname + '../../../node_scripts/noop.js'));

let plugins = [];

replaceFiles.forEach(function (value, key) {
	plugins.push(new webpack.NormalModuleReplacementPlugin(key, value));
});

const config = {
	plugins: plugins
};

module.exports = env => config;

