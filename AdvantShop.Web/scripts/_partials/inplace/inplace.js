; (function (ng) {
    'use strict';

    angular.module('inplace', [])
      .constant('inplaceRichConfig', {
          title: false,
          allowedContent: true,
          autoParagraph: false,
          removePlugins: 'dragdrop, basket',
          floatSpaceDockedOffsetY: 5,
          toolbar: [
              { name: 'source', items: ['Sourcedialog', 'Templates'] },
              { name: 'elements', items: ['NumberedList', 'BulletedList', 'Link', 'Unlink', '-', 'Image', 'Flash', 'Table', 'HorizontalRule'] },
              { name: 'styles', items: ['Styles', 'Font', 'FontSize','lineheight'] },
              '/',
              { name: 'text', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'RemoveFormat'] },
              { name: 'text', items: ['TextColor', 'BGColor'] },
              { name: 'align', items: ['Outdent', 'Indent', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
              { name: 'document', items: ['PasteFromWord', 'Undo', 'Redo', 'Scayt'] },
		      { name: 'icons', items: ['Emojione']}
          ],
          extraPlugins: 'scriptencode,sourcedialog,codemirror,lineheight,bold,emojione,fontSizeExtend',
          codemirror: {},
          stylesSet: 'inplaceStyles'
      });

})(window.angular);