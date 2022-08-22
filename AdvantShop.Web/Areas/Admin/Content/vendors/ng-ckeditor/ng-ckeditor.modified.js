(function (angular, factory) {
    if (typeof define === 'function' && define.amd) {
        define(['angular', 'ckeditor'], function (angular) {
            return factory(angular);
        });
    } else {
        return factory(angular);
    }
}(angular || null, function (angular) {
    var app = angular.module('ngCkeditor', ['oc.lazyLoad']);
    var $defer, loaded = false;

    //app.run(['$q', '$timeout', function ($q, $timeout) {
    //    $defer = $q.defer();

    //    if (angular.isUndefined(CKEDITOR)) {
    //        throw new Error('CKEDITOR not found');
    //    }
    //    CKEDITOR.disableAutoInline = true;
    //    function checkLoaded() {
    //        if (CKEDITOR.status == 'loaded') {
    //            loaded = true;
    //            $defer.resolve();
    //        } else {
    //            checkLoaded();
    //        }
    //    }
    //    CKEDITOR.on('loaded', checkLoaded);
    //    //$timeout(checkLoaded, 100);
    //}])

    app.directive('ckeditor', ['$timeout', '$q', '$parse', 'ngCkeditorOptions', '$ocLazyLoad', 'urlHelper', function ($timeout, $q, $parse, ngCkeditorOptions, $ocLazyLoad, urlHelper) {
        'use strict';

        return {
            restrict: 'AC',
            require: ['ngModel', '^?form'],
            scope: false,
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0];
                var form = ctrls[1] || null;
                var EMPTY_HTML = '',
                    isTextarea = element[0].tagName.toLowerCase() == 'textarea',
                    data = [],
                    isReady = false;

                if (!isTextarea) {
                    element.attr('contenteditable', true);
                }

                var onLoad = function () {
                    //var options = {
                    //    toolbar: 'full',
                    //    toolbar_full: [
                    //        { name: 'basicstyles',
                    //            items: [ 'Bold', 'Italic', 'Strike', 'Underline' ] },
                    //        { name: 'paragraph', items: [ 'BulletedList', 'NumberedList', 'Blockquote' ] },
                    //        { name: 'editing', items: ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock' ] },
                    //        { name: 'links', items: [ 'Link', 'Unlink', 'Anchor' ] },
                    //        { name: 'tools', items: [ 'SpellChecker', 'Maximize' ] },
                    //        '/',
                    //        { name: 'styles', items: [ 'Format', 'FontSize', 'TextColor', 'PasteText', 'PasteFromWord', 'RemoveFormat' ] },
                    //        { name: 'insert', items: [ 'Image', 'Table', 'SpecialChar' ] },
                    //        { name: 'forms', items: [ 'Outdent', 'Indent' ] },
                    //        { name: 'clipboard', items: [ 'Undo', 'Redo' ] },
                    //        { name: 'document', items: [ 'PageBreak', 'Source' ] }
                    //    ],
                    //    disableNativeSpellChecker: false,
                    //    uiColor: '#FAFAFA',
                    //    height: '400px',
                    //    width: '100%'
                    //};

                    var options = angular.copy(ngCkeditorOptions);
                    var customOptions = $parse(attrs.ckeditor)(scope);


                    options = angular.extend(options, { contentsCss: [CKEDITOR.getUrl('contents.css'), CKEDITOR.getUrl('contents.custom.css'), './areas/admin/content/styles/common/headers.css'] }, customOptions);

                    var instance = (isTextarea) ? CKEDITOR.replace(element[0], options) : CKEDITOR.inline(element[0], options),
                        configLoaderDef = $q.defer();

                    element[0].ckEditorInstance = instance;

                    element.bind('$destroy', function () {
                        instance.destroy(
                            false //If the instance is replacing a DOM element, this parameter indicates whether or not to update the element with the instance contents.
                        );
                    });
                    var setModelData = function (setPristine, data) {
                        //var data = instance.getData();

                        if (data == '') {
                            data = null;
                        }

                        if (data == ngModel.$viewValue) {
                            return;
                        }

                        $timeout(function () { // for key up event
                            (setPristine !== true || data != ngModel.$viewValue) && ngModel.$setViewValue(data);
                            //(setPristine === true && form) && form.$setPristine();
                        }, 0);
                    }, onUpdateModelData = function (setPristine, value) {
                        if (!data.length) { return; }


                        var item = data.pop() || EMPTY_HTML;
                        isReady = false;
                        instance.setData(item, function () {
                            setModelData(setPristine, value);
                            isReady = true;
                        });
                    };

                    //instance.on('pasteState',   setModelData);
                    instance.on('change', function () {
                        setModelData(false, instance.getData());

                    });
                    instance.on('blur', function () {
                        setModelData(false, instance.getData());
                    });
                    //instance.on('key',          setModelData); // for source view

                    instance.on('instanceReady', function () {
                        scope.$broadcast("ckeditor.ready");
                        scope.$apply(function () {
                            //onUpdateModelData(true, instance.element.$.value);
                            onUpdateModelData(true, ngModel.$modelValue);
                        });

                        instance.document.on("keyup", function () {
                            setModelData(false, instance.getData());
                        });
                    });
                    instance.on('customConfigLoaded', function () {
                        configLoaderDef.resolve();
                    });

                    ngModel.$render = function () {
                        data.push(ngModel.$viewValue);
                        if (isReady) {
                            onUpdateModelData(false, instance.getData());
                        }
                    };
                };

                $q.when(typeof (CKEDITOR) !== 'undefined' && CKEDITOR != null && CKEDITOR.env.isCompatible === true ? true : $ocLazyLoad.load(urlHelper.getAbsUrl('./vendors/ckeditor/ckeditor.js?v=4.11.4', true)))
                    .then(function () {

                        CKEDITOR.disableAutoInline = true;

                        if (CKEDITOR.stylesSet.get('adminStyles') == null) {
                            CKEDITOR.stylesSet.add('adminStyles', [
                                // Block-level styles
                                { name: 'Заголовок', element: 'div', attributes: { 'class': 'h1' } },
                                { name: 'Подзаголовок 2 уровня', element: 'div', attributes: { 'class': 'h2' } },
                                { name: 'Подзаголовок 3 уровня', element: 'div', attributes: { 'class': 'h3' } },
                                { name: 'Подзаголовок 4 уровня', element: 'div', attributes: { 'class': 'h4' } },

                                // Inline styles
                                { name: 'Крупный текст', element: 'span', attributes: { 'class': 'h1' } },
                                { name: 'Большой текст', element: 'span', attributes: { 'class': 'h2' } },
                                { name: 'Средний текст', element: 'span', attributes: { 'class': 'h3' } },
                            ]);
                        }

                        for (name in CKEDITOR.instances) {
                            if (CKEDITOR.instances[name].elementMode === 3) {
                                CKEDITOR.instances[name].destroy(true);
                            }
                        }

                        onLoad();
                    });
            }
        };
    }]);


    app.constant('ngCkeditorOptions', {
        autoParagraph: false,
        forceSimpleAmpersand: true,
        height: '250px',
        uiColor: '#FAFAFA',
        toolbar: [
            { name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source', '-', 'Templates'] },
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', '-', 'RemoveFormat'] },
            { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
            { name: 'colors', items: ['TextColor', 'BGColor'] },
            { name: 'tools', items: ['Maximize'] },
            '/',
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
            { name: 'links', items: ['Link', 'Unlink'] },
            { name: 'insert', items: ['Image', 'Table'] },
            '/',
            { name: 'styles', items: ['Styles', 'Font', 'FontSize', 'lineheight'] },
            { name: 'icons', items: ['Emojione'] }
        ],
        toolbarGroups: [
            { name: 'document', groups: ['mode', 'document', 'doctools'] },
            { name: 'clipboard', groups: ['clipboard', 'undo'] },
            { name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
            { name: 'forms' },
            '/',
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
            { name: 'links' },
            { name: 'insert' },
            '/',
            { name: 'styles' },
            { name: 'colors' },
            { name: 'tools' },
            { name: 'others' },
            { name: 'about' }
        ],
        stylesSet: 'adminStyles'
    });

    return app;
}));
