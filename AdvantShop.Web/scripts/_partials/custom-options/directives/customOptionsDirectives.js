function customOptionsDirective() {
          return {
              scope: {
                  productId: '@',
                  initFn: '&',
                  changeFn: '&'
              },
              replace: true,
              templateUrl: '/scripts/_partials/custom-options/templates/customOptions.html',
              controller: 'CustomOptionsCtrl',
              controllerAs: 'customOptions',
              bindToController: true
          }
      }

export {
    customOptionsDirective
}