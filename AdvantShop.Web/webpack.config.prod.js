const TerserPlugin = require('terser-webpack-plugin');
const { merge }  = require('webpack-merge');
const common = require('./webpack.config.js');

const config = {
    mode: 'production',
    devtool: false,
    optimization: {
        minimizer: [new TerserPlugin({
            terserOptions: {
                output: {
                    comments: false,
                },
            },
            extractComments: false
        })],
    }
};

module.exports = env => merge(common(env), config);