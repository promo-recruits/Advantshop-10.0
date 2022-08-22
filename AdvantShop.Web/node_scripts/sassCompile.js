const sass = require('sass');
const fs = require('fs');
const path = require('path');
/**
 * Фунция для компилирования SCSS в CSS
 * @param {string} filePath - директория, где находится SCSS файл
 * @param {string} name - название файла (без расширения)
 */
function compileSync(filePath, name) {
    const result = sass.renderSync({ file: path.join(filePath, `${name}.scss`), linefeed: 'crlf' });
    fs.writeFileSync(path.join(filePath, `${name}.css`), result.css);
}

exports.compileSync = compileSync;