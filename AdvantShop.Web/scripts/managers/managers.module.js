import '../../styles/views/managers.scss';
import '../../styles/views/news.scss';

import ManagersCtrl from './controllers/managersController.js';
import managersService from './services/managersService.js';

const moduleName = 'managers';

angular.module(moduleName, [])
    .service('managersService', managersService)
    .controller('ManagersCtrl', ManagersCtrl);

export default moduleName;