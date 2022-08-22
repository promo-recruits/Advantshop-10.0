function sizesViewerDirective() {
          return {
              restrict: 'A',
              replace: true,
              templateUrl: '/scripts/_partials/sizes-viewer/templates/sizes.html',
              controller: 'SizesViewerCtrl',
              controllerAs: 'sizesViewer',
              bindToController: true,
              scope: {
                  sizes: '<?',
                  sizeSelected: '=?',
                  initSizes: '&',
                  changeSize: '&',
                  startSelectedSizes: '<?',
              }
          }
};

export {
    sizesViewerDirective
}