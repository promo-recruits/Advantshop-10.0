import '../../styles/partials/login.scss';
import '../../styles/views/login.scss';

import authModule from '../auth/auth.module.js';

const moduleName = 'login';

angular.module('login', [authModule])
    .controller('LoginCtrl', function () { });

export default moduleName;