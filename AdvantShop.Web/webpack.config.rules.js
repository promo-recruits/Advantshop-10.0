const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = env => {
    return [
        {
            test: /\.(css|scss)$/,
            use: [{
                loader: MiniCssExtractPlugin.loader,
                options: {
                    publicPath: '',
                },
            },
            {
                loader: 'css-loader',
                options: {
                    sourceMap: true,
                    modules: false,
                    url: {
                        filter: function (url, resourcePath) {
                            const regExpTpl = /^\\Templates/g;
                            const clearPath = path.resolve(path.dirname(resourcePath), url).replace(__dirname, '');
                            const isTemplateSrc = regExpTpl.test(clearPath);
                            if (isTemplateSrc) {
                                return true;
                            }
                            const regExp = /\.(woff|woff2|ttf|eot)$/ig;
                            return regExp.test(clearPath) === false;

                        }
                    }
                },
            },
            {
                loader: 'postcss-loader',
                options: {
                    sourceMap: true,
                    postcssOptions: {
                        variables: { ...env }
                    }
                }
            },
            {
                loader: 'sass-loader',
                options: {
                    sourceMap: true,
                }
            }]
        },
        {
            test: /\.(png|jpg|gif|webp|jpeg)$/,
            type: 'asset/resource',
            generator: {
                filename: '[name].[contenthash][ext]',
            }
        },
        {
            test: /\.(svg)$/,
            type: 'asset/resource',
            generator: {
                filename: '[name].[contenthash][ext]',
            }
        },
        {
            test: /\.(woff|woff2|ttf|eot)$/i,
            type: 'asset/resource',
            generator: {
                filename: '[name].[contenthash][ext]',
            }
        },
        {
            test: /\.js$/,
            exclude: [/node_modules/],
            use: [
                // Из-за того что падает PubSub
                // {
                //     loader: 'thread-loader'
                // },
                {
                    loader: 'babel-loader',
                    options: {
                        cacheDirectory: true,
                        configFile: path.resolve(__dirname, '.babelrc')
                    }
                }]
        },
        //проталкиваем jquery в глобальное пространство
        {
            test: require.resolve('jquery'),
            loader: 'expose-loader',
            options: {
                exposes: [{
                    globalName: "$",
                    override: true,
                }, {
                    globalName: "jQuery",
                    override: true,
                }]
            },
        },
        /*{
          test: require.resolve('./scripts/_common/PubSub/PubSub.js'),
          loader: 'expose-loader',
          options: {
              exposes: [{
                  globalName: "PubSub",
                  moduleLocalName: "PubSub",
                  override: true
              }]
          },
        },*/
        {
            test: require.resolve('./node_modules/sweetalert2/dist/sweetalert2.js'),
            loader: 'expose-loader',
            options: {
                exposes: [{
                    globalName: "Sweetalert2",
                    override: true,
                }]
            },
        },
        //{
        //    test: require.resolve('./node_modules/sweetalert2/dist/sweetalert2.js'),
        //    loader: 'imports-loader',
        //    options: {
        //        imports: 'side-effects sweetalert2',
        //        wrapper: 'window',
        //        additionalCode: 'var define = false; /* Disable AMD for misbehaving libraries */ var exports = false; /* Disable exports for misbehaving libraries */',
        //    },
        //},
        {
            test: require.resolve('angular-ladda'),
            loader: 'imports-loader',
            options: {
                type: 'commonjs',
                imports: 'pure angular-ladda',
                additionalCode: `var define = false; /* Disable AMD for misbehaving libraries */`,
            },
        },
        {
            test: require.resolve('angularjs-color-picker'),
            loader: 'imports-loader',
            options: {
                type: 'commonjs',
                imports: 'pure angularjs-color-picker',
                additionalCode: `var tinycolor=tinycolor2;`,
            },
        },
        {
            test: /\.html$/i,
            type: 'asset/resource'
        }
    ]
}