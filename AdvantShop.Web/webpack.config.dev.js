const { merge }  = require('webpack-merge');
const common = require('./webpack.config.js');

const config = {
    mode: 'development',
    devtool: 'cheap-source-map'
};

module.exports = env =>  merge(common(env), config);