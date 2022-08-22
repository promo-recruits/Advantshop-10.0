const { merge } = require('webpack-merge');
const configCommon = require('./webpack.config.js');
const configProdStandart = require('../../webpack.config.prod.js');

module.exports = env => merge(configCommon(env), configProdStandart(env));