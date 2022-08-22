function colorsViewerDirective() {
          return {
              require: {
                  carousel: '?^carousel'
              },
              restrict: 'A',
              replace: true,
              templateUrl: '/scripts/_partials/colors-viewer/templates/colors.html',
              controller: 'ColorsViewerCtrl',
              controllerAs: 'colorsViewer',
              bindToController: true,
              scope: {
                  colors: '=',
                  colorSelected: '=?',
                  startSelectedColors: '<?',
                  changeStartSelectedColor: '<?',
                  colorWidth: '=?',
                  colorHeight: '=?',
                  initColors: '&',
                  changeColor: '&',
                  multiselect: '<?',
                  imageType: '@',
                  viewMode: '@',
                  isHiddenColorName: '<?'
              }
          };
      };

export {
    colorsViewerDirective
}