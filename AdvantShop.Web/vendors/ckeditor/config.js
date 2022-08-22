/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';


    config.language = window.CKEDITOR_LANGUAGE || 'en';

    config.image_prefillDimensions = false;

    config.allowedContent = true;

    config.baseHref = CKEDITOR_BASEPATH.replace('vendors/ckeditor/', '');

    //#region filemanager
    config.filebrowserBrowseUrl = CKEDITOR_BASEPATH + 'plugins/fileman/index.html?rnd=0';
    config.filebrowserImageBrowseUrl = CKEDITOR_BASEPATH + 'plugins/fileman/index.html?type=image&rnd=0';
    config.removeDialogTabs = 'link:upload;image:upload';
    //#endregion

    Object.keys(CKEDITOR.dtd.$removeEmpty).forEach(function (currentValue) {
        CKEDITOR.dtd.$removeEmpty[currentValue] = 0;
    });

    config.extraPlugins = 'codemirror,lineheight,bold,emojione,fontSizeExtend,html5validation';

    config.font_names = 'Arial/Arial, Helvetica, sans-serif;' +
	//'Comic Sans MS/Comic Sans MS, cursive;' + // not supported in IOS
	'Courier New/Courier New, Courier, monospace;' +
	'Georgia/Georgia, serif;' +
	'Lucida Sans Unicode/Lucida Sans Unicode, Lucida Grande, sans-serif;' +
	'Tahoma/Tahoma, Geneva, sans-serif;' +
	'Times New Roman/Times New Roman, Times, serif;' +
    'Trebuchet MS/Trebuchet MS, Helvetica, sans-serif;' +
	'Verdana/Verdana, Geneva, sans-serif';

    config.templates_files = [CKEDITOR.getUrl('plugins/templates/templates/advantshop.js')];
    config.templates = 'advantshop';
    config.templates_replaceContent = false;

    config.contentsCss = [CKEDITOR.getUrl('contents.css'), CKEDITOR.getUrl('contents.custom.css')]

    config.disableNativeSpellChecker = false;
};

CKEDITOR.on('dialogDefinition', function (e) {
    // NOTE: this is an instance of CKEDITOR.dialog.definitionObject
    var dialogDefinition = e.data.definition;

    var dialogProcess = {
        'link': function () {
            var infoTab = dialogDefinition.getContents('info');
            infoTab.remove('protocol');

            var url = infoTab.get('url');
            url.onKeyUp = function () { };
            url.setup = function (data) {
                this.allowOnChange = false;
                if (data.url) {
                    var value = '';
                    if (data.url.protocol) {
                        value += data.url.protocol;
                    }
                    if (data.url.url) {
                        value += data.url.url;
                    }
                    this.setValue(value);
                }
                this.allowOnChange = true;
            };

            url.commit = function (data) {
                data.url = { protocol: '', url: this.getValue() };
            };

            //удаляем тип ссылки якорь
            infoTab.get('linkType').items.splice(1, 1);
        },
        'table': function () {
            var infoTab = dialogDefinition.getContents('info');

            infoTab.get('txtWidth')['default'] = null;
            infoTab.get('txtBorder')['default'] = null;
            infoTab.get('txtCellPad')['default'] = null;
            infoTab.get('txtCellSpace')['default'] = null;
        }
    }

    if (dialogProcess[e.data.name] != null) {
        dialogProcess[e.data.name]();
    }
});

CKEDITOR.on('instanceReady', function (event) {
    var editor = event.editor;

    if (editor.mode === 'wysiwyg' && editor.document.$.defaultView.frameElement != null && editor.document.$.defaultView.frameElement.classList.contains('cke_wysiwyg_frame') === true) {

        editor.document.on('click', function (e) {

            var body = editor.window.$.top.document.body;
            //принудительно вызываем клик на боди родителя, чтобы другие компоненты отловили событие
            editor.document.$.defaultView.top.document.body.click();
        });
    }
});
