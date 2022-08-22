const yargs = require('yargs');
const child_process = require('child_process');
const path = require('path');
const chalk = require('chalk');
const fs = require('fs');
const argv = yargs
    .scriptName('critical-css')
    .usage('$0 <cmd> [args]')
    .option('name', {
        alias: 'n',
        description: 'Template Name',
        type: 'string',
        required: true
    })
    .help()
    .alias('help', 'h')
    .argv;

const sourcePath = path.resolve(__dirname, `../../templates/origin/`);
const resultPath = path.resolve(__dirname, `../templates/${argv.name}`);

if (fs.existsSync(resultPath)) {
    console.error(chalk`{red Шаблон "${argv.name}" уже существует}`);
    return 1;
}

child_process.execSync(`xcopy /I /E ${sourcePath} ${resultPath}`);

console.log(chalk`{green Template "${argv.name}" is created}`);