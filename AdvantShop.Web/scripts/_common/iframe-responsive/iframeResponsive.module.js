import './styles/iframe-responsive.css';

import iframeResponsiveCtrl from './controllers/iframeResponsiveController.js';
import iframeResponsiveDirective from './directives/iframeResponsiveDirective.js';
import iframeResponsiveService from './services/iframeResponsiveService.js';

const moduleName = 'iframeResponsive';

angular.module(moduleName, [])
    .directive('iframeResponsive', iframeResponsiveDirective)
    .service('iframeResponsiveService', iframeResponsiveService)
    .controller('IframeResponsiveCtrl', iframeResponsiveCtrl);


export default moduleName;