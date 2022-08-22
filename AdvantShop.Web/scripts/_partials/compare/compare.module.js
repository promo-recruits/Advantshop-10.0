import './styles/compare.scss';

import CompareCtrl from './controllers/compareController.js';
import CompareCountCtrl from './controllers/compareCountController.js';
import {
    compareControlDirective,
    compareCountDirective,
    compareRemoveAllDirective,
    compareRemoveDirective
} from './directives/compareDirectives.js';

import compareService from './services/compareService.js';

const moduleName = 'compare';

angular.module(moduleName, [])
    .controller('CompareCtrl', CompareCtrl)
    .controller('CompareCountCtrl', CompareCountCtrl)
    .service('compareService', compareService)
    .directive('compareControl', compareControlDirective)
    .directive('compareCount', compareCountDirective)
    .directive('compareRemoveAll', compareRemoveAllDirective)
    .directive('compareRemove', compareRemoveDirective);

export default moduleName;