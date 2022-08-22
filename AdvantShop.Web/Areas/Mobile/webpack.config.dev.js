const { merge } = require('webpack-merge');
const configCommon = require('./webpack.config.js');
const configDevStandart = require('../../webpack.config.dev.js');

module.exports = env => merge(configCommon(env), configDevStandart(env));