import './styles/address.scss';

import AddressListCtrl from './controllers/addressListController.js';
import { addressListDirective } from './directives/addressDirectives.js';
import addressService from './services/addressService.js';
import zone from '../zone/zone.module.js';


const moduleName = 'address';

angular.module(moduleName, [zone])
    .service('addressService', addressService)
    .controller('AddressListCtrl', AddressListCtrl)
    .directive('addressList', addressListDirective);

export default moduleName;