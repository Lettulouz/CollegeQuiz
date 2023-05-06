/// <binding />
"use strict";

var path = require("path");
var WebpackNotifierPlugin = require("webpack-notifier");
var BrowserSyncPlugin = require("browser-sync-webpack-plugin");

module.exports = {
    entry: {
        quizManagerRenderer: [ "babel-polyfill", "./React/src/quiz-manager-renderer.js" ],
        quizQuestionsRenderer: [ "babel-polyfill", "./React/src/quiz-questions-renderer.js" ],
        quizSessionRenderer: [ "babel-polyfill", "./React/src/quiz-session-renderer.js" ],
    },
    output: {
        path: path.resolve(__dirname, "./wwwroot/js/bundles"),
        filename: "[name].js"
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: [ 'babel-loader' ],
            },
            {
                test: /\.css$/i,
                use: [ "style-loader", "css-loader" ],
            },
        ],
    },
    devtool: "inline-source-map",
    plugins: [ new WebpackNotifierPlugin(), new BrowserSyncPlugin() ],
    resolve: {
        extensions: [ ".js", ".jsx" ],
    }
};